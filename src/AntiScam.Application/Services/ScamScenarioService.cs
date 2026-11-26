using AntiScam.Application.Common;
using AntiScam.Application.DTOs.ScamScenario;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AntiScam.Application.Services;

public interface IScamScenarioService
{
    Task<PagedResult<ScamScenarioDto>> GetAllAsync(int page, int pageSize, ScamType? type, ScamStatus? status);
    Task<Result<ScamScenarioDto>> GetByIdAsync(Guid id);
    Task<Result<ScamScenarioDto>> CreateAsync(CreateScamScenarioRequest request, string adminId);
    Task<Result<ScamScenarioDto>> UpdateAsync(Guid id, UpdateScamScenarioRequest request, string adminId);
    Task<Result> DeleteAsync(Guid id, string adminId);
    Task<Result<List<ScamScenarioDto>>> GetVariantsAsync(Guid parentId);
    Task<Result<List<ScamScenarioDto>>> SearchAsync(string query);
}

public class ScamScenarioService : IScamScenarioService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ScamScenarioService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PagedResult<ScamScenarioDto>> GetAllAsync(int page, int pageSize, ScamType? type, ScamStatus? status)
    {
        var query = _unitOfWork.ScamScenarios.Query()
            .Include(s => s.RelatedScenarios)
            .AsNoTracking();

        if (type.HasValue)
        {
            query = query.Where(s => s.ScamType == type.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        int total = await query.CountAsync();
        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = _mapper.Map<List<ScamScenarioDto>>(items);
        return PagedResult<ScamScenarioDto>.Create(dtos, total, page, pageSize);
    }

    public async Task<Result<ScamScenarioDto>> GetByIdAsync(Guid id)
    {
        var item = await _unitOfWork.ScamScenarios.Query()
            .Include(s => s.RelatedScenarios)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (item == null)
        {
            return Result<ScamScenarioDto>.Failure("Không tìm thấy kịch bản lừa đảo.");
        }

        return Result<ScamScenarioDto>.Success(_mapper.Map<ScamScenarioDto>(item));
    }

    public async Task<Result<ScamScenarioDto>> CreateAsync(CreateScamScenarioRequest request, string adminId)
    {
        var scenario = new ScamScenario
        {
            Title = request.Title,
            ScamType = request.ScamType,
            Description = request.Description,
            RecognitionSigns = string.Join("; ", request.RecognitionSigns),
            AnonymizedExample = request.AnonymizedExample,
            PreventionAdvice = request.PreventionAdvice,
            ParentScenarioId = request.ParentScenarioId,
            CreatedById = adminId,
            CreatedAt = DateTime.UtcNow,
            Status = ScamStatus.Active
        };

        await _unitOfWork.ScamScenarios.AddAsync(scenario);
        await _unitOfWork.SaveChangesAsync();

        return Result<ScamScenarioDto>.Success(_mapper.Map<ScamScenarioDto>(scenario));
    }

    public async Task<Result<ScamScenarioDto>> UpdateAsync(Guid id, UpdateScamScenarioRequest request, string adminId)
    {
        var item = await _unitOfWork.ScamScenarios.GetByIdAsync(id);
        if (item == null)
        {
            return Result<ScamScenarioDto>.Failure("Không tìm thấy kịch bản.");
        }

        item.Title = request.Title;
        item.ScamType = request.ScamType;
        item.Description = request.Description;
        item.RecognitionSigns = string.Join("; ", request.RecognitionSigns);
        item.AnonymizedExample = request.AnonymizedExample;
        item.PreventionAdvice = request.PreventionAdvice;
        item.Status = request.Status;
        item.ParentScenarioId = request.ParentScenarioId;
        item.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.ScamScenarios.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return Result<ScamScenarioDto>.Success(_mapper.Map<ScamScenarioDto>(item));
    }

    public async Task<Result> DeleteAsync(Guid id, string adminId)
    {
        var item = await _unitOfWork.ScamScenarios.GetByIdAsync(id);
        if (item == null)
        {
            return Result.Failure("Không tìm thấy kịch bản.");
        }

        item.SoftDelete(adminId);
        _unitOfWork.ScamScenarios.Update(item);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }

    public async Task<Result<List<ScamScenarioDto>>> GetVariantsAsync(Guid parentId)
    {
        var list = await _unitOfWork.ScamScenarios.Query()
            .Where(s => s.ParentScenarioId == parentId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return Result<List<ScamScenarioDto>>.Success(_mapper.Map<List<ScamScenarioDto>>(list));
    }

    public async Task<Result<List<ScamScenarioDto>>> SearchAsync(string query)
    {
        var q = query.ToLowerInvariant();
        var list = await _unitOfWork.ScamScenarios.Query()
            .Where(s => s.Title.ToLower().Contains(q) || s.Description.ToLower().Contains(q) || s.RecognitionSigns.ToLower().Contains(q))
            .Take(20)
            .ToListAsync();

        return Result<List<ScamScenarioDto>>.Success(_mapper.Map<List<ScamScenarioDto>>(list));
    }
}
