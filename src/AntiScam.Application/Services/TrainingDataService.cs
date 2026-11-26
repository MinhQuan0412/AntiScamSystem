using AntiScam.Application.Common;
using AntiScam.Application.DTOs.TrainingData;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AntiScam.Application.Services;

public interface ITrainingDataService
{
    Task<PagedResult<TrainingDataSampleDto>> GetAllAsync(int page, int pageSize, string? batch);
    Task<Result<TrainingDataSampleDto>> AddSampleAsync(CreateSampleRequest request, string adminId);
    Task<Result> LabelSampleAsync(Guid id, LabelSampleRequest request, string adminId);
    Task<Result> DeleteSampleAsync(Guid id);
}

public class TrainingDataService : ITrainingDataService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TrainingDataService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<TrainingDataSampleDto>> GetAllAsync(int page, int pageSize, string? batch)
    {
        var query = _unitOfWork.TrainingDataSamples.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(batch))
        {
            query = query.Where(s => s.BatchId == batch);
        }

        int total = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return PagedResult<TrainingDataSampleDto>.Create(_mapper.Map<List<TrainingDataSampleDto>>(items), total, page, pageSize);
    }

    public async Task<Result<TrainingDataSampleDto>> AddSampleAsync(CreateSampleRequest request, string adminId)
    {
        var entity = new TrainingDataSample
        {
            Content = request.Content,
            IsScam = request.IsScam,
            ScamType = request.ScamType,
            BatchId = request.DataBatch ?? "batch-manual",
            Source = request.Source ?? "Quản trị viên nhập",
            LabeledById = adminId,
            VerifiedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _unitOfWork.TrainingDataSamples.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return Result<TrainingDataSampleDto>.Success(_mapper.Map<TrainingDataSampleDto>(entity));
    }

    public async Task<Result> LabelSampleAsync(Guid id, LabelSampleRequest request, string adminId)
    {
        var sample = await _unitOfWork.TrainingDataSamples.GetByIdAsync(id);
        if (sample == null) return Result.Failure("Không tìm thấy mẫu dữ liệu.");

        sample.IsScam = request.IsScam;
        sample.ScamType = request.ScamType;
        sample.LabeledById = adminId;
        sample.VerifiedAt = DateTime.UtcNow;
        sample.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.TrainingDataSamples.Update(sample);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result> DeleteSampleAsync(Guid id)
    {
        var sample = await _unitOfWork.TrainingDataSamples.GetByIdAsync(id);
        if (sample == null) return Result.Failure("Không tìm thấy mẫu.");

        _unitOfWork.TrainingDataSamples.Delete(sample);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
