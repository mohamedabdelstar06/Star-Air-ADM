namespace StarAirAdm.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
    Task SendWelcomeEmailAsync(string toEmail, string fullName, string invitationToken);
    Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetToken);
}
