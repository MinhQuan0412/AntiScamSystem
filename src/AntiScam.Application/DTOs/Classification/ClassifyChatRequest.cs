using System.ComponentModel.DataAnnotations;

namespace AntiScam.Application.DTOs.Classification;

/// <summary>
/// One message in a chat conversation.
/// </summary>
public sealed class ChatMessage
{
    /// <summary>Sender label: "them" = đối phương, "me" = người dùng.</summary>
    public string Sender { get; set; } = "them";

    /// <summary>Message text content.</summary>
    [Required]
    public string Text { get; set; } = string.Empty;
}

/// <summary>
/// Request to analyze a full chat conversation for scam patterns.
/// </summary>
public sealed class ClassifyChatRequest
{
    /// <summary>List of chat messages in chronological order.</summary>
    [Required]
    [MinLength(1)]
    public List<ChatMessage> Messages { get; set; } = new();

    /// <summary>Consent to store anonymized data for model improvement.</summary>
    public bool ConsentToStore { get; set; }
}

/// <summary>
/// Analysis result for a single message within a chat.
/// </summary>
public sealed class ChatMessageAnalysis
{
    public string Sender { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsSuspicious { get; set; }
    public List<string> DetectedSignals { get; set; } = new();
    public string? ScamTypeName { get; set; }
}

/// <summary>
/// Overall scam tactic detected across the conversation.
/// </summary>
public sealed class DetectedTactic
{
    /// <summary>Tên thủ đoạn (ví dụ: "Tạo áp lực thời gian").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Mô tả thủ đoạn.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Tin nhắn nào trong cuộc trò chuyện thể hiện thủ đoạn này.</summary>
    public List<int> MessageIndices { get; set; } = new();
}

/// <summary>
/// Full analysis result for a chat conversation.
/// </summary>
public sealed class ClassifyChatResponse
{
    /// <summary>Kết luận tổng thể: có phải lừa đảo không.</summary>
    public bool IsScam { get; set; }

    /// <summary>Mức độ nguy hiểm tổng thể (0.0 – 1.0).</summary>
    public double OverallRiskScore { get; set; }

    /// <summary>Mức rủi ro dạng text: Thấp / Trung bình / Cao / Rất cao.</summary>
    public string RiskLevel { get; set; } = string.Empty;

    /// <summary>Loại lừa đảo chính phát hiện được.</summary>
    public string? PrimaryScamType { get; set; }

    /// <summary>Phân tích từng tin nhắn.</summary>
    public List<ChatMessageAnalysis> MessageAnalyses { get; set; } = new();

    /// <summary>Các thủ đoạn phát hiện trong toàn bộ đoạn chat.</summary>
    public List<DetectedTactic> DetectedTactics { get; set; } = new();

    /// <summary>Số tin nhắn đáng ngờ / tổng.</summary>
    public int SuspiciousMessageCount { get; set; }
    public int TotalMessageCount { get; set; }

    /// <summary>Khuyến nghị phòng tránh.</summary>
    public string PreventionAdvice { get; set; } = string.Empty;

    /// <summary>Disclaimer.</summary>
    public string Disclaimer { get; set; } =
        "Kết quả phân tích chỉ mang tính tham khảo. Hãy xác minh với cơ quan chức năng nếu nghi ngờ.";
}
