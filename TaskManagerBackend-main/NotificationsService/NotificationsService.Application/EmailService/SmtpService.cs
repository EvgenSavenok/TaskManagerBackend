using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using NotificationsService.Application.Contracts.ServicesContracts;

namespace NotificationsService.Application.EmailService;

public class SmtpService(IConfiguration configuration) : ISmtpService
{
    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var emailSettings = configuration.GetSection("EmailSettings");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(emailSettings["SenderName"], emailSettings["SenderEmail"]));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(
            emailSettings["SmtpServer"], 
            int.Parse(emailSettings["Port"]!),
            MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(emailSettings["SenderEmail"], emailSettings["Password"]);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}
