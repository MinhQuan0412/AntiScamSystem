using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Domain.ValueObjects;

namespace AntiScam.Domain.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user, IList<string> roles);
    string GenerateRefreshToken();
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string htmlBody);
    Task SendAlertToRelativeAsync(string email, string userName, double lat, double lng, DateTime time, string reason);
    Task SendSosAlertAsync(string email, string userName, double lat, double lng);
}

public interface ITelegramService
{
    Task SendMessageAsync(string chatId, string message);
    Task SendAlertToRelativeAsync(string chatId, string userName, double lat, double lng, DateTime time, string reason);
    Task SendSosAlertAsync(string chatId, string userName, double lat, double lng);
}

public record ClassificationRequest(string Content, string ModelVersion = "latest");
public record ClassificationResponse(bool IsScam, ScamType? ScamType, double Confidence, List<string> MatchedSignals, string Advice);
public record TrainingRequest(string DataBatch, string ModelType, Dictionary<string, object>? Hyperparameters = null);
public record TrainingResponse(string JobId, string Status);
public record ModelVersionInfo(string Version, string ModelType, string Task, double Accuracy, double Precision, double Recall, double F1Score, string TrainedAt, bool IsActive);

public interface IMlClassificationService
{
    Task<ClassificationResponse> PredictAsync(ClassificationRequest request, CancellationToken ct = default);
    Task<List<ModelVersionInfo>> GetModelVersionsAsync(CancellationToken ct = default);
    Task<TrainingResponse> TriggerTrainingAsync(TrainingRequest request, CancellationToken ct = default);
    Task<TrainingResponse?> GetTrainingStatusAsync(string jobId, CancellationToken ct = default);
}

public interface IRiskScoreCalculator
{
    RiskScore Calculate(PhoneReport report);
}

public interface IRouteDeviationDetector
{
    bool IsDeviating(GeoPoint current, TravelRoute route, int consecutiveReadings);
    double GetDistanceToRouteMeters(GeoPoint current, TravelRoute route);
    double DistanceToSegment(GeoPoint p, GeoPoint a, GeoPoint b);
}

public interface IAlertCountdownService
{
    void StartCountdown(Guid alertId, int seconds);
    void CancelCountdown(Guid alertId);
}

public interface IReportExportService
{
    Task<byte[]> ExportScamStatisticsToExcelAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<byte[]> ExportScamStatisticsToPdfAsync(DateTime from, DateTime to, CancellationToken ct = default);
}
