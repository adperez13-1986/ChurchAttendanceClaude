namespace ChurchAttendance

open System

module Templates =

    let private htmlEncode (s: string) =
        System.Net.WebUtility.HtmlEncode(s)

    let private themeScript =
        """<script>document.documentElement.setAttribute('data-theme',localStorage.getItem('theme')||'light')</script>"""

    let loginPage (churchName: string) (error: string option) =
        let errorHtml =
            match error with
            | Some msg -> $"""<p class="status-msg error" style="text-align:center">{htmlEncode msg}</p>"""
            | None -> ""

        $"""<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Login - {htmlEncode churchName}</title>
    {themeScript}
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@picocss/pico@2/css/pico.min.css">
    <link rel="stylesheet" href="/css/app.css?v=16">
</head>
<body>
    <div style="position:absolute;top:1rem;right:1rem">
        <a href="#" class="theme-toggle-text" id="theme-toggle" aria-label="Toggle dark mode">Dark Mode</a>
    </div>
    <main class="container" style="max-width:400px;margin-top:10vh">
        <article>
            <header><h3 style="text-align:center">{htmlEncode churchName}</h3></header>
            {errorHtml}
            <form method="post" action="/login">
                <label for="password">Password
                    <input type="password" id="password" name="password" required autofocus>
                </label>
                <button type="submit">Login</button>
            </form>
        </article>
    </main>
    <script src="/js/app.js?v=15"></script>
</body>
</html>"""

    let layout (churchName: string) (title: string) (activeNav: string) (content: string) =
        let tabClass tab = if activeNav = tab then "tab-active" else ""
        $"""<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>{htmlEncode title} - {htmlEncode churchName}</title>
    {themeScript}
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@picocss/pico@2/css/pico.min.css">
    <link rel="stylesheet" href="/css/app.css?v=16">
    <script src="https://unpkg.com/htmx.org@2.0.4"></script>
</head>
<body>
    <nav class="container">
        <ul>
            <li><strong>{htmlEncode churchName}</strong></li>
        </ul>
        <ul>
            <li class="nav-link"><a href="/" class="{if activeNav = "home" then "active" else ""}">Dashboard</a></li>
            <li class="nav-link"><a href="/members" class="{if activeNav = "members" then "active" else ""}">Members</a></li>
            <li class="nav-link"><a href="/attendance" class="{if activeNav = "attendance" then "active" else ""}">Attendance</a></li>
            <li class="nav-link"><a href="/reports" class="{if activeNav = "reports" then "active" else ""}">Reports</a></li>
            <li class="nav-link"><a href="#" id="desktop-more">More</a></li>
        </ul>
    </nav>
    <main class="container">
        {content}
    </main>
    <div id="more-menu" class="more-menu" style="display:none">
        <div class="more-menu-item" id="more-theme-toggle">
            <span class="more-menu-icon" id="more-theme-icon">&#9789;</span>
            <span id="more-theme-label">Dark Mode</span>
        </div>
        <a href="/logout" class="more-menu-item" data-logout="true">
            <span class="more-menu-icon">&#10132;</span>
            <span>Logout</span>
        </a>
    </div>
    <nav class="bottom-tab-bar">
        <a href="/" class="{tabClass "home"}"><span class="tab-icon">&#8962;</span><span class="tab-label">Dashboard</span></a>
        <a href="/members" class="{tabClass "members"}"><span class="tab-icon">&#128101;</span><span class="tab-label">Members</span></a>
        <a href="/attendance" class="{tabClass "attendance"}"><span class="tab-icon">&#10003;</span><span class="tab-label">Attendance</span></a>
        <a href="/reports" class="{tabClass "reports"}"><span class="tab-icon">&#128202;</span><span class="tab-label">Reports</span></a>
        <a href="#" id="more-tab" class="{tabClass "more"}"><span class="tab-icon">&#8942;</span><span class="tab-label">More</span></a>
    </nav>
    <script src="/js/app.js?v=15"></script>
</body>
</html>"""

    let homePage
        (churchName: string)
        (memberCount: int)
        (activeCount: int)
        (todayAttendance: int)
        (lastService: (int * DateTime * string) option)
        (recentActivity: (DateTime * string * int) list)
        =
        let lastServiceCard =
            match lastService with
            | Some (count, date, label) ->
                let dateStr = date.ToString("MMM d")
                $"""<article class="stat-purple">
        <header>Last Service</header>
        <p class="stat">{count}</p>
        <small class="stat-label">Last Service</small>
        <small>{htmlEncode label} &middot; {htmlEncode dateStr}</small>
    </article>"""
            | None ->
                """<article class="stat-purple">
        <header>Last Service</header>
        <p class="stat">&mdash;</p>
        <small class="stat-label">Last Service</small>
        <small>No services yet</small>
    </article>"""

        let recentRows =
            if recentActivity.IsEmpty then
                """<p style="color:var(--pico-muted-color)">No attendance records yet</p>"""
            else
                let rows =
                    recentActivity
                    |> List.map (fun (date, label, count) ->
                        let dayName = date.ToString("ddd")
                        let dateStr = date.ToString("MMM d")
                        $"""<tr><td>{htmlEncode dayName}, {htmlEncode dateStr}</td><td>{htmlEncode label}</td><td><strong>{count}</strong></td></tr>""")
                    |> String.concat "\n"

                $"""<table role="grid">
    <thead><tr><th>Date</th><th>Service</th><th>Present</th></tr></thead>
    <tbody>{rows}</tbody>
</table>"""

        let content =
            $"""<h1>Dashboard</h1>
<div class="stat-grid">
    <article class="stat-blue">
        <header>Total Members</header>
        <p class="stat">{memberCount}</p>
        <small class="stat-label">Total Members</small>
    </article>
    <article class="stat-green">
        <header>Active Members</header>
        <p class="stat">{activeCount}</p>
        <small class="stat-label">Active Members</small>
    </article>
    <article class="stat-amber">
        <header>Today's Attendance</header>
        <p class="stat">{todayAttendance}</p>
        <small class="stat-label">Today's Attendance</small>
    </article>
    {lastServiceCard}
</div>
<div class="grid" style="margin-bottom:1.5rem">
    <a href="/attendance" role="button">Take Today's Attendance</a>
    <a href="/reports" role="button" class="secondary">View Reports</a>
</div>
<article>
    <header><h3 style="margin:0">Recent Activity</h3></header>
    {recentRows}
</article>"""

        layout churchName "Dashboard" "home" content

    let private ageGroupOptions (selected: string) =
        Domain.allAgeGroups
        |> List.map (fun ag ->
            let label = Domain.ageGroupLabel ag
            let sel = if label = selected then " selected" else ""
            $"""<option value="{label}"{sel}>{label}</option>""")
        |> String.concat "\n"

    let private categoryOptions (selected: string) =
        Domain.allCategories
        |> List.map (fun c ->
            let label =
                match c with
                | Member -> "Member"
                | Attendee -> "Attendee"
                | Visitor -> "Visitor"
                | UnderMonitoring -> "UnderMonitoring"

            let display = Domain.categoryLabel c
            let sel = if label = selected then " selected" else ""
            $"""<option value="{label}"{sel}>{display}</option>""")
        |> String.concat "\n"

    let memberForm (m: ChurchAttendance.Member option) =
        let isEdit = m.IsSome

        let title =
            if isEdit then
                "Edit Member"
            else
                "Add New Member"

        let action, method_ =
            match m with
            | Some mem -> $"/members/{mem.Id}", "put"
            | None -> "/members", "post"

        let name = m |> Option.map (fun x -> x.FullName) |> Option.defaultValue ""

        let ageGroup =
            m
            |> Option.map (fun x -> Domain.ageGroupLabel x.AgeGroup)
            |> Option.defaultValue "Men"

        let category =
            m
            |> Option.map (fun x ->
                match x.Category with
                | Member -> "Member"
                | Attendee -> "Attendee"
                | Visitor -> "Visitor"
                | UnderMonitoring -> "UnderMonitoring")
            |> Option.defaultValue "Member"

        let firstAttendedDate =
            m
            |> Option.bind (fun x -> x.FirstAttendedDate)
            |> Option.map (fun d -> d.ToString("yyyy-MM-dd"))
            |> Option.defaultValue ""

        let hxAttr =
            if isEdit then
                $"""hx-put="{action}" """
            else
                $"""hx-post="{action}" """

        $"""<form {hxAttr} hx-target="#members-sections" hx-swap="outerHTML">
    <label for="fullName">Full Name
        <input type="text" id="fullName" name="fullName" value="{htmlEncode name}" required>
    </label>
    <label for="ageGroup">Age Group
        <select id="ageGroup" name="ageGroup" required>
            {ageGroupOptions ageGroup}
        </select>
    </label>
    <label for="category">Category
        <select id="category" name="category" required>
            {categoryOptions category}
        </select>
    </label>
    <label for="firstAttendedDate">First Attended Date (Optional)
        <input type="date" id="firstAttendedDate" name="firstAttendedDate" value="{htmlEncode firstAttendedDate}">
        <small>Leave empty if unknown. Used to identify first timers.</small>
    </label>
    <div class="grid">
        <button type="submit">{if isEdit then "Update" else "Add"} Member</button>
        <button type="button" class="secondary" onclick="closeModal()">Cancel</button>
    </div>
</form>"""

    let statusMessage (msg: string) (isError: bool) =
        let cls = if isError then "error" else "success"
        $"""<p class="status-msg {cls}">{htmlEncode msg}</p>"""

    let private memberRow (m: ChurchAttendance.Member) =
        let inactiveClass = if m.IsActive then "" else " inactive"

        let deactivateBtn =
            if m.IsActive then
                $"""<button class="outline secondary icon-btn" title="Deactivate" hx-delete="/members/{m.Id}" hx-target="#members-sections" hx-swap="outerHTML" hx-confirm="Deactivate {htmlEncode m.FullName}?">&#10005;</button>"""
            else
                ""

        let statusText = if m.IsActive then "Active" else "Inactive"
        let nameAttr = htmlEncode (m.FullName.ToLowerInvariant())

        $"""<tr class="{inactiveClass}" data-name="{nameAttr}">
    <td>{htmlEncode m.FullName}</td>
    <td>{Domain.categoryLabel m.Category}</td>
    <td>{statusText}</td>
    <td><div class="action-btns">
        <button class="outline icon-btn" title="Edit" hx-get="/members/{m.Id}/edit" hx-target="#member-form-area" hx-swap="innerHTML" onclick="openModal();document.getElementById('modal-title').textContent='Edit Member'">&#9998;</button>
        {deactivateBtn}
    </div></td>
</tr>"""

    let private renderMemberSection (label: string) (count: int) (rows: string) =
        $"""<div class="age-group-section collapsed" data-age-group="{label}">
    <div class="age-group-header"><span class="section-toggle">&#9654;</span> {label} ({count})</div>
    <div class="age-group-body">
        <table role="grid">
            <thead>
                <tr>
                    <th>Name</th>
                    <th>Category</th>
                    <th>Status</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                {rows}
            </tbody>
        </table>
    </div>
</div>"""

    let membersTable (members: ChurchAttendance.Member list) =
        let sections =
            Domain.allAgeGroups
            |> List.choose (fun ag ->
                let groupMembers =
                    members
                    |> List.filter (fun m -> m.AgeGroup = ag)
                    |> List.sortBy (fun m -> (not m.IsActive, m.FullName))

                if groupMembers.IsEmpty then
                    None
                else
                    let label = Domain.ageGroupLabel ag
                    let rows = groupMembers |> List.map memberRow |> String.concat "\n"
                    Some (renderMemberSection label groupMembers.Length rows))
            |> String.concat "\n"

        $"""<div id="members-sections">
{sections}
</div>"""

    let membersPage (churchName: string) (members: ChurchAttendance.Member list) =
        let content =
            $"""<div class="page-header">
    <h1>Members</h1>
    <button hx-get="/members/new" hx-target="#member-form-area" hx-swap="innerHTML" onclick="openModal();document.getElementById('modal-title').textContent='Add New Member'">+ Add Member</button>
</div>
<div class="modal-overlay" id="member-modal-overlay" style="display: none;" onclick="if(event.target === this) closeModal()">
    <div class="modal-content">
        <button class="modal-close" onclick="closeModal()">&times;</button>
        <h3 id="modal-title">Member</h3>
        <div id="member-form-area"></div>
    </div>
</div>
<input type="search" id="member-name-filter" placeholder="Search members...">
{membersTable members}"""

        layout churchName "Members" "members" content

    let attendancePage (churchName: string) (initialDate: string option) =
        // Default to today or use provided date
        let today = DateTime.Today
        let todayStr = today.ToString("yyyy-MM-dd")
        let dateStr = initialDate |> Option.defaultValue todayStr

        let content =
            $"""<h1>Attendance</h1>
<form hx-get="/attendance/list" hx-target="#attendance-area" hx-swap="innerHTML" hx-trigger="load, change from:#date">
    <label for="date">Date
        <input type="date" id="date" name="date" value="{dateStr}" max="{todayStr}" required>
        <small>Service type auto-detected: Sunday = Sunday Service, Friday = Prayer Meeting</small>
    </label>
</form>
<div id="attendance-area"></div>"""

        layout churchName "Attendance" "attendance" content

    let attendanceChecklist
        (members: ChurchAttendance.Member list)
        (date: string)
        (serviceType: string)
        (serviceLabel: string)
        (isToday: bool)
        (checkedIds: Set<Guid>)
        (_firstTimerIds: Set<Guid>)
        =
        let activeMembers =
            members
            |> List.filter (fun m -> m.IsActive)

        let abbreviatedLabel =
            match serviceLabel with
            | "Sunday Service" -> "Sun Service"
            | "Prayer Meeting" -> "Prayer Mtg"
            | s -> s

        let parsedDate =
            match DateTime.TryParse(date) with
            | true, d -> d.ToString("MMM d")
            | _ -> date

        let renderMemberRow (m: ChurchAttendance.Member) =
            let isChecked = if checkedIds.Contains m.Id then " checked" else ""
            let nameAttr = htmlEncode (m.FullName.ToLowerInvariant())
            let nameText = htmlEncode m.FullName
            $"""<label class="attendance-row" data-name="{nameAttr}">
            <input type="checkbox" name="memberIds" value="{m.Id}" data-member-id="{m.Id}"{isChecked}> {nameText}
        </label>"""

        let renderSection (label: string) (checkedCount: int) (total: int) (memberRows: string) =
            $"""<div class="age-group-section collapsed" data-age-group="{label}">
        <div class="age-group-header"><span class="section-toggle">&#9654;</span> {label} (<span class="section-checked">{checkedCount}</span>/{total})</div>
        <div class="age-group-body">
        {memberRows}
        </div>
    </div>"""

        let ageGroupSections =
            Domain.allAgeGroups
            |> List.choose (fun ag ->
                let groupMembers =
                    activeMembers
                    |> List.filter (fun m -> m.AgeGroup = ag)
                    |> List.sortBy (fun m -> m.FullName)

                if groupMembers.IsEmpty then
                    None
                else
                    let label = Domain.ageGroupLabel ag
                    let total = groupMembers.Length
                    let checkedCount = groupMembers |> List.filter (fun m -> checkedIds.Contains m.Id) |> List.length
                    let memberRows = groupMembers |> List.map renderMemberRow |> String.concat "\n"
                    Some (renderSection label checkedCount total memberRows))
            |> String.concat "\n"

        let totalChecked = activeMembers |> List.filter (fun m -> checkedIds.Contains m.Id) |> List.length

        let topBar =
            $"""<div class="attendance-top-bar" id="attendance-top-bar">
        <div class="top-bar-info">
            <span class="top-bar-label">{htmlEncode abbreviatedLabel} &middot; {htmlEncode parsedDate}</span>
            <span class="top-bar-count" id="top-bar-count">{totalChecked} present</span>
        </div>
        <button type="button" class="top-bar-search-btn" id="search-toggle" aria-label="Search">&#128269;</button>
        <div class="top-bar-search" id="top-bar-search">
            <input type="search" id="name-filter" placeholder="Search name..." autofocus>
            <button type="button" class="top-bar-search-close" id="search-close" aria-label="Close search">&times;</button>
        </div>
    </div>"""

        let bottomBar =
            $"""<div class="attendance-bottom-bar">
        <span id="auto-save-status"></span>
        <div class="bottom-bar-actions">
            <button type="submit">View Summary</button>
            <button type="button" class="secondary" onclick="shareAttendancePdf()">Share PDF</button>
        </div>
    </div>
    <div id="attendance-pdf-status"></div>"""

        let formHtml =
            $"""{topBar}
    {ageGroupSections}
    {bottomBar}"""

        let confirmHtml =
            if isToday then
                formHtml
            else
                $"""<div id="date-confirm">
    <p>You are about to record attendance for a <strong>past date</strong>. Continue?</p>
    <button type="button" onclick="document.getElementById('date-confirm').style.display='none';document.getElementById('checklist-body').style.display='';">Yes, continue</button>
</div>
<div id="checklist-body" style="display:none">
    {formHtml}
</div>"""

        $"""<form hx-post="/attendance" hx-target="#attendance-area" hx-swap="innerHTML">
    <input type="hidden" name="date" value="{htmlEncode date}">
    <input type="hidden" name="serviceType" value="{htmlEncode serviceType}">
    {confirmHtml}
</form>"""

    let attendanceSummary
        (date: string)
        (serviceTypeLabel: string)
        (totalPresent: int)
        (ageGroupCounts: (string * int) list)
        (categoryCounts: (string * int) list)
        (firstTimers: string list)
        =
        let ageRows =
            ageGroupCounts
            |> List.map (fun (label, count) -> $"<tr><td>{label}</td><td>{count}</td></tr>")
            |> String.concat "\n"

        let catRows =
            categoryCounts
            |> List.map (fun (label, count) -> $"<tr><td>{label}</td><td>{count}</td></tr>")
            |> String.concat "\n"

        let firstTimerSection =
            if firstTimers.IsEmpty then
                ""
            else
                let items =
                    firstTimers
                    |> List.map (fun n -> $"<li>{htmlEncode n}</li>")
                    |> String.concat ""

                $"""<h4>First Timers</h4><ul>{items}</ul>"""

        $"""<article>
    <header><h3>Attendance Summary</h3></header>
    <p><strong>Date:</strong> {htmlEncode date} | <strong>Service:</strong> {htmlEncode serviceTypeLabel} | <strong>Total Present:</strong> {totalPresent}</p>
    <div class="grid">
        <div>
            <h4>By Age Group</h4>
            <table>
                <thead><tr><th>Age Group</th><th>Count</th></tr></thead>
                <tbody>{ageRows}</tbody>
            </table>
        </div>
        <div>
            <h4>By Category</h4>
            <table>
                <thead><tr><th>Category</th><th>Count</th></tr></thead>
                <tbody>{catRows}</tbody>
            </table>
        </div>
    </div>
    {firstTimerSection}
    <footer>
        <button type="button" class="secondary" onclick="window.location.href='/attendance?date={htmlEncode date}'">Back to Attendance</button>
    </footer>
</article>
{statusMessage "Attendance saved successfully!" false}"""

    let reportsPage (churchName: string) =
        let today = DateTime.Today.ToString("yyyy-MM-dd")
        let weekAgo = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd")

        let content =
            $"""<h1>Reports</h1>
<article>
    <header><h3>Generate Attendance Report</h3></header>
    <div class="grid">
        <label for="startDate">Start Date
            <input type="date" id="startDate" name="startDate" value="{weekAgo}">
        </label>
        <label for="endDate">End Date
            <input type="date" id="endDate" name="endDate" value="{today}">
        </label>
    </div>
    <div class="grid">
        <form method="post" action="/reports/export" id="export-pdf-form">
            <input type="hidden" name="startDate" class="report-start-date" value="{weekAgo}">
            <input type="hidden" name="endDate" class="report-end-date" value="{today}">
            <button type="submit" id="export-pdf-btn">Download PDF</button>
        </form>
        <button type="button" class="secondary" id="share-pdf-btn" onclick="sharePdf()">Share PDF</button>
    </div>
    <div id="report-status"></div>
</article>"""

        layout churchName "Reports" "reports" content

    let landingPage (domain: string) (tenants: (string * string) list) =
        let tenantLinks =
            tenants
            |> List.map (fun (slug, name) ->
                let url = $"https://{slug}.{domain}"
                $"""<a href="{htmlEncode url}" role="button" class="outline" style="display:block;margin-bottom:0.5rem">{htmlEncode name}</a>""")
            |> String.concat "\n"

        $"""<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Church Attendance</title>
    {themeScript}
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@picocss/pico@2/css/pico.min.css">
    <link rel="stylesheet" href="/css/app.css?v=16">
</head>
<body>
    <div style="position:absolute;top:1rem;right:1rem">
        <a href="#" class="theme-toggle-text" id="theme-toggle" aria-label="Toggle dark mode">Dark Mode</a>
    </div>
    <main class="container" style="max-width:500px;margin-top:10vh">
        <article>
            <header><h3 style="text-align:center">Church Attendance</h3></header>
            <p style="text-align:center">Select your church:</p>
            {tenantLinks}
        </article>
    </main>
    <script src="/js/app.js?v=15"></script>
</body>
</html>"""

