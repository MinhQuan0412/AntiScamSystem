using AntiScam.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace AntiScam.Infrastructure.Services;



public class TelegramService : ITelegramService
{
    private readonly IConfiguration _config;
    private readonly ILogger<TelegramService> _logger;
    private TelegramBotClient? _botClient;

    public TelegramService(IConfiguration config, ILogger<TelegramService> logger)
    {
        _config = config;
        _logger = logger;
        var token = _config["Telegram:BotToken"];
        if (!string.IsNullOrWhiteSpace(token))
        {
            _botClient = new TelegramBotClient(token);
        }
    }

    public async Task SendMessageAsync(string chatId, string message)
    {
        try
        {
            if (_botClient == null)
            {
                _logger.LogWarning("Telegram BotToken not configured. Mocking Telegram message to {ChatId}: {Message}", chatId, message);
                return;
            }

            if (long.TryParse(chatId, out var parsedChatId))
            {
                await _botClient.SendTextMessageAsync(parsedChatId, message);
                _logger.LogInformation("Telegram message sent to {ChatId}", chatId);
            }
            else
            {
                _logger.LogWarning("Invalid Telegram ChatId format: {ChatId}", chatId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send Telegram message to {ChatId}", chatId);
        }
    }

    public async Task SendAlertToRelativeAsync(string chatId, string userName, double lat, double lng, DateTime time, string reason)
    {
        var mapsUrl = $"https://www.google.com/maps?q={lat},{lng}";
        var msg = $"""
            🚨 *CẢNH BÁO LỆCH LỘ TRÌNH AN TOÀN* 🚨
            
            Người thân: *{userName}*
            Trạng thái: Đi lệch lộ trình du lịch
            Lý do: {reason}
            Thời gian: {time:dd/MM/yyyy HH:mm:ss} UTC
            
            📍 Vị trí GPS cuối cùng:
            {lat:F6}, {lng:F6}
            
            🔗 [Xem trên Google Maps]({mapsUrl})
            """;

        await SendMessageAsync(chatId, msg);
    }

    public async Task SendSosAlertAsync(string chatId, string userName, double lat, double lng)
    {
        var mapsUrl = $"https://www.google.com/maps?q={lat},{lng}";
        var msg = $"""
            🆘 *TÍN HIỆU CỨU HỘ KHẨN CẤP (SOS)* 🆘
            
            Người thân *{userName}* vừa kích hoạt cảnh báo SOS!
            
            📍 Toạ độ khẩn cấp: {lat:F6}, {lng:F6}
            ⏰ Thời gian: {DateTime.UtcNow:dd/MM/yyyy HH:mm:ss} UTC
            
            🔗 [MỞ GOOGLE MAPS NGAY]({mapsUrl})
            
            Vui lòng liên lạc ngay lập tức hoặc gọi 113 / 115!
            """;

        await SendMessageAsync(chatId, msg);
    }
}
