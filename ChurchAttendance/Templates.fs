namespace ChurchAttendance

open System

module Templates =

    let private htmlEncode (s: string) =
        System.Net.WebUtility.HtmlEncode(s)

    let layout (title: string) (activeNav: string) (content: string) =
        $"""<!DOCTYPE html>
<html lang="en" data-theme="light">
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>{htmlEncode title} - Church Attendance</title>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/@picocss/pico@2/css/pico.min.css">
    <link rel="stylesheet" href="/css/app.css?v=4">
    <script src="https://unpkg.com/htmx.org@2.0.4"></script>
</head>
<body>
    <nav class="container">
        <ul>
            <li><strong>Church Attendance</strong></li>
        </ul>
        <ul>
            <li><a href="/" class="{if activeNav = "home" then "active" else ""}">Dashboard</a></li>
            <li><a href="/members" class="{if activeNav = "members" then "active" else ""}">Members</a></li>
            <li><a href="/attendance" class="{if activeNav = "attendance" then "active" else ""}">Attendance</a></li>
            <li><a href="/reports" class="{if activeNav = "reports" then "active" else ""}">Reports</a></li>
            <li><a href="/settings" class="{if activeNav = "settings" then "active" else ""}">Settings</a></li>
        </ul>
    </nav>
    <main class="container">
        {content}
    </main>
    <script src="/js/app.js"></script>
</body>
</html>"""

    let homePage (memberCount: int) (activeCount: int) (todayAttendance: int) =
        let content =
            $"""<h1>Dashboard</h1>
<div class="grid">
    <article>
        <header>Total Members</header>
        <p class="stat">{memberCount}</p>
    </article>
    <article>
        <header>Active Members</header>
        <p class="stat">{activeCount}</p>
    </article>
    <article>
        <header>Today's Attendance</header>
        <p class="stat">{todayAttendance}</p>
    </article>
</div>"""

        layout "Dashboard" "home" content

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

        $"""<form {hxAttr} hx-target="#members-table" hx-swap="outerHTML">
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
        let ageLabel = Domain.ageGroupLabel m.AgeGroup
        let inactiveClass = if m.IsActive then "" else " inactive"

        let deactivateBtn =
            if m.IsActive then
                $"""<button class="outline secondary small" hx-delete="/members/{m.Id}" hx-target="#members-table" hx-swap="outerHTML" hx-confirm="Deactivate {htmlEncode m.FullName}?">Deactivate</button>"""
            else
                ""

        let statusText = if m.IsActive then "Active" else "Inactive"

        $"""<tr class="{inactiveClass}" data-age-group="{ageLabel}" data-name="{htmlEncode (m.FullName.ToLowerInvariant())}">
    <td>{htmlEncode m.FullName}</td>
    <td>{Domain.ageGroupLabel m.AgeGroup}</td>
    <td>{Domain.categoryLabel m.Category}</td>
    <td>{statusText}</td>
    <td>
        <button class="outline small" hx-get="/members/{m.Id}/edit" hx-target="#member-form-area" hx-swap="innerHTML" onclick="openModal();document.getElementById('modal-title').textContent='Edit Member'">Edit</button>
        {deactivateBtn}
    </td>
</tr>"""

    let membersTable (members: ChurchAttendance.Member list) =
        let rows =
            members
            |> List.sortBy (fun m -> (not m.IsActive, m.FullName))
            |> List.map memberRow
            |> String.concat "\n"

        $"""<table id="members-table" role="grid">
    <thead>
        <tr>
            <th>Name</th>
            <th>Age Group</th>
            <th>Category</th>
            <th>Status</th>
            <th>Actions</th>
        </tr>
    </thead>
    <tbody>
        {rows}
    </tbody>
</table>"""

    let membersPage (members: ChurchAttendance.Member list) =
        let ageGroupFilterOptions =
            Domain.allAgeGroups
            |> List.map (fun ag ->
                let label = Domain.ageGroupLabel ag
                $"""<option value="{label}">{label}</option>""")
            |> String.concat "\n"

        let content =
            $"""<h1>Members</h1>
<div class="modal-overlay" id="member-modal-overlay" style="display: none;" onclick="if(event.target === this) closeModal()">
    <div class="modal-content">
        <button class="modal-close" onclick="closeModal()">&times;</button>
        <h3 id="modal-title">Member</h3>
        <div id="member-form-area"></div>
    </div>
</div>
<div class="grid">
    <div></div>
    <button hx-get="/members/new" hx-target="#member-form-area" hx-swap="innerHTML" onclick="openModal();document.getElementById('modal-title').textContent='Add New Member'">Add New Member</button>
</div>
<div class="grid">
    <label for="member-name-filter">Search by Name
        <input type="search" id="member-name-filter" placeholder="Type a name...">
    </label>
    <label for="member-age-group-filter">Filter by Age Group
        <select id="member-age-group-filter">
            <option value="">All</option>
            {ageGroupFilterOptions}
        </select>
    </label>
</div>
{membersTable members}"""

        layout "Members" "members" content

    let attendancePage (initialDate: string option) =
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

        layout "Attendance" "attendance" content

    let attendanceChecklist
        (members: ChurchAttendance.Member list)
        (date: string)
        (serviceType: string)
        (serviceLabel: string)
        (isToday: bool)
        (checkedIds: Set<Guid>)
        (firstTimerIds: Set<Guid>)
        =
        let memberRow (m: ChurchAttendance.Member) =
            let isChecked =
                if checkedIds.Contains m.Id then
                    " checked"
                else
                    ""

            let ageLabel = Domain.ageGroupLabel m.AgeGroup

            let firstTimerBadge =
                if firstTimerIds.Contains m.Id && checkedIds.Contains m.Id then
                    " <mark>First Timer</mark>"
                else
                    ""

            $"""<tr data-age-group="{ageLabel}" data-name="{htmlEncode (m.FullName.ToLowerInvariant())}">
    <td><input type="checkbox" name="memberIds" value="{m.Id}"{isChecked}></td>
    <td>{htmlEncode m.FullName}{firstTimerBadge}</td>
    <td>{ageLabel}</td>
    <td>{Domain.categoryLabel m.Category}</td>
</tr>"""

        let rows =
            members
            |> List.filter (fun m -> m.IsActive)
            |> List.sortBy (fun m -> m.FullName)
            |> List.map memberRow
            |> String.concat "\n"

        let ageGroupFilterOptions =
            Domain.allAgeGroups
            |> List.map (fun ag ->
                let label = Domain.ageGroupLabel ag
                $"""<option value="{label}">{label}</option>""")
            |> String.concat "\n"

        let bannerClass = if isToday then "date-banner today" else "date-banner past-date"
        let bannerExtra = if isToday then "" else " (not today)"

        let formHtml =
            $"""<div id="attendance-count" class="attendance-counter">
        <strong>0 present</strong>
    </div>
    <div class="grid">
        <label for="name-filter">Search by Name
            <input type="search" id="name-filter" placeholder="Type a name...">
        </label>
        <label for="age-group-filter">Filter by Age Group
            <select id="age-group-filter">
                <option value="">All</option>
                {ageGroupFilterOptions}
            </select>
        </label>
    </div>
    <table role="grid">
        <thead>
            <tr>
                <th><input type="checkbox" id="select-all"></th>
                <th>Name</th>
                <th>Age Group</th>
                <th>Category</th>
            </tr>
        </thead>
        <tbody>
            {rows}
        </tbody>
    </table>
    <div class="grid">
        <span id="auto-save-status"></span>
        <button type="submit">View Summary</button>
        <button type="button" class="secondary" onclick="shareAttendancePdf()">Share PDF</button>
    </div>
    <div id="attendance-pdf-status"></div>"""

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

        $"""<div class="{bannerClass}">
    {htmlEncode serviceLabel} &mdash; {htmlEncode date}{bannerExtra}
</div>
<form hx-post="/attendance" hx-target="#attendance-area" hx-swap="innerHTML">
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

    let reportsPage () =
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
        <form hx-post="/reports/email" hx-target="#report-status" hx-swap="innerHTML">
            <input type="hidden" name="startDate" class="report-start-date" value="{weekAgo}">
            <input type="hidden" name="endDate" class="report-end-date" value="{today}">
            <button type="submit" class="outline">Email Report</button>
        </form>
    </div>
    <div id="report-status"></div>
</article>"""

        layout "Reports" "reports" content

    let settingsPage (settings: SmtpSettings) =
        let sslChecked =
            if settings.UseSsl then
                " checked"
            else
                ""

        let content =
            $"""<h1>Settings</h1>
<article>
    <header><h3>SMTP Email Settings</h3></header>
    <form hx-post="/settings/smtp" hx-target="#settings-status" hx-swap="innerHTML">
        <div class="grid">
            <label for="host">SMTP Host
                <input type="text" id="host" name="host" value="{htmlEncode settings.Host}" placeholder="smtp.gmail.com" required>
            </label>
            <label for="port">Port
                <input type="number" id="port" name="port" value="{settings.Port}" required>
            </label>
        </div>
        <div class="grid">
            <label for="username">Username
                <input type="text" id="username" name="username" value="{htmlEncode settings.Username}" required>
            </label>
            <label for="password">Password
                <input type="password" id="password" name="password" value="{htmlEncode settings.Password}" required>
            </label>
        </div>
        <div class="grid">
            <label for="fromEmail">From Email
                <input type="email" id="fromEmail" name="fromEmail" value="{htmlEncode settings.FromEmail}" required>
            </label>
            <label for="toEmail">To Email
                <input type="email" id="toEmail" name="toEmail" value="{htmlEncode settings.ToEmail}" required>
            </label>
        </div>
        <label>
            <input type="checkbox" name="useSsl" value="true"{sslChecked}>
            Use SSL/TLS
        </label>
        <button type="submit">Save Settings</button>
    </form>
    <div id="settings-status"></div>
</article>"""

        layout "Settings" "settings" content
