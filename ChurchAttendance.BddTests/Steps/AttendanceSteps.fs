module ChurchAttendance.BddTests.AttendanceSteps

open System
open global.TickSpec
open global.Xunit
open ChurchAttendance
open ChurchAttendance.BddTests.TestHelper

/// Helper to get column index from table header
let private colIndex (table: Table) (colName: string) =
    table.Header
    |> Array.findIndex (fun h -> h.Trim() = colName)

let [<Given>] ``the following members exist:`` (table: Table) =
    let nameIdx = colIndex table "name"
    let ageIdx = colIndex table "ageGroup"
    let catIdx = colIndex table "category"
    for row in table.Rows do
        let name = row.[nameIdx].Trim()
        let ageGroupStr = row.[ageIdx].Trim()
        let categoryStr = row.[catIdx].Trim()
        let ageGroup = Domain.parseAgeGroup ageGroupStr |> Option.defaultValue Men
        let category = Domain.parseCategory categoryStr |> Option.defaultValue Member
        addMemberDirect name ageGroup category None |> ignore

let [<Given>] ``attendance is recorded for today with service type "(.*)" for "(.*)"`` (serviceTypeStr: string) (name: string) =
    let serviceType = Domain.parseServiceType serviceTypeStr |> Option.defaultValue SundayService
    match findMemberByName name with
    | Some m ->
        let record : AttendanceRecord =
            { Id = Guid.NewGuid()
              Date = DateTime.Today
              ServiceType = serviceType
              MemberIds = [ m.Id ] }
        Database.saveAttendanceRecord record
    | None -> failwith $"Member '{name}' not found"

let [<Given>] ``attendance is recorded for today with all members present`` () =
    let allMembers = Database.getMembers ()
    let memberIds = allMembers |> List.map (fun m -> m.Id)
    let record : AttendanceRecord =
        { Id = Guid.NewGuid()
          Date = DateTime.Today
          ServiceType = SundayService
          MemberIds = memberIds }
    Database.saveAttendanceRecord record

let [<Given>] ``attendance is recorded for today with "(.*)" present`` (name: string) =
    match findMemberByName name with
    | Some m ->
        let record : AttendanceRecord =
            { Id = Guid.NewGuid()
              Date = DateTime.Today
              ServiceType = SundayService
              MemberIds = [ m.Id ] }
        Database.saveAttendanceRecord record
    | None -> failwith $"Member '{name}' not found"

let [<When>] ``I record attendance for today with service type "(.*)" for members:`` (serviceTypeStr: string) (table: Table) =
    let serviceType = Domain.parseServiceType serviceTypeStr |> Option.defaultValue SundayService
    let nameIdx = colIndex table "name"
    let memberIds =
        [ for row in table.Rows do
            let name = row.[nameIdx].Trim()
            match findMemberByName name with
            | Some m -> yield m.Id
            | None -> failwith $"Member '{name}' not found" ]
    let record : AttendanceRecord =
        { Id = Guid.NewGuid()
          Date = DateTime.Today
          ServiceType = serviceType
          MemberIds = memberIds }
    Database.saveAttendanceRecord record

let [<When>] ``I load the attendance list for today`` () =
    let activeMembers = Database.getMembers () |> List.filter (fun m -> m.IsActive)
    let firstTimerIds = Database.getFirstTimerIds DateTime.Today
    let memberInfo =
        activeMembers
        |> List.map (fun m ->
            let isFirstTimer = firstTimerIds.Contains m.Id
            $"{m.FullName}|active={m.IsActive}|firstTimer={isFirstTimer}")
        |> String.concat "\n"
    setLastResponse memberInfo

let [<Then>] ``the attendance count for today should be (\d+)`` (expected: int) =
    let allRecords = Database.getAttendance ()
    let todayMemberIds =
        allRecords
        |> List.filter (fun r -> r.Date.Date = DateTime.Today)
        |> List.collect (fun r -> r.MemberIds)
        |> List.distinct
    Assert.Equal(expected, todayMemberIds.Length)

let [<Then>] ``the attendance for today with service type "(.*)" should have (\d+) member`` (serviceTypeStr: string) (expected: int) =
    let serviceType = Domain.parseServiceType serviceTypeStr |> Option.defaultValue SundayService
    match Database.getAttendanceForDate DateTime.Today serviceType with
    | Some record -> Assert.Equal(expected, record.MemberIds.Length)
    | None -> Assert.Fail($"No attendance record found for today with service type {serviceTypeStr}")

let [<Then>] ``"(.*)" should be marked as a first timer`` (name: string) =
    let response = lastResponse ()
    match response with
    | Some r ->
        let line = r.Split('\n') |> Array.tryFind (fun l -> l.StartsWith(name))
        match line with
        | Some l -> Assert.Contains("firstTimer=True", l)
        | None -> Assert.Fail($"'{name}' not found in attendance list")
    | None -> Assert.Fail("No attendance list was loaded")

let [<Then>] ``"(.*)" should appear in the attendance list`` (name: string) =
    let response = lastResponse ()
    match response with
    | Some r -> Assert.Contains(name, r)
    | None -> Assert.Fail("No attendance list was loaded")

let [<Then>] ``"(.*)" should not appear in the attendance list`` (name: string) =
    let response = lastResponse ()
    match response with
    | Some r -> Assert.DoesNotContain(name, r)
    | None -> Assert.Fail("No attendance list was loaded")
