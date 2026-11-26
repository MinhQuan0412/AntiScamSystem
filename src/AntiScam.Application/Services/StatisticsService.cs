using AntiScam.Application.Common;
using AntiScam.Application.DTOs.ModelVersion;
using AntiScam.Application.DTOs.PhoneReport;
using AntiScam.Application.DTOs.Statistics;
using AntiScam.Domain.Interfaces;
using AntiScam.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AntiScam.Application.Services;

public interface IStatisticsService
{
    Task<Result<ScamStatisticsDto>> GetScamStatisticsAsync(DateTime from, DateTime to);
    Task<Result<List<ModelVersionDto>>> GetModelPerformanceAsync();
    Task<Result<byte[]>> ExportToExcelAsync(DateTime from, DateTime to);
    Task<Result<byte[]>> ExportToPdfAsync(DateTime from, DateTime to);
}

public class StatisticsService : IStatisticsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IReportExportService _exportService;
    private readonly IMapper _mapper;

    public StatisticsService(
        IUnitOfWork unitOfWork,
        IReportExportService exportService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _exportService = exportService;
        _mapper = mapper;
    }

    public async Task<Result<ScamStatisticsDto>> GetScamStatisticsAsync(DateTime from, DateTime to)
    {
        var phoneReports = await _unitOfWork.PhoneReports.Query()
            .Where(r => r.CreatedAt >= from && r.CreatedAt <= to)
            .OrderByDescending(r => r.RiskScore)
            .Take(10)
            .ToListAsync();

        var details = await _unitOfWork.PhoneReportDetails.Query()
            .Where(d => d.CreatedAt >= from && d.CreatedAt <= to && d.ScamType.HasValue)
            .ToListAsync();

        var totalByType = details
            .GroupBy(d => d.ScamType!.Value.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        var modelVersions = await _unitOfWork.ModelVersions.Query()
            .OrderByDescending(m => m.CreatedAt)
            .Take(5)
            .ToListAsync();

        var stats = new ScamStatisticsDto
        {
            TotalScamsDetected = details.Count,
            TotalByType = totalByType,
            TopReportedNumbers = _mapper.Map<List<PhoneReportDto>>(phoneReports),
            ModelPerformanceSummary = _mapper.Map<List<ModelVersionDto>>(modelVersions)
        };

        return Result<ScamStatisticsDto>.Success(stats);
    }

    public async Task<Result<List<ModelVersionDto>>> GetModelPerformanceAsync()
    {
        var versions = await _unitOfWork.ModelVersions.Query()
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return Result<List<ModelVersionDto>>.Success(_mapper.Map<List<ModelVersionDto>>(versions));
    }

    public async Task<Result<byte[]>> ExportToExcelAsync(DateTime from, DateTime to)
    {
        var bytes = await _exportService.ExportScamStatisticsToExcelAsync(from, to);
        return Result<byte[]>.Success(bytes);
    }

    public async Task<Result<byte[]>> ExportToPdfAsync(DateTime from, DateTime to)
    {
        var bytes = await _exportService.ExportScamStatisticsToPdfAsync(from, to);
        return Result<byte[]>.Success(bytes);
    }
}
