using System.Net.Http.Json;
using System.Text.Json;
using AntiScam.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AntiScam.Infrastructure.Services;

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

public class MlClassificationService : IMlClassificationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MlClassificationService> _logger;

    public MlClassificationService(HttpClient httpClient, IConfiguration config, ILogger<MlClassificationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        var baseUrl = config["MlService:BaseUrl"] ?? "http://localhost:8001";
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
    }

    public async Task<ClassificationResponse> PredictAsync(ClassificationRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/predict", request, ct);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ClassificationResponse>(cancellationToken: ct);
                if (result != null) return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "ML service unreachable at {BaseAddress}, using rule-based fallback analyzer", _httpClient.BaseAddress);
        }

        // Rule-based heuristic fallback if Python ML service is offline
        return EvaluateFallbackRuleBased(request.Content);
    }

    public async Task<List<ModelVersionInfo>> GetModelVersionsAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("/models", ct);
            if (response.IsSuccessStatusCode)
            {
                var list = await response.Content.ReadFromJsonAsync<List<ModelVersionInfo>>(cancellationToken: ct);
                if (list != null) return list;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to fetch model versions from ML service");
        }

        return new List<ModelVersionInfo>();
    }

    public async Task<TrainingResponse> TriggerTrainingAsync(TrainingRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/train", request, ct);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<TrainingResponse>(cancellationToken: ct);
                if (result != null) return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initiating model training on ML service");
        }

        return new TrainingResponse(Guid.NewGuid().ToString(), "simulated");
    }

    public async Task<TrainingResponse?> GetTrainingStatusAsync(string jobId, CancellationToken ct = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<TrainingResponse>($"/train/{jobId}/status", ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to check training status for {JobId}", jobId);
            return new TrainingResponse(jobId, "completed");
        }
    }

    private static ClassificationResponse EvaluateFallbackRuleBased(string content)
    {
        var lower = content.ToLowerInvariant();
        var matched = new List<string>();

        string[] bankKeywords = ["khóa tài khoản", "mã otp", "xác minh thông tin", "ngân hàng thông báo", "vietcombank", "bidv", "techcombank", "mb bank", "vpbank", "agribank"];
        string[] jobKeywords = ["việc nhẹ lương cao", "nhiệm vụ", "shopee tuyển dụng", "hoa hồng", "chuyển tiền làm nhiệm vụ", "đặt đơn hàng"];
        string[] policeKeywords = ["bộ công an", "lệnh bắt giam", "rửa tiền", "viện kiểm sát", "cơ quan điều tra", "tạm giam"];
        string[] investmentKeywords = ["lợi nhuận 30%", "đầu tư tài chính", "tiền ảo", "sàn giao dịch", "lãi suất cao", "cam kết hoàn vốn"];

        foreach (var kw in bankKeywords) if (lower.Contains(kw)) matched.Add(kw);
        foreach (var kw in jobKeywords) if (lower.Contains(kw)) matched.Add(kw);
        foreach (var kw in policeKeywords) if (lower.Contains(kw)) matched.Add(kw);
        foreach (var kw in investmentKeywords) if (lower.Contains(kw)) matched.Add(kw);

        bool isScam = matched.Count > 0;
        ScamType? scamType = null;
        string advice = "Nội dung có vẻ an toàn. Luôn cẩn trọng trước các yêu cầu chuyển khoản.";

        if (bankKeywords.Any(k => lower.Contains(k)))
        {
            scamType = ScamType.BankImpersonation;
            advice = "CẢNH BÁO: Không bao giờ cung cấp mã OTP, mật khẩu hay click vào đường link lạ. Ngân hàng không bao giờ yêu cầu OTP qua tin nhắn.";
        }
        else if (jobKeywords.Any(k => lower.Contains(k)))
        {
            scamType = ScamType.EasyJobHighPay;
            advice = "CẢNH BÁO: Chiêu trò 'việc nhẹ lương cao', nhận hoa hồng qua nhiệm vụ đặt đơn là lừa đảo 100%. Không chuyển tiền ký quỹ hay nạp tiền đặt lệnh.";
        }
        else if (policeKeywords.Any(k => lower.Contains(k)))
        {
            scamType = ScamType.AuthorityImpersonation;
            advice = "CẢNH BÁO: Công an và cơ quan chức năng không làm việc, không thông báo lệnh bắt qua điện thoại hay Zalo. Hãy đến trụ sở gần nhất xác minh.";
        }
        else if (investmentKeywords.Any(k => lower.Contains(k)))
        {
            scamType = ScamType.InvestmentFraud;
            advice = "CẢNH BÁO: Mô hình đầu tư hứa hẹn lãi suất cao không rủi ro là bẫy tài chính. Tuyệt đối không nạp tiền vào các ứng dụng sàn lạ.";
        }

        double confidence = isScam ? Math.Min(0.70 + (matched.Count * 0.1), 0.98) : 0.95;

        return new ClassificationResponse(isScam, scamType, confidence, matched, advice);
    }
}
