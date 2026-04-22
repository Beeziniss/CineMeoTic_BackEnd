using BuildingBlocks.Models;
using CineMeoTic.UserService.API.Services.Intefaces;
using Serilog;
using System.Net;
using System.Net.Mail;

namespace CineMeoTic.UserService.API.Services.Implements;

public sealed class EmailService : IEmailService
{
    public void Send(EmailTemplateType templateType, string toEmail, params string[] parameters)
    {
        try
        {
            SmtpClient smtpClient = new(Environment.GetEnvironmentVariable("EMAIL_SMTP_HOST"))
            {
                Port = Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT") != null ? int.Parse(Environment.GetEnvironmentVariable("EMAIL_SMTP_PORT")!) : 587,

                Credentials = new NetworkCredential(Environment.GetEnvironmentVariable("EMAIL_SMTP_USERNAME"), Environment.GetEnvironmentVariable("EMAIL_SMTP_PASSWORD")),

                EnableSsl = true,
            };

            Func<string[], string> emailTemplate = EmailTemplateFactory.GetTemplate(templateType);
            string subject = EmailTemplateFactory.GetSubject(templateType);

            MailMessage mailMessage = new()
            {
                From = new MailAddress(Environment.GetEnvironmentVariable("EMAIL_SMTP_USERNAME")!),
                Subject = subject,
                Body = emailTemplate.Invoke(parameters),
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            mailMessage.Headers.Add("X-Priority", "1");
            mailMessage.Headers.Add("X-MSMail-Priority", "High");
            mailMessage.Headers.Add("Importance", "High");

            smtpClient.Send(mailMessage);
            Log.Information($"Email sent to {toEmail}");
        }
        catch (Exception ex)
        {
            Log.Error(ex, $"Failed to send email to {toEmail}. Error: {ex.Message}");
        }
    }
}
