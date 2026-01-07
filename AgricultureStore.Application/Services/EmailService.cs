using System.Net;
using System.Net.Mail;
using AgricultureStore.Application.Interfaces;
using AgricultureStore.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AgricultureStore.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body, bool isHtml = true)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    EnableSsl = _emailSettings.EnableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmationToken)
        {
            var confirmationLink = $"{_emailSettings.BaseUrl}/confirm-email?token={confirmationToken}&email={Uri.EscapeDataString(toEmail)}";
            
            var subject = "Xác nhận đăng ký tài khoản - Agriculture Store";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Agriculture Store</h1>
                        </div>
                        <div class='content'>
                            <h2>Xin chào {userName}!</h2>
                            <p>Cảm ơn bạn đã đăng ký tài khoản tại Agriculture Store.</p>
                            <p>Vui lòng nhấn vào nút bên dưới để xác nhận địa chỉ email của bạn:</p>
                            <p style='text-align: center;'>
                                <a href='{confirmationLink}' class='button'>Xác nhận Email</a>
                            </p>
                            <p>Hoặc copy đường link sau vào trình duyệt:</p>
                            <p style='word-break: break-all; background: #eee; padding: 10px;'>{confirmationLink}</p>
                            <p><strong>Lưu ý:</strong> Link xác nhận sẽ hết hạn sau 24 giờ.</p>
                        </div>
                        <div class='footer'>
                            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                            <p>&copy; 2026 Agriculture Store. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
            _logger.LogInformation("Email confirmation sent to {Email} for user {Username}", toEmail, userName);
        }

        public async Task SendPasswordResetAsync(string toEmail, string userName, string resetToken)
        {
            var resetLink = $"{_emailSettings.BaseUrl}/reset-password?token={resetToken}&email={Uri.EscapeDataString(toEmail)}";
            
            var subject = "Đặt lại mật khẩu - Agriculture Store";
            var body = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
                        .content {{ padding: 20px; background-color: #f9f9f9; }}
                        .button {{ display: inline-block; padding: 12px 24px; background-color: #2196F3; color: white; text-decoration: none; border-radius: 4px; margin: 20px 0; }}
                        .footer {{ padding: 20px; text-align: center; font-size: 12px; color: #666; }}
                        .warning {{ background-color: #fff3cd; border: 1px solid #ffc107; padding: 10px; border-radius: 4px; margin: 10px 0; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Agriculture Store</h1>
                        </div>
                        <div class='content'>
                            <h2>Xin chào {userName}!</h2>
                            <p>Chúng tôi nhận được yêu cầu đặt lại mật khẩu cho tài khoản của bạn.</p>
                            <p>Nhấn vào nút bên dưới để đặt lại mật khẩu:</p>
                            <p style='text-align: center;'>
                                <a href='{resetLink}' class='button'>Đặt lại mật khẩu</a>
                            </p>
                            <p>Hoặc copy đường link sau vào trình duyệt:</p>
                            <p style='word-break: break-all; background: #eee; padding: 10px;'>{resetLink}</p>
                            <div class='warning'>
                                <strong>⚠️ Lưu ý:</strong>
                                <ul>
                                    <li>Link đặt lại mật khẩu sẽ hết hạn sau 1 giờ.</li>
                                    <li>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</li>
                                </ul>
                            </div>
                        </div>
                        <div class='footer'>
                            <p>Email này được gửi tự động, vui lòng không trả lời.</p>
                            <p>&copy; 2026 Agriculture Store. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, body);
            _logger.LogInformation("Password reset email sent to {Email} for user {Username}", toEmail, userName);
        }
    }
}
