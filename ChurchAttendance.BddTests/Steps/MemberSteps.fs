module ChurchAttendance.BddTests.MemberSteps

open System
open global.TickSpec
open global.Xunit
open ChurchAttendance
open ChurchAttendance.BddTests.TestHelper

let [<Given>] ``the application is running`` () =
    initEnv ()

let [<Given>] ``a member "(.*)" exists with age group "(.*)" and category "(.*)"`` (name: string) (ageGroupStr: string) (categoryStr: string) =
    let ageGroup = Domain.parseAgeGroup ageGroupStr |> Option.defaultValue Men
    let category = Domain.parseCategory categoryStr |> Option.defaultValue Member
    addMemberDirect name ageGroup category None |> ignore

let [<Given>] ``a member "(.*)" exists with age group "(.*)" category "(.*)" and first attended date of today`` (name: string) (ageGroupStr: string) (categoryStr: string) =
    let ageGroup = Domain.parseAgeGroup ageGroupStr |> Option.defaultValue Men
    let category = Domain.parseCategory categoryStr |> Option.defaultValue Member
    addMemberDirect name ageGroup category (Some DateTime.Today) |> ignore

let [<Given>] ``the member "(.*)" is deactivated`` (name: string) =
    match findMemberByName name with
    | Some m ->
        Database.deactivateMember m.Id
    | None -> failwith $"Member '{name}' not found"

let [<When>] ``I add a member with name "(.*)" age group "(.*)" and category "(.*)"`` (name: string) (ageGroupStr: string) (categoryStr: string) =
    let ageGroup = Domain.parseAgeGroup ageGroupStr |> Option.defaultValue Men
    let category = Domain.parseCategory categoryStr |> Option.defaultValue Member
    addMemberDirect name ageGroup category None |> ignore

let [<When>] ``I add a member with name "(.*)" age group "(.*)" category "(.*)" and first attended date "(.*)"`` (name: string) (ageGroupStr: string) (categoryStr: string) (dateStr: string) =
    let ageGroup = Domain.parseAgeGroup ageGroupStr |> Option.defaultValue Men
    let category = Domain.parseCategory categoryStr |> Option.defaultValue Member
    let firstAttended =
        match DateTime.TryParse(dateStr) with
        | true, d -> Some d
        | _ -> None
    addMemberDirect name ageGroup category firstAttended |> ignore

let [<When>] ``I update the member "(.*)" category to "(.*)"`` (name: string) (newCategoryStr: string) =
    match findMemberByName name with
    | Some m ->
        let newCategory = Domain.parseCategory newCategoryStr |> Option.defaultValue m.Category
        let updated = { m with Category = newCategory }
        Database.updateMember updated
    | None -> failwith $"Member '{name}' not found"

let [<When>] ``I deactivate the member "(.*)"`` (name: string) =
    match findMemberByName name with
    | Some m -> Database.deactivateMember m.Id
    | None -> failwith $"Member '{name}' not found"

let [<Then>] ``the member "(.*)" should exist in the system`` (name: string) =
    let m = findMemberByName name
    Assert.True(m.IsSome, $"Member '{name}' should exist")

let [<Then>] ``the member should be active`` () =
    let allMembers = Database.getMembers ()
    let last = allMembers |> List.last
    Assert.True(last.IsActive, "The member should be active")

let [<Then>] ``the member "(.*)" should have first attended date "(.*)"`` (name: string) (dateStr: string) =
    match findMemberByName name with
    | Some m ->
        Assert.True(m.FirstAttendedDate.IsSome, $"Member '{name}' should have a first attended date")
        let expected = DateTime.Parse(dateStr).Date
        Assert.Equal(expected, m.FirstAttendedDate.Value.Date)
    | None -> failwith $"Member '{name}' not found"

let [<Then>] ``the member "(.*)" should have category "(.*)"`` (name: string) (categoryStr: string) =
    match findMemberByName name with
    | Some m ->
        let expected = Domain.parseCategory categoryStr |> Option.defaultValue Member
        Assert.Equal(expected, m.Category)
    | None -> failwith $"Member '{name}' not found"

let [<Then>] ``the member "(.*)" should be inactive`` (name: string) =
    match findMemberByName name with
    | Some m -> Assert.False(m.IsActive, $"Member '{name}' should be inactive")
    | None -> failwith $"Member '{name}' not found"

let [<Then>] ``the member "(.*)" should not appear in attendance lists`` (name: string) =
    let activeMembers = Database.getMembers () |> List.filter (fun m -> m.IsActive)
    let found = activeMembers |> List.exists (fun m -> m.FullName = name)
    Assert.False(found, $"Member '{name}' should not appear in active member lists")

let [<Then>] ``the member "(.*)" should have age group "(.*)"`` (name: string) (ageGroupStr: string) =
    match findMemberByName name with
    | Some m ->
        let expected = Domain.parseAgeGroup ageGroupStr |> Option.defaultValue Men
        Assert.Equal(expected, m.AgeGroup)
    | None -> failwith $"Member '{name}' not found"
