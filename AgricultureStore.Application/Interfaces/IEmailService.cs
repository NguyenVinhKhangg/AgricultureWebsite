namespace AgricultureStore.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true);
        Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmationToken);
        Task SendPasswordResetAsync(string toEmail, string userName, string resetToken);
    }
}
