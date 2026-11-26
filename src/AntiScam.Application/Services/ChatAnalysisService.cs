using AntiScam.Application.Common;
using AntiScam.Application.DTOs.Classification;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Interfaces;

namespace AntiScam.Application.Services;

public interface IChatAnalysisService
{
    Task<Result<ClassifyChatResponse>> AnalyzeChatAsync(ClassifyChatRequest request, string? userId);
}

public class ChatAnalysisService : IChatAnalysisService
{
    private readonly IClassificationService _classificationService;
    private readonly IUnitOfWork _unitOfWork;

    public ChatAnalysisService(IClassificationService classificationService, IUnitOfWork unitOfWork)
    {
        _classificationService = classificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ClassifyChatResponse>> AnalyzeChatAsync(ClassifyChatRequest request, string? userId)
    {
        if (request.Messages == null || request.Messages.Count == 0)
            return Result<ClassifyChatResponse>.Failure("Đoạn chat không có tin nhắn nào.");

        // ── Phân tích từng tin nhắn từ đối phương ──────────────────────────
        var analyses   = new List<ChatMessageAnalysis>();
        var scamTypes  = new List<string>();

        foreach (var (msg, idx) in request.Messages.Select((m, i) => (m, i)))
        {
            if (string.IsNullOrWhiteSpace(msg.Text)) continue;

            // Chỉ classify tin nhắn từ "them" (đối phương)
            if (!msg.Sender.Equals("them", StringComparison.OrdinalIgnoreCase))
            {
                analyses.Add(new ChatMessageAnalysis
                {
                    Sender = msg.Sender,
                    Text = msg.Text,
                    IsSuspicious = false,
                    DetectedSignals = new List<string>()
                });
                continue;
            }

            var classifyReq = new ClassifyMessageRequest
            {
                Content = msg.Text,
                ConsentToStore = false
            };

            var result = await _classificationService.ClassifyAsync(classifyReq, userId);
            var isSuspicious = result.IsSuccess && result.Value!.IsScam;

            if (isSuspicious && result.Value!.ScamTypeName is { Length: > 0 } stype)
                scamTypes.Add(stype);

            analyses.Add(new ChatMessageAnalysis
            {
                Sender       = msg.Sender,
                Text         = msg.Text,
                IsSuspicious = isSuspicious,
                DetectedSignals = result.IsSuccess ? result.Value!.MatchedSignals : new(),
                ScamTypeName = result.IsSuccess ? result.Value!.ScamTypeName : null
            });
        }

        // ── Phát hiện thủ đoạn trong toàn bộ đoạn chat ─────────────────────
        var tactics = DetectTactics(request.Messages, analyses);

        // ── Tính điểm rủi ro tổng thể ──────────────────────────────────────
        var suspiciousCount  = analyses.Count(a => a.IsSuspicious);
        var themMessages     = analyses.Count(a =>
            a.Sender.Equals("them", StringComparison.OrdinalIgnoreCase));
        double rawRisk = themMessages == 0 ? 0.0
            : (double)suspiciousCount / themMessages;

        // Bonus risk từ số thủ đoạn phát hiện
        double tacticBonus = Math.Min(0.3, tactics.Count * 0.08);
        double overallRisk = Math.Min(1.0, rawRisk + tacticBonus);

        bool isScam = overallRisk >= 0.3 || tactics.Count >= 2;

        string riskLevel = overallRisk switch
        {
            >= 0.75 => "Rất cao",
            >= 0.5  => "Cao",
            >= 0.25 => "Trung bình",
            _       => "Thấp"
        };

        string primaryScamType = scamTypes.GroupBy(x => x)
            .OrderByDescending(g => g.Count())
            .FirstOrDefault()?.Key;

        string advice = GetAdvice(primaryScamType, tactics);

        // ── Lưu training data nếu user đồng ý ──────────────────────────────
        if (request.ConsentToStore && isScam)
        {
            var combined = string.Join("\n",
                request.Messages
                    .Where(m => m.Sender.Equals("them", StringComparison.OrdinalIgnoreCase))
                    .Select(m => m.Text));

            if (!string.IsNullOrWhiteSpace(combined))
            {
                await _unitOfWork.TrainingDataSamples.AddAsync(new TrainingDataSample
                {
                    Content   = AnonymizeText(combined),
                    IsScam    = true,
                    BatchId   = "community-chat",
                    Source    = "Phân tích đoạn chat người dùng đóng góp",
                    CreatedAt = DateTime.UtcNow,
                    IsActive  = true
                });
                await _unitOfWork.SaveChangesAsync();
            }
        }

        return Result<ClassifyChatResponse>.Success(new ClassifyChatResponse
        {
            IsScam                = isScam,
            OverallRiskScore      = Math.Round(overallRisk, 2),
            RiskLevel             = riskLevel,
            PrimaryScamType       = primaryScamType,
            MessageAnalyses       = analyses,
            DetectedTactics       = tactics,
            SuspiciousMessageCount = suspiciousCount,
            TotalMessageCount     = request.Messages.Count,
            PreventionAdvice      = advice
        });
    }

    // ── Phát hiện thủ đoạn theo pattern ─────────────────────────────────────
    private static List<DetectedTactic> DetectTactics(
        List<ChatMessage> messages,
        List<ChatMessageAnalysis> analyses)
    {
        var tactics = new List<DetectedTactic>();
        var allThemText = string.Join(" ", messages
            .Where(m => m.Sender.Equals("them", StringComparison.OrdinalIgnoreCase))
            .Select(m => m.Text.ToLowerInvariant()));

        var indices = Enumerable.Range(0, messages.Count)
            .Where(i => messages[i].Sender.Equals("them", StringComparison.OrdinalIgnoreCase))
            .ToList();

        // Thủ đoạn 1: Tạo áp lực thời gian
        if (ContainsAny(allThemText,
            "ngay hôm nay", "trong vòng", "khẩn cấp", "gấp", "ngay lập tức",
            "hạn chót", "24 giờ", "hết hạn", "cuối ngày"))
        {
            tactics.Add(new DetectedTactic
            {
                Name        = "Tạo áp lực thời gian",
                Description = "Kẻ lừa đảo thúc ép nạn nhân hành động nhanh, không cho thời gian suy nghĩ hoặc hỏi người khác.",
                MessageIndices = indices
            });
        }

        // Thủ đoạn 2: Giả danh thẩm quyền
        if (ContainsAny(allThemText,
            "công an", "cảnh sát", "bộ công an", "viện kiểm sát",
            "cơ quan điều tra", "ngân hàng nhà nước", "bộ tài chính",
            "nhân viên", "cán bộ"))
        {
            tactics.Add(new DetectedTactic
            {
                Name        = "Giả danh cơ quan thẩm quyền",
                Description = "Kẻ lừa đảo mạo danh cơ quan nhà nước, ngân hàng hoặc tổ chức uy tín để tạo niềm tin và sợ hãi.",
                MessageIndices = indices
            });
        }

        // Thủ đoạn 3: Yêu cầu thông tin bí mật
        if (ContainsAny(allThemText,
            "mã otp", "mật khẩu", "số tài khoản", "thẻ căn cước",
            "chứng minh nhân dân", "thông tin cá nhân", "mã pin",
            "xác thực", "xác minh danh tính"))
        {
            tactics.Add(new DetectedTactic
            {
                Name        = "Thu thập thông tin bí mật",
                Description = "Yêu cầu cung cấp mã OTP, mật khẩu, số tài khoản hoặc thông tin định danh cá nhân — KHÔNG BAO GIỜ chia sẻ những thông tin này.",
                MessageIndices = indices
            });
        }

        // Thủ đoạn 4: Hứa hẹn lợi nhuận bất thường
        if (ContainsAny(allThemText,
            "lợi nhuận", "hoa hồng", "trúng thưởng", "miễn phí",
            "hoàn tiền", "nhận tiền", "kiếm tiền", "thu nhập cao",
            "lãi suất", "tăng trưởng"))
        {
            tactics.Add(new DetectedTactic
            {
                Name        = "Hứa hẹn lợi ích bất thường",
                Description = "Dụ dỗ bằng phần thưởng, hoa hồng hoặc lợi nhuận cao bất thường — đây là dấu hiệu kinh điển của lừa đảo.",
                MessageIndices = indices
            });
        }

        // Thủ đoạn 5: Yêu cầu chuyển tiền / cọc
        if (ContainsAny(allThemText,
            "chuyển tiền", "chuyển khoản", "đặt cọc", "phí xử lý",
            "phí kích hoạt", "phí bảo hiểm", "nạp tiền", "thanh toán trước"))
        {
            tactics.Add(new DetectedTactic
            {
                Name        = "Yêu cầu chuyển tiền trước",
                Description = "Yêu cầu chuyển tiền đặt cọc, phí xử lý hoặc phí kích hoạt — sau khi nhận tiền kẻ lừa đảo sẽ mất liên lạc.",
                MessageIndices = indices
            });
        }

        // Thủ đoạn 6: Tạo mối quan hệ cảm xúc (Romance Scam)
        if (ContainsAny(allThemText,
            "yêu", "thương", "nhớ", "anh ơi", "em ơi", "người thân",
            "gia đình", "bạn bè", "quen biết", "tin tưởng"))
        {
            var suspiciousCount = analyses.Count(a => a.IsSuspicious);
            if (suspiciousCount >= 1)
            {
                tactics.Add(new DetectedTactic
                {
                    Name        = "Lợi dụng cảm xúc / Mối quan hệ",
                    Description = "Xây dựng lòng tin qua cảm xúc trước khi đưa ra yêu cầu — thủ đoạn phổ biến trong lừa đảo tình cảm và nhờ vả.",
                    MessageIndices = indices
                });
            }
        }

        // Thủ đoạn 7: Link giả mạo
        if (ContainsAny(allThemText, "http", "www.", ".top", ".xyz", ".vip", ".club", "bit.ly", "tinyurl"))
        {
            tactics.Add(new DetectedTactic
            {
                Name        = "Đường link / Website giả mạo",
                Description = "Chứa liên kết dẫn đến website giả mạo ngân hàng, cơ quan nhà nước hoặc sàn giao dịch — KHÔNG click vào.",
                MessageIndices = indices
            });
        }

        return tactics;
    }

    private static bool ContainsAny(string text, params string[] keywords)
        => keywords.Any(kw => text.Contains(kw, StringComparison.OrdinalIgnoreCase));

    private static string GetAdvice(string? scamType, List<DetectedTactic> tactics)
    {
        var advices = new List<string>();

        if (tactics.Any(t => t.Name.Contains("thông tin bí mật")))
            advices.Add("Tuyệt đối KHÔNG cung cấp mã OTP, mật khẩu hay số tài khoản cho bất kỳ ai qua điện thoại/chat.");

        if (tactics.Any(t => t.Name.Contains("chuyển tiền")))
            advices.Add("KHÔNG chuyển tiền cho người lạ dù bất kỳ lý do gì, kể cả đặt cọc hay phí xử lý.");

        if (tactics.Any(t => t.Name.Contains("thẩm quyền")))
            advices.Add("Cơ quan công an KHÔNG làm việc qua điện thoại/Zalo. Hãy đến trực tiếp trụ sở để xác minh.");

        if (tactics.Any(t => t.Name.Contains("link")))
            advices.Add("KHÔNG click vào đường link lạ. Truy cập trực tiếp website chính thức của ngân hàng/cơ quan.");

        if (tactics.Any(t => t.Name.Contains("áp lực thời gian")))
            advices.Add("Hãy bình tĩnh, hỏi ý kiến người thân hoặc gọi đường dây nóng 113 trước khi làm bất kỳ điều gì.");

        if (advices.Count == 0)
            advices.Add("Hãy cẩn thận với người lạ liên hệ qua mạng. Xác minh danh tính và mục đích trước khi chia sẻ bất kỳ thông tin nào.");

        return string.Join(" ", advices);
    }

    private static string AnonymizeText(string text)
    {
        var masked = System.Text.RegularExpressions.Regex.Replace(
            text, @"0[3|5|7|8|9]\d{8}", "***SDT***");
        masked = System.Text.RegularExpressions.Regex.Replace(
            masked, @"\b\d{9,16}\b", "***STK***");
        return masked;
    }
}
