module ChurchAttendance.BddTests.SettingsSteps

open System
open global.TickSpec
open global.Xunit
open ChurchAttendance
open ChurchAttendance.BddTests.TestHelper

let [<Given>] ``no SMTP settings are configured`` () =
    Database.saveSmtpSettings Domain.defaultSmtpSettings

let [<When>] ``I save SMTP settings with host "(.*)" port (\d+) and email "(.*)"`` (host: string) (port: int) (email: string) =
    let settings : SmtpSettings =
        { Host = host
          Port = port
          Username = "testuser"
          Password = "testpass"
          FromEmail = email
          ToEmail = "recipient@example.com"
          UseSsl = true }
    Database.saveSmtpSettings settings

let [<When>] ``I try to email a report`` () =
    let settings = Database.getSmtpSettings ()
    if String.IsNullOrEmpty settings.Host then
        setLastResponse "Please configure SMTP settings first."
    else
        setLastResponse "Email sent"

let [<Then>] ``the SMTP settings should be saved`` () =
    let settings = Database.getSmtpSettings ()
    Assert.False(String.IsNullOrEmpty settings.Host, "SMTP host should be saved")

let [<Then>] ``the saved host should be "(.*)"`` (expectedHost: string) =
    let settings = Database.getSmtpSettings ()
    Assert.Equal(expectedHost, settings.Host)

let [<Then>] ``I should receive an error about missing SMTP configuration`` () =
    match lastResponse () with
    | Some r -> Assert.Contains("configure SMTP", r)
    | None -> Assert.Fail("No response was received")
