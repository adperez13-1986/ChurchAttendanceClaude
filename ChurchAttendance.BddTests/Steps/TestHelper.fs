module ChurchAttendance.BddTests.TestHelper

open System
open System.IO
open ChurchAttendance

/// Mutable shared state for step definitions within a scenario.
let mutable private _members: Map<string, Member> = Map.empty
let mutable private _lastResponse: string option = None
let mutable private _lastPdfBytes: byte[] option = None

let private getDataDir () =
    let home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
    let dir = Path.Combine(home, ".church-attendance", "data")
    if not (Directory.Exists dir) then
        Directory.CreateDirectory dir |> ignore
    dir

let cleanDataFiles () =
    let dir = getDataDir ()
    let membersFile = Path.Combine(dir, "members.json")
    let attendanceFile = Path.Combine(dir, "attendance.json")
    if File.Exists membersFile then File.Delete membersFile
    if File.Exists attendanceFile then File.Delete attendanceFile
    _members <- Map.empty
    _lastResponse <- None
    _lastPdfBytes <- None

let initEnv () =
    Database.ensureDataDir ()
    cleanDataFiles ()

let members () = _members
let setMembers m = _members <- m

let lastResponse () = _lastResponse
let setLastResponse r = _lastResponse <- Some r

let lastPdfBytes () = _lastPdfBytes
let setLastPdfBytes b = _lastPdfBytes <- Some b

let addMemberDirect (name: string) (ageGroup: AgeGroup) (category: Category) (firstAttended: DateTime option) =
    let m = Domain.newMember name ageGroup category firstAttended
    Database.addMember m
    _members <- _members |> Map.add name m
    m

let findMemberByName (name: string) =
    Database.getMembers ()
    |> List.tryFind (fun m -> m.FullName = name)
