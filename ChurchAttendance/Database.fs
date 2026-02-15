namespace ChurchAttendance

open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization

module Database =

    let private dataDir =
        let home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        Path.Combine(home, ".church-attendance", "data")

    let private membersFile = Path.Combine(dataDir, "members.json")
    let private attendanceFile = Path.Combine(dataDir, "attendance.json")


    let private lockObj = obj ()

    let private jsonOptions =
        let opts = JsonSerializerOptions(WriteIndented = true)
        let unionEncoding = JsonUnionEncoding.UnwrapFieldlessTags ||| JsonUnionEncoding.Default
        opts.Converters.Add(JsonFSharpConverter(unionEncoding = unionEncoding))
        opts

    let private oldDataDir =
        Path.Combine(AppContext.BaseDirectory, "data")

    let ensureDataDir () =
        if not (Directory.Exists dataDir) then
            Directory.CreateDirectory dataDir |> ignore

        // Migration hint: if old location has data but new location is empty
        let oldMembersFile = Path.Combine(oldDataDir, "members.json")
        if File.Exists oldMembersFile && not (File.Exists membersFile) then
            printfn ""
            printfn "============================================================"
            printfn " DATA MIGRATION NOTICE"
            printfn "============================================================"
            printfn " Data files found in the old location (build output):"
            printfn "   %s" oldDataDir
            printfn ""
            printfn " The app now stores data in:"
            printfn "   %s" dataDir
            printfn ""
            printfn " To migrate your existing data, run:"
            printfn "   cp %s/* %s/" oldDataDir dataDir
            printfn "============================================================"
            printfn ""

    let private readFile<'T> (path: string) (defaultValue: 'T) : 'T =
        lock lockObj (fun () ->
            if File.Exists path then
                let json = File.ReadAllText path
                if String.IsNullOrWhiteSpace json then
                    defaultValue
                else
                    JsonSerializer.Deserialize<'T>(json, jsonOptions)
            else
                defaultValue)

    let private writeFile<'T> (path: string) (data: 'T) =
        lock lockObj (fun () ->
            let json = JsonSerializer.Serialize(data, jsonOptions)
            File.WriteAllText(path, json))

    // Members
    let getMembers () : Member list = readFile membersFile []

    let saveMembers (members: Member list) = writeFile membersFile members

    let getMember (id: Guid) =
        getMembers () |> List.tryFind (fun m -> m.Id = id)

    let addMember (m: Member) =
        let members = getMembers ()
        saveMembers (members @ [ m ])

    let updateMember (m: Member) =
        let members =
            getMembers ()
            |> List.map (fun existing -> if existing.Id = m.Id then m else existing)

        saveMembers members

    let deactivateMember (id: Guid) =
        match getMember id with
        | Some m -> updateMember { m with IsActive = false }
        | None -> ()

    // Attendance
    let getAttendance () : AttendanceRecord list =
        readFile attendanceFile []

    let saveAttendance (records: AttendanceRecord list) =
        writeFile attendanceFile records

    let getAttendanceForDate (date: DateTime) (serviceType: ServiceType) =
        getAttendance ()
        |> List.tryFind (fun r -> r.Date.Date = date.Date && r.ServiceType = serviceType)

    let saveAttendanceRecord (record: AttendanceRecord) =
        lock lockObj (fun () ->
            let records: AttendanceRecord list = readFile attendanceFile []

            let updated =
                match
                    records
                    |> List.tryFindIndex (fun r ->
                        r.Date.Date = record.Date.Date && r.ServiceType = record.ServiceType)
                with
                | Some idx -> records |> List.mapi (fun i r -> if i = idx then record else r)
                | None -> records @ [ record ]

            let json = JsonSerializer.Serialize(updated, jsonOptions)
            File.WriteAllText(attendanceFile, json))

    let toggleAttendanceMember (date: DateTime) (serviceType: ServiceType) (memberId: Guid) (add: bool) : int =
        lock lockObj (fun () ->
            let records: AttendanceRecord list = readFile attendanceFile []

            let idx =
                records
                |> List.tryFindIndex (fun r ->
                    r.Date.Date = date.Date && r.ServiceType = serviceType)

            let record =
                match idx with
                | Some i -> records.[i]
                | None ->
                    { Id = Guid.NewGuid()
                      Date = date
                      ServiceType = serviceType
                      MemberIds = [] }

            let updatedIds =
                if add then
                    if record.MemberIds |> List.contains memberId then
                        record.MemberIds
                    else
                        record.MemberIds @ [ memberId ]
                else
                    record.MemberIds |> List.filter (fun id -> id <> memberId)

            let updatedRecord = { record with MemberIds = updatedIds }

            let updatedRecords =
                match idx with
                | Some i -> records |> List.mapi (fun j r -> if j = i then updatedRecord else r)
                | None -> records @ [ updatedRecord ]

            let json = JsonSerializer.Serialize(updatedRecords, jsonOptions)
            File.WriteAllText(attendanceFile, json)

            updatedIds.Length)

    /// Returns the set of member IDs whose FirstAttendedDate matches the given date.
    let getFirstTimerIds (date: DateTime) : Set<Guid> =
        getMembers ()
        |> List.filter (fun m ->
            match m.FirstAttendedDate with
            | Some firstDate -> firstDate.Date = date.Date
            | None -> false)
        |> List.map (fun m -> m.Id)
        |> Set.ofList

