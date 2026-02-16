# Church Attendance

A web application for tracking church attendance, managing members, and generating PDF reports. Built with F# and ASP.NET Minimal APIs.

## Features

- **Dashboard** - At-a-glance statistics: total members, active members, and today's attendance
- **Member Management** - Add, edit, and deactivate members with age group and category tracking. Members are organized in collapsible age group sections with name search filtering
- **Attendance Tracking** - Compact checklist grouped by age group with sticky top/bottom bars. Each checkbox auto-saves individually, supporting multiple users recording attendance simultaneously without data loss
- **First Timer Detection** - Automatically highlights members attending for the first time based on their first attended date
- **Service Type Auto-Detection** - Server-side inference of Sunday Service or Prayer Meeting from the selected date
- **Past-Date Confirmation** - Safety prompt when recording attendance for a date other than today
- **PDF Reports** - Generate attendance reports for any date range with breakdowns by age group and category
- **PDF Sharing** - Share reports directly from mobile devices via the Web Share API

## Tech Stack

- **Backend**: F# / ASP.NET Minimal APIs
- **Frontend**: HTMX + Pico CSS
- **PDF Generation**: QuestPDF
- **Data Storage**: JSON flat files (`~/.church-attendance/data/`)

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run

```bash
cd ChurchAttendance
dotnet run
```

The app will be available at:
- HTTP: http://localhost:5050
- HTTPS: https://localhost:5051

### HTTPS Setup

The app supports HTTPS with a self-signed certificate. See [HTTPS-SETUP.md](ChurchAttendance/HTTPS-SETUP.md) for instructions on generating and installing the certificate.

## Project Structure

```
ChurchAttendance/               # Main application
  Domain.fs                     # Data models and domain logic
  Database.fs                   # JSON file persistence with thread-safe locking
  Templates.fs                  # HTML templates (F# string interpolation)
  Handlers.fs                   # HTTP request handlers
  PdfService.fs                 # QuestPDF report generation
  Program.fs                    # App startup and routing
  wwwroot/                      # Static assets (CSS, JS)
  certs/                        # Self-signed HTTPS certificate

ChurchAttendance.BddTests/      # BDD tests (Gherkin/TickSpec)
ChurchAttendance.PlaywrightTests/ # End-to-end browser tests
docs/                           # Business rules documentation
```

## Data Storage

Data is stored as JSON files in `~/.church-attendance/data/`:

- `members.json` - Member records
- `attendance.json` - Attendance records

The directory is created automatically on first run. If data exists in a previous location (the app's build output directory), a migration hint is logged to the console.
