# Church Attendance

A web application for tracking church attendance, managing members, and generating PDF reports. Built with F# and ASP.NET Minimal APIs.

## Features

- **Member Management** - Add, edit, and deactivate members with age group and category tracking
- **Attendance Tracking** - Check off members per service with auto-save, date selection, and filtering by name or age group
- **First Timer Detection** - Automatically highlights members attending for the first time based on their first attended date
- **Service Type Auto-Detection** - Infers Sunday Service or Prayer Meeting from the selected date
- **PDF Reports** - Generate attendance reports for any date range with breakdowns by age group and category
- **PDF Sharing** - Share reports directly from mobile devices via the Web Share API
- **Email Reports** - Send PDF reports via configurable SMTP

## Tech Stack

- **Backend**: F# / ASP.NET Minimal APIs
- **Frontend**: HTMX + Pico CSS
- **PDF Generation**: QuestPDF
- **Email**: MailKit
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
  Database.fs                   # JSON file persistence
  Templates.fs                  # HTML templates (F# string interpolation)
  Handlers.fs                   # HTTP request handlers
  PdfService.fs                 # QuestPDF report generation
  EmailService.fs               # SMTP email delivery
  Program.fs                    # App startup and routing
  wwwroot/                      # Static assets (CSS, JS)

ChurchAttendance.BddTests/      # BDD tests (Gherkin/TickSpec)
ChurchAttendance.PlaywrightTests/ # End-to-end browser tests
docs/                           # Business rules documentation
```

## Data Storage

Data is stored as JSON files in `~/.church-attendance/data/`:

- `members.json` - Member records
- `attendance.json` - Attendance records
- `settings.json` - SMTP configuration

The directory is created automatically on first run.
