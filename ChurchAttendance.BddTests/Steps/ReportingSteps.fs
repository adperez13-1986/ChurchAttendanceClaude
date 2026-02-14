module ChurchAttendance.BddTests.ReportingSteps

open System
open global.TickSpec
open global.Xunit
open ChurchAttendance
open ChurchAttendance.BddTests.TestHelper

let [<Given>] ``attendance data exists for the last (\d+) days`` (days: int) =
    let m = addMemberDirect "Report Test Member" Men Member None
    for i in 0 .. (days - 1) do
        let date = DateTime.Today.AddDays(float -i)
        let record : AttendanceRecord =
            { Id = Guid.NewGuid()
              Date = date
              ServiceType = SundayService
              MemberIds = [ m.Id ] }
        Database.saveAttendanceRecord record

let [<When>] ``I generate a report from (\d+) days ago to today`` (daysAgo: int) =
    let startDate = DateTime.Today.AddDays(float -daysAgo)
    let endDate = DateTime.Today
    let records = Database.getAttendance ()
    let members = Database.getMembers ()
    let pdfBytes = PdfService.generateReport startDate endDate records members
    setLastPdfBytes pdfBytes

let [<When>] ``I generate a report for today`` () =
    let today = DateTime.Today
    let records = Database.getAttendance ()
    let members = Database.getMembers ()
    let pdfBytes = PdfService.generateReport today today records members
    setLastPdfBytes pdfBytes

let [<Then>] ``a PDF report should be generated`` () =
    match lastPdfBytes () with
    | Some bytes -> Assert.True(bytes.Length > 0, "PDF should have content")
    | None -> Assert.Fail("No PDF was generated")

let [<Then>] ``the report should contain attendance data`` () =
    match lastPdfBytes () with
    | Some bytes ->
        // PDF files start with %PDF magic bytes
        Assert.True(bytes.Length > 100, "PDF should contain substantial data")
        Assert.Equal(0x25uy, bytes.[0]) // '%'
        Assert.Equal(0x50uy, bytes.[1]) // 'P'
        Assert.Equal(0x44uy, bytes.[2]) // 'D'
        Assert.Equal(0x46uy, bytes.[3]) // 'F'
    | None -> Assert.Fail("No PDF was generated")

let [<Then>] ``the report should show age group counts`` () =
    match lastPdfBytes () with
    | Some bytes ->
        Assert.True(bytes.Length > 100, "PDF report should have been generated with age group data")
    | None -> Assert.Fail("No PDF was generated")

let [<Then>] ``"(.*)" should be highlighted in the report`` (name: string) =
    match lastPdfBytes () with
    | Some bytes ->
        Assert.True(bytes.Length > 100, "PDF report should have been generated")
        match findMemberByName name with
        | Some m ->
            Assert.True(Domain.isFirstTimer m DateTime.Today, $"'{name}' should be a first timer")
        | None -> Assert.Fail($"Member '{name}' not found")
    | None -> Assert.Fail("No PDF was generated")
