using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Domain.ValueObjects;

namespace AntiScam.Infrastructure.Services;

public interface IRiskScoreCalculator
{
    RiskScore Calculate(PhoneReport report);
}

public class RiskScoreCalculator : IRiskScoreCalculator
{
    public RiskScore Calculate(PhoneReport report)
    {
        var factors = new Dictionary<string, double>();
        double score = 0.0;

        // 1. Report Count (up to 30 points, logarithmic scaling)
        double reportCountScore = Math.Min(30.0, Math.Log2(report.ReportCount + 1) * 7.5);
        factors["ReportCountScore"] = Math.Round(reportCountScore, 2);
        score += reportCountScore;

        // 2. Unique Reporters Count (up to 25 points)
        double uniqueReportersScore = Math.Min(25.0, report.UniqueReporterCount * 5.0);
        factors["UniqueReportersScore"] = Math.Round(uniqueReportersScore, 2);
        score += uniqueReportersScore;

        // 3. Verification Status (+25 for verified, -50 for false report)
        double verificationScore = report.VerificationStatus switch
        {
            VerificationStatus.Verified => 25.0,
            VerificationStatus.FalseReport => -50.0,
            _ => 0.0
        };
        factors["VerificationScore"] = verificationScore;
        score += verificationScore;

        // 4. Recency / First seen factor (up to 10 points)
        double ageDays = (DateTime.UtcNow - report.FirstSeenAt).TotalDays;
        double recencyScore = ageDays < 30 ? 10.0 : Math.Max(2.0, 10.0 - (ageDays / 30.0));
        factors["RecencyScore"] = Math.Round(recencyScore, 2);
        score += recencyScore;

        // 5. Severity bonus if primary scam type is identified (+10 points)
        if (report.PrimaryScamType.HasValue)
        {
            score += 10.0;
            factors["ScamTypeAssignedBonus"] = 10.0;
        }

        score = Math.Clamp(score, 0.0, 100.0);

        string explanation = score switch
        {
            >= 75.0 => "Mức độ rủi ro RẤT CAO: Số này có nhiều lượt báo cáo lừa đảo từ nhiều người và đã được xác thực.",
            >= 50.0 => "Mức độ rủi ro CAO: Đã có nhiều phản ánh đáng ngờ. Người dùng cần hết sức cảnh giác.",
            >= 25.0 => "Mức độ rủi ro TRUNG BÌNH: Có báo cáo mới xuất hiện hoặc chưa qua kiểm duyệt đầy đủ.",
            _ => "Mức độ rủi ro THẤP: Chưa ghi nhận nhiều phản ánh tiêu cực hoặc đã được xác minh an toàn."
        };

        return RiskScore.Create(score, factors, explanation);
    }
}
