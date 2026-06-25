using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;
using StarAirAdm.Application.Interfaces;
using StarAirAdm.Application.Models;

namespace StarAirAdm.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly IConfiguration _configuration;

    public EmailService(IOptions<EmailSettings> emailSettings, IConfiguration configuration)
    {
        _emailSettings = emailSettings.Value;
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var email = new MimeMessage();
        email.Sender = MailboxAddress.Parse(_emailSettings.FromEmail);
        email.Sender.Name = _emailSettings.FromName;
        email.To.Add(MailboxAddress.Parse(to));
        email.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        email.Body = builder.ToMessageBody();
        email.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_emailSettings.Host, _emailSettings.Port, _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
        await smtp.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string fullName, string invitationToken)
    {
        var frontendUrl = _configuration.GetValue<string>("FrontendUrl");
        var activeUrl = $"{frontendUrl}/set-password?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(invitationToken)}";

        var subject = "Welcome to STAR Air ADM - Set Your Password";
        var body = $@"
            <html>
            <body>
                <h2>Welcome to STAR Air, {fullName}!</h2>
                <p>An administrator has created an account for you as a pilot in the STAR Air ADM system.</p>
                <p>To access your account, please set your password by clicking the link below:</p>
                <p><a href='{activeUrl}' target='_blank'>Activate Account & Set Password</a></p>
                <br>
                <p>This invitation link will expire in 7 days.</p>
                <p>If you have any questions, please contact your systems administrator.</p>
                <p>Best regards,<br/>The STAR Air Team</p>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetToken)
    {
        var frontendUrl = _configuration.GetValue<string>("FrontendUrl");
        var activeUrl = $"{frontendUrl}/reset-password?email={Uri.EscapeDataString(toEmail)}&token={Uri.EscapeDataString(resetToken)}";

        var subject = "Reset Your Password - STAR Air ADM";
        var body = $@"
            <html>
            <body>
                <h2>Password Reset Request</h2>
                <p>Hi {fullName},</p>
                <p>We received a request to reset your password for the STAR Air ADM system.</p>
                <p>You can reset your password by clicking the link below:</p>
                <p><a href='{activeUrl}' target='_blank'>Reset Password</a></p>
                <br>
                <p>If you did not request a password reset, please ignore this email.</p>
                <p>Best regards,<br/>The STAR Air Team</p>
            </body>
            </html>
        ";

        await SendEmailAsync(toEmail, subject, body);
    }
}
