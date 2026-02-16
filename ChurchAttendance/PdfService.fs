namespace ChurchAttendance

open System
open QuestPDF.Fluent
open QuestPDF.Helpers
open QuestPDF.Infrastructure

module PdfService =

    type AttendanceSummary =
        { Date: DateTime
          ServiceType: string
          Attendees: Member list
          AgeGroupCounts: (string * int) list
          CategoryCounts: (string * int) list
          FirstTimers: string list
          Total: int }

    let private buildSummaries
        (records: AttendanceRecord list)
        (allRecords: AttendanceRecord list)
        (members: Member list)
        : AttendanceSummary list =
        let memberMap =
            members |> List.map (fun m -> m.Id, m) |> Map.ofList

        let sortedRecords = records |> List.sortBy (fun r -> r.Date)

        // For each record, compute who is a first timer based on FirstAttendedDate field
        sortedRecords
        |> List.map (fun record ->
            let attendees =
                record.MemberIds
                |> List.choose (fun id -> Map.tryFind id memberMap)

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

            let firstTimers =
                attendees
                |> List.filter (fun m -> Domain.isFirstTimer m record.Date)
                |> List.map (fun m -> m.FullName)

            { Date = record.Date
              ServiceType = Domain.serviceTypeLabel record.ServiceType
              Attendees = attendees
              AgeGroupCounts = ageGroupCounts
              CategoryCounts = categoryCounts
              FirstTimers = firstTimers
              Total = attendees.Length })

    let private composeSummaryTable
        (table: TableDescriptor)
        (headerLabel: string)
        (items: (string * int) list)
        =
        table.ColumnsDefinition(fun cols ->
            cols.RelativeColumn()
            cols.ConstantColumn(50f))

        table.Header(fun header ->
            header.Cell().BorderBottom(1f).Padding(3f).Text(headerLabel).Bold()
            |> ignore

            header
                .Cell()
                .BorderBottom(1f)
                .Padding(3f)
                .AlignRight()
                .Text("Count")
                .Bold()
            |> ignore)

        for (label, count) in items do
            table.Cell().Padding(2f).Text(label) |> ignore

            table.Cell().Padding(2f).AlignRight().Text(string count)
            |> ignore

    let generateReport
        (startDate: DateTime)
        (endDate: DateTime)
        (records: AttendanceRecord list)
        (members: Member list)
        : byte array =

        QuestPDF.Settings.License <- LicenseType.Community

        let filteredRecords =
            records
            |> List.filter (fun r -> r.Date.Date >= startDate.Date && r.Date.Date <= endDate.Date)

        let summaries = buildSummaries filteredRecords records members

        let startLabel = startDate.ToString("MMM dd, yyyy")
        let endLabel = endDate.ToString("MMM dd, yyyy")

        Document
            .Create(fun (container: IDocumentContainer) ->
                container.Page(fun page ->
                    page.Size(PageSizes.A4)
                    page.Margin(1.5f, Unit.Centimetre)
                    page.DefaultTextStyle(fun x -> x.FontSize(10f))

                    page
                        .Header()
                        .Column(fun col ->
                            col
                                .Item()
                                .Text("Church Attendance Report")
                                .Bold()
                                .FontSize(18f)
                            |> ignore

                            col
                                .Item()
                                .Text($"{startLabel} - {endLabel}")
                                .FontSize(12f)
                            |> ignore

                            col.Item().PaddingBottom(10f).LineHorizontal(1f) |> ignore)

                    page
                        .Content()
                        .Column(fun content ->
                            content.Spacing(15f)

                            if summaries.IsEmpty then
                                content.Item().Text("No attendance records found for this period.")
                                |> ignore
                            else
                                for summary in summaries do
                                    let svcLabel =
                                        $"""{summary.ServiceType} - {summary.Date.ToString("MMM dd, yyyy")}"""

                                    content
                                        .Item()
                                        .Column(fun section ->
                                            section.Spacing(8f)

                                            section.Item().Text(svcLabel).Bold().FontSize(14f)
                                            |> ignore

                                            section.Item().Text($"Total Present: {summary.Total}")
                                            |> ignore

                                            // Attendees grouped by age group
                                            let firstTimerNames = summary.FirstTimers |> Set.ofList

                                            for ag in Domain.allAgeGroups do
                                                let groupMembers =
                                                    summary.Attendees
                                                    |> List.filter (fun m -> m.AgeGroup = ag)
                                                    |> List.sortBy (fun m -> m.FullName)

                                                if not groupMembers.IsEmpty then
                                                    let groupLabel = Domain.ageGroupLabel ag

                                                    section
                                                        .Item()
                                                        .PaddingTop(6f)
                                                        .Text($"{groupLabel} ({groupMembers.Length})")
                                                        .Bold()
                                                        .FontSize(11f)
                                                    |> ignore

                                                    section
                                                        .Item()
                                                        .Table(fun table ->
                                                            table.ColumnsDefinition(fun cols ->
                                                                cols.ConstantColumn(30f)
                                                                cols.RelativeColumn(3f)
                                                                cols.RelativeColumn(1.5f))

                                                            table.Header(fun header ->
                                                                header
                                                                    .Cell()
                                                                    .BorderBottom(1f)
                                                                    .Padding(4f)
                                                                    .Text("#")
                                                                    .Bold()
                                                                |> ignore

                                                                header
                                                                    .Cell()
                                                                    .BorderBottom(1f)
                                                                    .Padding(4f)
                                                                    .Text("Name")
                                                                    .Bold()
                                                                |> ignore

                                                                header
                                                                    .Cell()
                                                                    .BorderBottom(1f)
                                                                    .Padding(4f)
                                                                    .Text("Category")
                                                                    .Bold()
                                                                |> ignore)

                                                            groupMembers
                                                            |> List.iteri (fun i m ->
                                                                let isFirstTimer = firstTimerNames.Contains m.FullName

                                                                let bgColor =
                                                                    if isFirstTimer then
                                                                        Colors.Yellow.Lighten4
                                                                    else
                                                                        Colors.White

                                                                table
                                                                    .Cell()
                                                                    .Background(bgColor)
                                                                    .Padding(3f)
                                                                    .Text(string (i + 1))
                                                                |> ignore

                                                                let nameText =
                                                                    if isFirstTimer then
                                                                        m.FullName + " *"
                                                                    else
                                                                        m.FullName

                                                                table
                                                                    .Cell()
                                                                    .Background(bgColor)
                                                                    .Padding(3f)
                                                                    .Text(nameText)
                                                                |> ignore

                                                                table
                                                                    .Cell()
                                                                    .Background(bgColor)
                                                                    .Padding(3f)
                                                                    .Text(Domain.categoryLabel m.Category)
                                                                |> ignore))
                                            |> ignore

                                            // Age group + category summary side by side
                                            section
                                                .Item()
                                                .Row(fun row ->
                                                    row
                                                        .RelativeItem()
                                                        .Table(fun table ->
                                                            composeSummaryTable
                                                                table
                                                                "Age Group"
                                                                summary.AgeGroupCounts)
                                                    |> ignore

                                                    row.ConstantItem(30f) |> ignore

                                                    row
                                                        .RelativeItem()
                                                        .Table(fun table ->
                                                            composeSummaryTable
                                                                table
                                                                "Category"
                                                                summary.CategoryCounts)
                                                    |> ignore)
                                            |> ignore

                                            // First timers
                                            if not summary.FirstTimers.IsEmpty then
                                                section
                                                    .Item()
                                                    .Column(fun ftCol ->
                                                        ftCol
                                                            .Item()
                                                            .Text("* First Timers:")
                                                            .Bold()
                                                            .FontSize(9f)
                                                        |> ignore

                                                        for name in summary.FirstTimers do
                                                            ftCol
                                                                .Item()
                                                                .Text($"  - {name}")
                                                                .FontSize(9f)
                                                            |> ignore)
                                                |> ignore

                                            section.Item().PaddingTop(5f).LineHorizontal(0.5f)
                                            |> ignore)
                                    |> ignore)

                    page
                        .Footer()
                        .AlignCenter()
                        .Text(fun text ->
                            text.Span("Page ") |> ignore
                            text.CurrentPageNumber() |> ignore
                            text.Span(" of ") |> ignore
                            text.TotalPages() |> ignore))
                |> ignore)
            .GeneratePdf()
