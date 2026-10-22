using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace AntiScam.Infrastructure.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
    Task SendAlertToRelativeAsync(string email, string userName, double lat, double lng, DateTime time, string reason);
    Task SendSosAlertAsync(string email, string userName, double lat, double lng);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        try
        {
            var host = _config["Email:SmtpHost"] ?? "smtp.gmail.com";
            var port = int.TryParse(_config["Email:SmtpPort"], out var p) ? p : 587;
            var username = _config["Email:Username"];
            var password = _config["Email:Password"];
            var fromName = _config["Email:FromName"] ?? "AntiScam Safety Alert";
            var fromEmail = _config["Email:FromEmail"] ?? (username ?? "alert@antiscam.local");

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                _logger.LogWarning("Email credentials not configured. Mocking email to {Recipient}: {Subject}", to, subject);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email successfully sent to {Recipient}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", to);
        }
    }

    public async Task SendAlertToRelativeAsync(string email, string userName, double lat, double lng, DateTime time, string reason)
    {
        var mapsUrl = $"https://www.google.com/maps?q={lat},{lng}";
        var subject = $"[CẢNH BÁO AN TOÀN] Người thân {userName} có dấu hiệu lệch lộ trình";
        var body = $"""
            <div style="font-family: Arial, sans-serif; padding: 20px; border: 2px solid #e53e3e; border-radius: 8px;">
                <h2 style="color: #e53e3e;">CẢNH BÁO LỆCH LỘ TRÌNH DU LỊCH</h2>
                <p>Hệ thống ghi nhận người thân của bạn (<b>{userName}</b>) đã đi lệch khỏi lộ trình du lịch đã đăng ký.</p>
                <p><b>Lý do cảnh báo:</b> {reason}</p>
                <p><b>Thời gian ghi nhận:</b> {time:dd/MM/yyyy HH:mm:ss} UTC</p>
                <p><b>Vị trí toạ độ cuối cùng:</b> {lat:F6}, {lng:F6}</p>
                <div style="margin: 20px 0;">
                    <a href="{mapsUrl}" style="background-color: #e53e3e; color: white; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: bold;" target="_blank">
                        Xem vị trí trên Google Maps
                    </a>
                </div>
                <p style="color: #718096; font-size: 13px;">Đây là thông báo tự động từ Hệ thống Cảnh báo Lừa đảo và An toàn Du lịch AntiScam.</p>
            </div>
            """;

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendSosAlertAsync(string email, string userName, double lat, double lng)
    {
        var mapsUrl = $"https://www.google.com/maps?q={lat},{lng}";
        var subject = $"[KHẨN CẤP SOS] Người thân {userName} đã kích hoạt nút cứu hộ SOS";
        var body = $"""
            <div style="font-family: Arial, sans-serif; padding: 20px; border: 3px solid #b91c1c; border-radius: 8px; background-color: #fff5f5;">
                <h1 style="color: #b91c1c;">TÍN HIỆU CỨU HỘ KHẨN CẤP (SOS)</h1>
                <p style="font-size: 16px;">Người thân <b>{userName}</b> vừa nhấn nút cứu hộ khẩn cấp SOS trên ứng dụng AntiScam!</p>
                <p><b>Vị trí GPS khẩn cấp:</b> {lat:F6}, {lng:F6}</p>
                <p><b>Thời gian:</b> {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss} UTC</p>
                <div style="margin: 25px 0;">
                    <a href="{mapsUrl}" style="background-color: #b91c1c; color: white; padding: 14px 28px; text-decoration: none; border-radius: 6px; font-weight: bold; font-size: 16px;" target="_blank">
                        Mở Google Maps ngay
                    </a>
                </div>
                <p>Vui lòng liên lạc ngay với {userName} hoặc gọi lực lượng cứu hộ địa phương (113, 115) nếu không thể kết nối.</p>
            </div>
            """;

        await SendEmailAsync(email, subject, body);
    }
}
