open System
open System.IO
open System.Security.Claims
open System.Threading.Tasks
open Microsoft.AspNetCore.Authentication
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Routing
open Microsoft.AspNetCore.Server.Kestrel.Core
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open ChurchAttendance

[<EntryPoint>]
let main args =
    Database.ensureDataDir ()

    // Read password from env var with fallback
    let appPassword =
        match Environment.GetEnvironmentVariable("APP_PASSWORD") with
        | null | "" ->
            printfn "WARNING: APP_PASSWORD not set — using default password. Set APP_PASSWORD environment variable for production."
            "changeme"
        | pwd -> pwd

    let builder = WebApplication.CreateBuilder(args)

    // Configure cookie authentication
    builder.Services
        .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(fun options ->
            options.LoginPath <- "/login"
            options.LogoutPath <- "/logout"
            options.ExpireTimeSpan <- TimeSpan.FromDays(7)
            options.SlidingExpiration <- true
            options.Cookie.HttpOnly <- true
            options.Cookie.SameSite <- SameSiteMode.Strict)
    |> ignore

    builder.Services.AddAuthorization() |> ignore

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

    // Middleware order: static files (no auth) → authentication → authorization
    app.UseStaticFiles() |> ignore
    app.UseAuthentication() |> ignore
    app.UseAuthorization() |> ignore

    let mapGet (pattern: string) (handler: HttpContext -> Task) =
        app.MapGet(pattern, RequestDelegate(handler))
            .RequireAuthorization()
        |> ignore

    let mapPost (pattern: string) (handler: HttpContext -> Task) =
        app.MapPost(pattern, RequestDelegate(handler))
            .RequireAuthorization()
        |> ignore

    let mapPut (pattern: string) (handler: HttpContext -> Task) =
        app.MapPut(pattern, RequestDelegate(handler))
            .RequireAuthorization()
        |> ignore

    let mapDelete (pattern: string) (handler: HttpContext -> Task) =
        app.MapDelete(pattern, RequestDelegate(handler))
            .RequireAuthorization()
        |> ignore

    // Login (no auth required)
    app.MapGet("/login", RequestDelegate(Handlers.loginPage))
        .AllowAnonymous() |> ignore
    app.MapPost("/login", RequestDelegate(fun ctx -> Handlers.loginPost ctx appPassword))
        .AllowAnonymous() |> ignore

    // Logout
    mapPost "/logout" Handlers.logoutPost

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

    mapPost "/attendance/toggle" (fun ctx -> Handlers.toggleAttendance ctx)
    mapPost "/attendance" (fun ctx -> Handlers.saveAttendance ctx)
    mapPost "/attendance/export-pdf" (fun ctx -> Handlers.exportAttendancePdf ctx)

    // Reports
    mapGet "/reports" Handlers.reportsPage

    mapPost "/reports/export" (fun ctx -> Handlers.exportPdf ctx)

    app.Run()
    0
