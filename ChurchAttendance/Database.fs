namespace ChurchAttendance

open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization

module Database =

    let private baseDir =
        let home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
        Path.Combine(home, ".church-attendance")

    let private tenantsFile = Path.Combine(baseDir, "tenants.json")

    let private lockObj = obj ()

    let private jsonOptions =
        let opts = JsonSerializerOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)
        let unionEncoding = JsonUnionEncoding.UnwrapFieldlessTags ||| JsonUnionEncoding.Default
        opts.Converters.Add(JsonFSharpConverter(unionEncoding = unionEncoding))
        opts

    let private dataDirFor (tenant: string) =
        Path.Combine(baseDir, tenant)

    let private membersFileFor (tenant: string) =
        Path.Combine(dataDirFor tenant, "members.json")

    let private attendanceFileFor (tenant: string) =
        Path.Combine(dataDirFor tenant, "attendance.json")

    // Tenant config loading
    let loadTenantsConfig () : TenantsConfig =
        if File.Exists tenantsFile then
            let json = File.ReadAllText tenantsFile
            JsonSerializer.Deserialize<TenantsConfig>(json, jsonOptions)
        else
            let defaultPassword =
                match Environment.GetEnvironmentVariable("APP_PASSWORD") with
                | null | "" ->
                    printfn "WARNING: APP_PASSWORD not set — using default password. Set APP_PASSWORD environment variable or create tenants.json."
                    "changeme"
                | pwd -> pwd

            let config =
                { Default = "default"
                  Domain = ""
                  Tenants = Map.ofList [ "default", { Name = "Church Attendance"; Password = defaultPassword } ] }

            if not (Directory.Exists baseDir) then
                Directory.CreateDirectory baseDir |> ignore

            let json = JsonSerializer.Serialize(config, jsonOptions)
            File.WriteAllText(tenantsFile, json)
            printfn "Created default tenants.json at %s" tenantsFile
            config

    let mutable tenantConfig = loadTenantsConfig ()

    let private oldDataDir =
        Path.Combine(baseDir, "data")

    let ensureDataDir () =
        // Create directories for all configured tenants
        for kvp in tenantConfig.Tenants do
            let dir = dataDirFor kvp.Key
            if not (Directory.Exists dir) then
                Directory.CreateDirectory dir |> ignore

        // Migration hint: if old ~/.church-attendance/data/ location has data
        let oldMembersFile = Path.Combine(oldDataDir, "members.json")
        let anyTenantHasData =
            tenantConfig.Tenants
            |> Map.exists (fun slug _ -> File.Exists (membersFileFor slug))

        if File.Exists oldMembersFile && not anyTenantHasData then
            let defaultTenant = tenantConfig.Default
            printfn ""
            printfn "============================================================"
            printfn " DATA MIGRATION NOTICE"
            printfn "============================================================"
            printfn " Data files found in the old location:"
            printfn "   %s" oldDataDir
            printfn ""
            printfn " The app now stores data per-tenant. To migrate, run:"
            printfn "   cp %s/* %s/" oldDataDir (dataDirFor defaultTenant)
            printfn "============================================================"
            printfn ""

        // Also check AppContext.BaseDirectory/data/ (original old location)
        let legacyDataDir = Path.Combine(AppContext.BaseDirectory, "data")
        let legacyMembersFile = Path.Combine(legacyDataDir, "members.json")
        if File.Exists legacyMembersFile && not anyTenantHasData && not (File.Exists oldMembersFile) then
            let defaultTenant = tenantConfig.Default
            printfn ""
            printfn "============================================================"
            printfn " DATA MIGRATION NOTICE"
            printfn "============================================================"
            printfn " Data files found in the old location (build output):"
            printfn "   %s" legacyDataDir
            printfn ""
            printfn " The app now stores data per-tenant. To migrate, run:"
            printfn "   cp %s/* %s/" legacyDataDir (dataDirFor defaultTenant)
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
    let getMembers (tenant: string) : Member list =
        readFile (membersFileFor tenant) []

    let saveMembers (tenant: string) (members: Member list) =
        writeFile (membersFileFor tenant) members

    let getMember (tenant: string) (id: Guid) =
        getMembers tenant |> List.tryFind (fun m -> m.Id = id)

    let addMember (tenant: string) (m: Member) =
        let members = getMembers tenant
        saveMembers tenant (members @ [ m ])

    let updateMember (tenant: string) (m: Member) =
        let members =
            getMembers tenant
            |> List.map (fun existing -> if existing.Id = m.Id then m else existing)

        saveMembers tenant members

    let deactivateMember (tenant: string) (id: Guid) =
        match getMember tenant id with
        | Some m -> updateMember tenant { m with IsActive = false }
        | None -> ()

    // Attendance
    let getAttendance (tenant: string) : AttendanceRecord list =
        readFile (attendanceFileFor tenant) []

    let saveAttendance (tenant: string) (records: AttendanceRecord list) =
        writeFile (attendanceFileFor tenant) records

    let getAttendanceForDate (tenant: string) (date: DateTime) (serviceType: ServiceType) =
        getAttendance tenant
        |> List.tryFind (fun r -> r.Date.Date = date.Date && r.ServiceType = serviceType)

    let saveAttendanceRecord (tenant: string) (record: AttendanceRecord) =
        lock lockObj (fun () ->
            let records: AttendanceRecord list = readFile (attendanceFileFor tenant) []

            let updated =
                match
                    records
                    |> List.tryFindIndex (fun r ->
                        r.Date.Date = record.Date.Date && r.ServiceType = record.ServiceType)
                with
                | Some idx -> records |> List.mapi (fun i r -> if i = idx then record else r)
                | None -> records @ [ record ]

            let json = JsonSerializer.Serialize(updated, jsonOptions)
            File.WriteAllText(attendanceFileFor tenant, json))

    let toggleAttendanceMember (tenant: string) (date: DateTime) (serviceType: ServiceType) (memberId: Guid) (add: bool) : int =
        lock lockObj (fun () ->
            let records: AttendanceRecord list = readFile (attendanceFileFor tenant) []

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
            File.WriteAllText(attendanceFileFor tenant, json)

            updatedIds.Length)

    /// Returns the set of member IDs whose FirstAttendedDate matches the given date.
    let getFirstTimerIds (tenant: string) (date: DateTime) : Set<Guid> =
        getMembers tenant
        |> List.filter (fun m ->
            match m.FirstAttendedDate with
            | Some firstDate -> firstDate.Date = date.Date
            | None -> false)
        |> List.map (fun m -> m.Id)
        |> Set.ofList
