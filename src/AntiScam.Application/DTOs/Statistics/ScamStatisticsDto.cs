using AntiScam.Application.DTOs.ModelVersion;
using AntiScam.Application.DTOs.PhoneReport;

namespace AntiScam.Application.DTOs.Statistics;

public class MonthlyTrendDto
{
    public string Month { get; set; } = string.Empty;
    public int Count { get; set; }
    public Dictionary<string, int> ByType { get; set; } = new();
}

public class ScamStatisticsDto
{
    public int TotalScamsDetected { get; set; }
    public Dictionary<string, int> TotalByType { get; set; } = new();
    public List<MonthlyTrendDto> TrendByMonth { get; set; } = new();
    public List<PhoneReportDto> TopReportedNumbers { get; set; } = new();
    public List<ModelVersionDto> ModelPerformanceSummary { get; set; } = new();
}
