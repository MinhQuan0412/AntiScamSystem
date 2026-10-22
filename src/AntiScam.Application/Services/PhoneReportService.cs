using AntiScam.Application.Common;
using AntiScam.Application.DTOs.PhoneReport;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Domain.Interfaces;
using AntiScam.Infrastructure.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AntiScam.Application.Services;

public interface IPhoneReportService
{
    Task<Result> ReportAsync(CreatePhoneReportRequest request, string userId);
    Task<Result<PhoneReportLookupResponse>> LookupAsync(string number);
    Task<PagedResult<PhoneReportDetailDto>> GetPendingModerationAsync(int page, int pageSize);
    Task<Result> ModerateAsync(Guid reportDetailId, bool approve, string note, string adminId);
}

public class PhoneReportService : IPhoneReportService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRiskScoreCalculator _riskScoreCalculator;
    private readonly IMapper _mapper;

    public PhoneReportService(
        IUnitOfWork unitOfWork,
        IRiskScoreCalculator riskScoreCalculator,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _riskScoreCalculator = riskScoreCalculator;
        _mapper = mapper;
    }

    public async Task<Result> ReportAsync(CreatePhoneReportRequest request, string userId)
    {
        var cleanNumber = request.ReportedNumber.Trim();
        if (string.IsNullOrWhiteSpace(cleanNumber))
        {
            return Result.Failure("Số điện thoại hoặc số tài khoản không được để trống.");
        }

        // Rate limiting check: user cannot submit more than 5 reports per day
        var today = DateTime.UtcNow.Date;
        var userReportsToday = await _unitOfWork.PhoneReportDetails.Query()
            .CountAsync(d => d.ReportedById == userId && d.CreatedAt >= today);

        if (userReportsToday >= 5)
        {
            return Result.Failure("Bạn đã vượt quá giới hạn 5 báo cáo mỗi ngày để hạn chế spam.");
        }

        var report = await _unitOfWork.PhoneReports.Query()
            .Include(p => p.Details)
            .FirstOrDefaultAsync(p => p.ReportedNumber == cleanNumber && p.NumberType == request.NumberType);

        if (report == null)
        {
            report = new PhoneReport
            {
                ReportedNumber = cleanNumber,
                NumberType = request.NumberType,
                BankName = request.BankName,
                PrimaryScamType = request.ScamType,
                ReportCount = 1,
                UniqueReporterCount = 1,
                FirstSeenAt = DateTime.UtcNow,
                VerificationStatus = VerificationStatus.Unverified,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PhoneReports.AddAsync(report);
        }
        else
        {
            report.ReportCount++;
            var alreadyReported = report.Details.Any(d => d.ReportedById == userId);
            if (!alreadyReported)
            {
                report.UniqueReporterCount++;
            }

            if (request.ScamType.HasValue)
            {
                report.PrimaryScamType = request.ScamType;
            }
        }

        var detail = new PhoneReportDetail
        {
            PhoneReportId = report.Id,
            ReportedById = userId,
            Description = request.Description,
            ScamType = request.ScamType,
            EvidenceUrl = request.EvidenceUrl,
            CreatedAt = DateTime.UtcNow,
            IsModerated = false
        };

        await _unitOfWork.PhoneReportDetails.AddAsync(detail);

        // Calculate and update risk score
        var calculated = _riskScoreCalculator.Calculate(report);
        report.RiskScore = calculated.Score;
        report.RiskFactors = System.Text.Json.JsonSerializer.Serialize(calculated.Factors);

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<PhoneReportLookupResponse>> LookupAsync(string number)
    {
        var cleanNumber = number.Trim();
        var report = await _unitOfWork.PhoneReports.Query()
            .Include(p => p.Details)
                .ThenInclude(d => d.ReportedBy)
            .FirstOrDefaultAsync(p => p.ReportedNumber == cleanNumber);

        if (report == null)
        {
            return Result<PhoneReportLookupResponse>.Success(new PhoneReportLookupResponse
            {
                MaskedNumber = MaskNumber(cleanNumber),
                ReportCount = 0,
                UniqueReporterCount = 0,
                FirstSeenAt = DateTime.UtcNow,
                VerificationStatus = VerificationStatus.Unverified,
                RiskScore = new RiskScoreDto
                {
                    Score = 0,
                    Level = RiskLevel.Low,
                    Explanation = "Số này chưa có ghi nhận phản ánh lừa đảo nào trong hệ thống."
                },
                NeedsAttention = false
            });
        }

        var calculated = _riskScoreCalculator.Calculate(report);

        var response = new PhoneReportLookupResponse
        {
            MaskedNumber = MaskNumber(report.ReportedNumber),
            NumberType = report.NumberType,
            BankName = report.BankName,
            ReportCount = report.ReportCount,
            UniqueReporterCount = report.UniqueReporterCount,
            FirstSeenAt = report.FirstSeenAt,
            VerificationStatus = report.VerificationStatus,
            RiskScore = new RiskScoreDto
            {
                Score = calculated.Score,
                Level = calculated.Level,
                Explanation = calculated.Explanation,
                Factors = calculated.Factors
            },
            NeedsAttention = report.VerificationStatus == VerificationStatus.Unverified && report.ReportCount > 0,
            RecentReports = report.Details
                .OrderByDescending(d => d.CreatedAt)
                .Take(5)
                .Select(d => new PhoneReportDetailDto
                {
                    Id = d.Id,
                    Description = d.Description,
                    ScamType = d.ScamType,
                    CreatedAt = d.CreatedAt,
                    ReporterName = MaskReporter(d.ReportedBy?.FullName ?? "Người dùng")
                }).ToList()
        };

        return Result<PhoneReportLookupResponse>.Success(response);
    }

    public async Task<PagedResult<PhoneReportDetailDto>> GetPendingModerationAsync(int page, int pageSize)
    {
        var query = _unitOfWork.PhoneReportDetails.Query()
            .Where(d => !d.IsModerated)
            .Include(d => d.ReportedBy)
            .AsNoTracking();

        int total = await query.CountAsync();
        var items = await query
            .OrderBy(d => d.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = items.Select(d => new PhoneReportDetailDto
        {
            Id = d.Id,
            PhoneReportId = d.PhoneReportId,
            ReportedById = d.ReportedById,
            ReporterName = d.ReportedBy?.FullName ?? "Unknown",
            Description = d.Description,
            ScamType = d.ScamType,
            EvidenceUrl = d.EvidenceUrl,
            IsModerated = d.IsModerated,
            IsApproved = d.IsApproved,
            CreatedAt = d.CreatedAt
        }).ToList();

        return PagedResult<PhoneReportDetailDto>.Create(dtos, total, page, pageSize);
    }

    public async Task<Result> ModerateAsync(Guid reportDetailId, bool approve, string note, string adminId)
    {
        var detail = await _unitOfWork.PhoneReportDetails.Query()
            .Include(d => d.PhoneReport)
            .FirstOrDefaultAsync(d => d.Id == reportDetailId);

        if (detail == null)
        {
            return Result.Failure("Không tìm thấy báo cáo.");
        }

        detail.IsModerated = true;
        detail.IsApproved = approve;
        detail.ModeratedById = adminId;
        detail.ModeratedAt = DateTime.UtcNow;
        detail.ModerationNote = note;

        if (approve)
        {
            detail.PhoneReport.VerificationStatus = VerificationStatus.Verified;
        }

        var calc = _riskScoreCalculator.Calculate(detail.PhoneReport);
        detail.PhoneReport.RiskScore = calc.Score;

        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }

    private static string MaskNumber(string number)
    {
        if (number.Length <= 4) return number;
        return new string('*', number.Length - 4) + number[^4..];
    }

    private static string MaskReporter(string name)
    {
        if (name.Length <= 2) return name;
        return name[0] + "***" + name[^1];
    }
}
