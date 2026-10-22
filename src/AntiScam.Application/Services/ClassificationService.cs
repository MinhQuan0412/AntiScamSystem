using AntiScam.Application.Common;
using AntiScam.Application.DTOs.Classification;
using AntiScam.Application.DTOs.ModelVersion;
using AntiScam.Application.DTOs.ScamScenario;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Domain.Interfaces;
using AntiScam.Infrastructure.Services;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AntiScam.Application.Services;

public interface IClassificationService
{
    Task<Result<ClassifyMessageResponse>> ClassifyAsync(ClassifyMessageRequest request, string? userId);
    Task<Result<List<ModelVersionDto>>> GetModelVersionsAsync();
    Task<Result<string>> TriggerRetrainingAsync(string dataBatch, ModelType modelType, string adminId);
    Task<Result<TrainingStatusDto>> GetTrainingStatusAsync(string jobId);
}

public class ClassificationService : IClassificationService
{
    private readonly IMlClassificationService _mlService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ClassificationService(
        IMlClassificationService mlService,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _mlService = mlService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ClassifyMessageResponse>> ClassifyAsync(ClassifyMessageRequest request, string? userId)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return Result<ClassifyMessageResponse>.Failure("Nội dung kiểm tra không được để trống.");
        }

        var mlResult = await _mlService.PredictAsync(new ClassificationRequest(request.Content));

        // If user consented, anonymize and persist for future model retraining
        if (request.ConsentToStore)
        {
            var sample = new TrainingDataSample
            {
                Content = AnonymizeText(request.Content),
                IsScam = mlResult.IsScam,
                ScamType = mlResult.ScamType,
                BatchId = "community-contributed",
                Source = "Người dùng đóng góp (đã đồng ý)",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _unitOfWork.TrainingDataSamples.AddAsync(sample);
            await _unitOfWork.SaveChangesAsync();
        }

        // Fetch related scenarios if a scam type is identified
        var relatedScenarios = new List<ScamScenarioDto>();
        if (mlResult.ScamType.HasValue)
        {
            var scenarios = await _unitOfWork.ScamScenarios.Query()
                .Where(s => s.ScamType == mlResult.ScamType.Value)
                .Take(3)
                .ToListAsync();

            relatedScenarios = _mapper.Map<List<ScamScenarioDto>>(scenarios);
        }

        var response = new ClassifyMessageResponse
        {
            IsScam = mlResult.IsScam,
            ScamType = mlResult.ScamType,
            ScamTypeName = mlResult.ScamType.HasValue ? mlResult.ScamType.Value.ToString() : string.Empty,
            Confidence = mlResult.Confidence,
            MatchedSignals = mlResult.MatchedSignals,
            PreventionAdvice = mlResult.Advice,
            RelatedScenarios = relatedScenarios,
            Disclaimer = "Kết quả dự đoán bằng mô hình học máy chỉ mang tính chất tham khảo, cảnh báo ban đầu và không thay thế việc kiểm chứng với cơ quan chức năng hoặc ngân hàng."
        };

        return Result<ClassifyMessageResponse>.Success(response);
    }

    public async Task<Result<List<ModelVersionDto>>> GetModelVersionsAsync()
    {
        var versions = await _unitOfWork.ModelVersions.Query()
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

        return Result<List<ModelVersionDto>>.Success(_mapper.Map<List<ModelVersionDto>>(versions));
    }

    public async Task<Result<string>> TriggerRetrainingAsync(string dataBatch, ModelType modelType, string adminId)
    {
        var res = await _mlService.TriggerTrainingAsync(new TrainingRequest(dataBatch, modelType.ToString()));
        return Result<string>.Success(res.JobId);
    }

    public async Task<Result<TrainingStatusDto>> GetTrainingStatusAsync(string jobId)
    {
        var res = await _mlService.GetTrainingStatusAsync(jobId);
        if (res == null)
        {
            return Result<TrainingStatusDto>.Failure("Không tìm thấy thông tin tiến trình huấn luyện.");
        }

        return Result<TrainingStatusDto>.Success(new TrainingStatusDto
        {
            JobId = res.JobId,
            Status = res.Status,
            Progress = res.Status == "completed" ? 100 : 50
        });
    }

    private static string AnonymizeText(string text)
    {
        // Simple regex masking for phone numbers and bank accounts
        var masked = System.Text.RegularExpressions.Regex.Replace(text, @"(0[3|5|7|8|9][0-9]{8})", "***PHONE***");
        masked = System.Text.RegularExpressions.Regex.Replace(masked, @"(\d{9,16})", "***ACCOUNT***");
        return masked;
    }
}
