namespace ChurchAttendance

open System
open Microsoft.AspNetCore.Http

module Handlers =

    let private isHtmx (ctx: HttpContext) =
        ctx.Request.Headers.ContainsKey("HX-Request")

    let private formValue (ctx: HttpContext) (key: string) =
        match ctx.Request.Form.TryGetValue(key) with
        | true, values when values.Count > 0 -> Some(values.[0])
        | _ -> None

    let private formValues (ctx: HttpContext) (key: string) =
        match ctx.Request.Form.TryGetValue(key) with
        | true, values -> values |> Seq.toList
        | _ -> []

    // GET /
    let dashboard (ctx: HttpContext) =
        let members = Database.getMembers ()
        let activeCount = members |> List.filter (fun m -> m.IsActive) |> List.length
        let today = DateTime.Today

        let todayAttendance =
            Database.getAttendance ()
            |> List.filter (fun r -> r.Date.Date = today)
            |> List.collect (fun r -> r.MemberIds)
            |> List.distinct
            |> List.length

        let html = Templates.homePage members.Length activeCount todayAttendance
        ctx.Response.ContentType <- "text/html; charset=utf-8"
        ctx.Response.WriteAsync(html)

    // GET /members
    let membersPage (ctx: HttpContext) =
        let members = Database.getMembers ()
        let html = Templates.membersPage members
        ctx.Response.ContentType <- "text/html; charset=utf-8"
        ctx.Response.WriteAsync(html)

    // GET /members/new
    let newMemberForm (ctx: HttpContext) =
        let html = Templates.memberForm None
        ctx.Response.ContentType <- "text/html; charset=utf-8"
        ctx.Response.WriteAsync(html)

    // POST /members
    let createMember (ctx: HttpContext) =
        task {
            let! _ = ctx.Request.ReadFormAsync()
            let name = formValue ctx "fullName" |> Option.defaultValue ""
            let ageGroupStr = formValue ctx "ageGroup" |> Option.defaultValue "Men"
            let categoryStr = formValue ctx "category" |> Option.defaultValue "Member"
            let firstAttendedDateStr = formValue ctx "firstAttendedDate"

            let ageGroup =
                Domain.parseAgeGroup ageGroupStr |> Option.defaultValue Men

            let category =
                Domain.parseCategory categoryStr |> Option.defaultValue Member

            let firstAttendedDate =
                firstAttendedDateStr
                |> Option.bind (fun s ->
                    if String.IsNullOrWhiteSpace(s) then
                        None
                    else
                        match DateTime.TryParse(s) with
                        | true, date -> Some date
                        | _ -> None)

            let m = Domain.newMember name ageGroup category firstAttendedDate
            Database.addMember m

            if isHtmx ctx then
                let members = Database.getMembers()
                let tableHtml = Templates.membersTable members
                let closeScript = "<script>document.getElementById('member-modal').close()</script>"
                let html = tableHtml + closeScript

                ctx.Response.ContentType <- "text/html; charset=utf-8"
                return! ctx.Response.WriteAsync(html)
            else
                ctx.Response.Redirect("/members")
                return ()
        }

    // GET /members/{id}/edit
    let editMemberForm (ctx: HttpContext) (id: string) =
        match Guid.TryParse(id) with
        | true, guid ->
            let m = Database.getMember guid
            let html = Templates.memberForm m
            ctx.Response.ContentType <- "text/html; charset=utf-8"
            ctx.Response.WriteAsync(html)
        | _ ->
            ctx.Response.ContentType <- "text/html; charset=utf-8"
            ctx.Response.WriteAsync(Templates.statusMessage "Invalid member ID" true)

    // PUT /members/{id}
    let updateMember (ctx: HttpContext) (id: string) =
        task {
            let! _ = ctx.Request.ReadFormAsync()

            match Guid.TryParse(id) with
            | true, guid ->
                match Database.getMember guid with
                | Some existing ->
                    let name = formValue ctx "fullName" |> Option.defaultValue existing.FullName

                    let ageGroup =
                        formValue ctx "ageGroup"
                        |> Option.bind Domain.parseAgeGroup
                        |> Option.defaultValue existing.AgeGroup

                    let category =
                        formValue ctx "category"
                        |> Option.bind Domain.parseCategory
                        |> Option.defaultValue existing.Category

                    let firstAttendedDateStr = formValue ctx "firstAttendedDate"

                    let firstAttendedDate =
                        firstAttendedDateStr
                        |> Option.bind (fun s ->
                            if String.IsNullOrWhiteSpace(s) then
                                None
                            else
                                match DateTime.TryParse(s) with
                                | true, date -> Some date
                                | _ -> None)

                    let updated =
                        { existing with
                            FullName = name
                            AgeGroup = ageGroup
                            Category = category
                            FirstAttendedDate = firstAttendedDate }

                    Database.updateMember updated

                    let members = Database.getMembers()
                    let tableHtml = Templates.membersTable members
                    let closeScript = "<script>document.getElementById('member-modal').close()</script>"
                    let html = tableHtml + closeScript

                    ctx.Response.ContentType <- "text/html; charset=utf-8"
                    return! ctx.Response.WriteAsync(html)
                | None ->
                    ctx.Response.ContentType <- "text/html; charset=utf-8"
                    return! ctx.Response.WriteAsync(Templates.statusMessage "Member not found" true)
            | _ ->
                ctx.Response.ContentType <- "text/html; charset=utf-8"
                return! ctx.Response.WriteAsync(Templates.statusMessage "Invalid member ID" true)
        }

    // DELETE /members/{id}
    let deactivateMember (ctx: HttpContext) (id: string) =
        match Guid.TryParse(id) with
        | true, guid ->
            Database.deactivateMember guid
            let members = Database.getMembers ()
            let html = Templates.membersTable members
            ctx.Response.ContentType <- "text/html; charset=utf-8"
            ctx.Response.WriteAsync(html)
        | _ ->
            ctx.Response.ContentType <- "text/html; charset=utf-8"
            ctx.Response.WriteAsync(Templates.statusMessage "Invalid member ID" true)

    // GET /attendance
    let attendancePage (ctx: HttpContext) =
        let dateParam =
            if ctx.Request.Query.ContainsKey("date") then
                Some (ctx.Request.Query.["date"].ToString())
            else
                None
        let html = Templates.attendancePage dateParam
        ctx.Response.ContentType <- "text/html; charset=utf-8"
        ctx.Response.WriteAsync(html)

    let private parseServiceType (s: string) =
        Domain.parseServiceType s |> Option.defaultValue SundayService

    let private inferServiceType (date: DateTime) =
        if date.DayOfWeek = DayOfWeek.Friday then PrayerMeeting
        else SundayService

    // GET /attendance/list
    let attendanceList (ctx: HttpContext) =
        let dateStr =
            ctx.Request.Query.["date"].ToString()

        let serviceTypeStr =
            if ctx.Request.Query.ContainsKey("serviceType") then
                ctx.Request.Query.["serviceType"].ToString()
            else
                ""

        let serviceType =
            match DateTime.TryParse(dateStr) with
            | true, date when System.String.IsNullOrEmpty(serviceTypeStr) -> inferServiceType date
            | _ -> parseServiceType serviceTypeStr
        let members = Database.getMembers ()

        let checkedIds =
            match DateTime.TryParse(dateStr) with
            | true, date ->
                match Database.getAttendanceForDate date serviceType with
                | Some record -> record.MemberIds |> Set.ofList
                | None -> Set.empty
            | _ -> Set.empty

        let isToday =
            match DateTime.TryParse(dateStr) with
            | true, date -> date.Date = DateTime.Today
            | _ -> false

        let serviceTypeStr' =
            match serviceType with
            | SundayService -> "SundayService"
            | PrayerMeeting -> "PrayerMeeting"

        let serviceLabel = Domain.serviceTypeLabel serviceType

        let firstTimerIds =
            match DateTime.TryParse(dateStr) with
            | true, date -> Database.getFirstTimerIds date
            | _ -> Set.empty

        let html =
            Templates.attendanceChecklist members dateStr serviceTypeStr' serviceLabel isToday checkedIds firstTimerIds

        ctx.Response.ContentType <- "text/html; charset=utf-8"
        ctx.Response.WriteAsync(html)

    // POST /attendance/auto-save
    let autoSaveAttendance (ctx: HttpContext) =
        task {
            let! _ = ctx.Request.ReadFormAsync()
            let dateStr = formValue ctx "date" |> Option.defaultValue ""
            let serviceTypeStr = formValue ctx "serviceType" |> Option.defaultValue "SundayService"
            let serviceType = parseServiceType serviceTypeStr
            let memberIdStrs = formValues ctx "memberIds"

            let memberIds =
                memberIdStrs
                |> List.choose (fun s ->
                    match Guid.TryParse(s) with
                    | true, g -> Some g
                    | _ -> None)

            match DateTime.TryParse(dateStr) with
            | true, date ->
                let record =
                    { Id = Guid.NewGuid()
                      Date = date
                      ServiceType = serviceType
                      MemberIds = memberIds }

                Database.saveAttendanceRecord record
                ctx.Response.ContentType <- "text/html; charset=utf-8"
                return! ctx.Response.WriteAsync($"""<span class="status-msg success" style="padding:0.3rem 0.6rem;font-size:0.85rem">Saved ({memberIds.Length} present)</span>""")
            | _ ->
                ctx.Response.ContentType <- "text/html; charset=utf-8"
                return! ctx.Response.WriteAsync("")
        }

    // POST /attendance
    let saveAttendance (ctx: HttpContext) =
        task {
            let! _ = ctx.Request.ReadFormAsync()
            let dateStr = formValue ctx "date" |> Option.defaultValue ""
            let serviceTypeStr = formValue ctx "serviceType" |> Option.defaultValue "SundayService"
            let serviceType = parseServiceType serviceTypeStr
            let memberIdStrs = formValues ctx "memberIds"

            let memberIds =
                memberIdStrs
                |> List.choose (fun s ->
                    match Guid.TryParse(s) with
                    | true, g -> Some g
                    | _ -> None)

            match DateTime.TryParse(dateStr) with
            | true, date ->
                let record =
                    { Id = Guid.NewGuid()
                      Date = date
                      ServiceType = serviceType
                      MemberIds = memberIds }

                Database.saveAttendanceRecord record

                let members = Database.getMembers ()
                let memberMap = members |> List.map (fun m -> m.Id, m) |> Map.ofList

                let attendees =
                    memberIds |> List.choose (fun id -> Map.tryFind id memberMap)

                let ageGroupCounts =
                    Domain.allAgeGroups
                    |> List.map (fun ag ->
                        Domain.ageGroupLabel ag,
                        attendees
                        |> List.filter (fun m -> m.AgeGroup = ag)
                        |> List.length)

                let categoryCounts =
                    Domain.allCategories
                    |> List.map (fun c ->
                        Domain.categoryLabel c,
                        attendees
                        |> List.filter (fun m -> m.Category = c)
                        |> List.length)

                let firstTimerIds = Database.getFirstTimerIds date

                let firstTimers =
                    attendees
                    |> List.filter (fun m -> firstTimerIds.Contains m.Id)
                    |> List.map (fun m -> m.FullName)

                let serviceLabel = Domain.serviceTypeLabel serviceType

                let html =
                    Templates.attendanceSummary dateStr serviceLabel attendees.Length ageGroupCounts categoryCounts firstTimers

                ctx.Response.ContentType <- "text/html; charset=utf-8"
                return! ctx.Response.WriteAsync(html)
            | _ ->
                ctx.Response.ContentType <- "text/html; charset=utf-8"
                return! ctx.Response.WriteAsync(Templates.statusMessage "Invalid date" true)
        }

    // GET /reports
    let reportsPage (ctx: HttpContext) =
        let html = Templates.reportsPage ()
        ctx.Response.ContentType <- "text/html; charset=utf-8"
        ctx.Response.WriteAsync(html)

    // POST /attendance/export-pdf
    let exportAttendancePdf (ctx: HttpContext) =
        task {
            let! _ = ctx.Request.ReadFormAsync()
            let dateStr = formValue ctx "date" |> Option.defaultValue ""
            let serviceTypeStr = formValue ctx "serviceType" |> Option.defaultValue "SundayService"

            match DateTime.TryParse(dateStr) with
            | true, date ->
                let records = Database.getAttendance ()
                let members = Database.getMembers ()
                // Generate PDF for single date (start and end are the same)
                let pdfBytes = PdfService.generateReport date date records members

                let serviceType = parseServiceType serviceTypeStr
                let serviceLabel = Domain.serviceTypeLabel serviceType

                let fileName =
                    $"""Church-Attendance-{serviceLabel.Replace(" ", "-")}-{date.ToString("yyyy-MM-dd")}.pdf"""

                ctx.Response.ContentType <- "application/pdf"
                ctx.Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"")
                return! ctx.Response.Body.WriteAsync(pdfBytes, 0, pdfBytes.Length)
            | _ ->
                ctx.Response.ContentType <- "text/html; charset=utf-8"
                return! ctx.Response.WriteAsync(Templates.statusMessage "Invalid date" true)
        }

    // POST /reports/export
    let exportPdf (ctx: HttpContext) =
        task {
            let! _ = ctx.Request.ReadFormAsync()
            let startStr = formValue ctx "startDate" |> Option.defaultValue ""
            let endStr = formValue ctx "endDate" |> Option.defaultValue ""

            match DateTime.TryParse(startStr), DateTime.TryParse(endStr) with
            | (true, startDate), (true, endDate) ->
                let records = Database.getAttendance ()
                let members = Database.getMembers ()
                let pdfBytes = PdfService.generateReport startDate endDate records members

                let fileName =
                    $"""Church-Attendance-Report-{startDate.ToString("yyyy-MM-dd")}-to-{endDate.ToString("yyyy-MM-dd")}.pdf"""

                ctx.Response.ContentType <- "application/pdf"
                ctx.Response.Headers.Append("Content-Disposition", $"attachment; filename=\"{fileName}\"")
                return! ctx.Response.Body.WriteAsync(pdfBytes, 0, pdfBytes.Length)
            | _ ->
                ctx.Response.ContentType <- "text/html; charset=utf-8"
                return! ctx.Response.WriteAsync(Templates.statusMessage "Invalid date range" true)
        }

