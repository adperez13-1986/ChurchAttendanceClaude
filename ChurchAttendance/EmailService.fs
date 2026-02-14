namespace ChurchAttendance

open System
open MimeKit
open MailKit.Net.Smtp
open MailKit.Security

module EmailService =

    let sendReport (settings: SmtpSettings) (pdfBytes: byte array) (reportName: string) =
        try
            let message = new MimeMessage()
            message.From.Add(MailboxAddress("Church Attendance", settings.FromEmail))
            message.To.Add(MailboxAddress("", settings.ToEmail))
            message.Subject <- $"Attendance Report - {reportName}"

            let builder = new BodyBuilder()
            builder.TextBody <- $"Please find the attendance report ({reportName}) attached."

            builder.Attachments.Add(
                $"attendance-report-{reportName}.pdf",
                pdfBytes,
                ContentType("application", "pdf")
            )
            |> ignore

            message.Body <- builder.ToMessageBody()

            use client = new SmtpClient()

            let secureOption =
                if settings.UseSsl then
                    SecureSocketOptions.StartTls
                else
                    SecureSocketOptions.None

            client.Connect(settings.Host, settings.Port, secureOption)

            if
                not (String.IsNullOrWhiteSpace settings.Username)
                && not (String.IsNullOrWhiteSpace settings.Password)
            then
                client.Authenticate(settings.Username, settings.Password)

            client.Send(message) |> ignore
            client.Disconnect(true)
            Ok "Email sent successfully!"
        with ex ->
            Error $"Failed to send email: {ex.Message}"
