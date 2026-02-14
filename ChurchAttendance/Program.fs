open System
open System.IO
open System.Threading.Tasks
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Routing
open Microsoft.AspNetCore.Server.Kestrel.Core
open Microsoft.Extensions.Hosting
open ChurchAttendance

[<EntryPoint>]
let main args =
    Database.ensureDataDir ()

    let builder = WebApplication.CreateBuilder(args)

    // Configure Kestrel for HTTP (and HTTPS if cert exists)
    let certPath = Path.Combine(AppContext.BaseDirectory, "certs", "certificate.pfx")
    builder.WebHost.ConfigureKestrel(fun options ->
        options.ListenAnyIP(5050, fun listenOptions ->
            listenOptions.Protocols <- HttpProtocols.Http1AndHttp2)

        if File.Exists(certPath) then
            options.ListenAnyIP(5051, fun listenOptions ->
                listenOptions.Protocols <- HttpProtocols.Http1AndHttp2
                listenOptions.UseHttps(certPath) |> ignore)
    ) |> ignore

    let app = builder.Build()

    app.UseStaticFiles() |> ignore

    let mapGet (pattern: string) (handler: HttpContext -> Task) =
        app.MapGet(pattern, RequestDelegate(handler)) |> ignore

    let mapPost (pattern: string) (handler: HttpContext -> Task) =
        app.MapPost(pattern, RequestDelegate(handler)) |> ignore

    let mapPut (pattern: string) (handler: HttpContext -> Task) =
        app.MapPut(pattern, RequestDelegate(handler)) |> ignore

    let mapDelete (pattern: string) (handler: HttpContext -> Task) =
        app.MapDelete(pattern, RequestDelegate(handler)) |> ignore

    // Dashboard
    mapGet "/" Handlers.dashboard

    // Members
    mapGet "/members" Handlers.membersPage
    mapGet "/members/new" Handlers.newMemberForm

    mapPost "/members" (fun ctx -> Handlers.createMember ctx)

    mapGet "/members/{id}/edit" (fun ctx ->
        let id = ctx.GetRouteValue("id") :?> string
        Handlers.editMemberForm ctx id)

    mapPut "/members/{id}" (fun ctx ->
        let id = ctx.GetRouteValue("id") :?> string
        Handlers.updateMember ctx id)

    mapDelete "/members/{id}" (fun ctx ->
        let id = ctx.GetRouteValue("id") :?> string
        Handlers.deactivateMember ctx id)

    // Attendance
    mapGet "/attendance" Handlers.attendancePage
    mapGet "/attendance/list" Handlers.attendanceList

    mapPost "/attendance/auto-save" (fun ctx -> Handlers.autoSaveAttendance ctx)
    mapPost "/attendance" (fun ctx -> Handlers.saveAttendance ctx)
    mapPost "/attendance/export-pdf" (fun ctx -> Handlers.exportAttendancePdf ctx)

    // Reports
    mapGet "/reports" Handlers.reportsPage

    mapPost "/reports/export" (fun ctx -> Handlers.exportPdf ctx)

    app.Run()
    0
