using AntiScam.Application.Common;
using AntiScam.Application.DTOs.EmergencyContact;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AntiScam.Application.Services;

public interface IEmergencyContactService
{
    Task<Result<List<EmergencyContactDto>>> GetByProvinceAsync(string? provinceCode);
    Task<Result<List<EmergencyContactDto>>> GetNationalAsync();
    Task<Result<EmergencyContactDto>> CreateAsync(CreateEmergencyContactRequest request);
    Task<Result<EmergencyContactDto>> UpdateAsync(Guid id, CreateEmergencyContactRequest request);
    Task<Result> DeleteAsync(Guid id);
}

public class EmergencyContactService : IEmergencyContactService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public EmergencyContactService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<EmergencyContactDto>>> GetByProvinceAsync(string? provinceCode)
    {
        var query = _unitOfWork.EmergencyContacts.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(provinceCode))
        {
            query = query.Where(c => c.ProvinceCode == provinceCode || c.IsNational);
        }

        var items = await query.OrderByDescending(c => c.IsNational).ThenBy(c => c.Name).ToListAsync();
        return Result<List<EmergencyContactDto>>.Success(_mapper.Map<List<EmergencyContactDto>>(items));
    }

    public async Task<Result<List<EmergencyContactDto>>> GetNationalAsync()
    {
        var items = await _unitOfWork.EmergencyContacts.Query()
            .Where(c => c.IsNational)
            .AsNoTracking()
            .ToListAsync();

        return Result<List<EmergencyContactDto>>.Success(_mapper.Map<List<EmergencyContactDto>>(items));
    }

    public async Task<Result<EmergencyContactDto>> CreateAsync(CreateEmergencyContactRequest request)
    {
        var entity = new EmergencyContact
        {
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Category = request.Category,
            ProvinceCode = request.ProvinceCode,
            ProvinceName = request.ProvinceName,
            Description = request.Description,
            IsNational = request.IsNational,
            VerificationSource = request.VerificationSource,
            VerifiedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.EmergencyContacts.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return Result<EmergencyContactDto>.Success(_mapper.Map<EmergencyContactDto>(entity));
    }

    public async Task<Result<EmergencyContactDto>> UpdateAsync(Guid id, CreateEmergencyContactRequest request)
    {
        var entity = await _unitOfWork.EmergencyContacts.GetByIdAsync(id);
        if (entity == null) return Result<EmergencyContactDto>.Failure("Không tìm thấy số khẩn cấp.");

        entity.Name = request.Name;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Category = request.Category;
        entity.ProvinceCode = request.ProvinceCode;
        entity.ProvinceName = request.ProvinceName;
        entity.Description = request.Description;
        entity.IsNational = request.IsNational;
        entity.VerificationSource = request.VerificationSource;
        entity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.EmergencyContacts.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return Result<EmergencyContactDto>.Success(_mapper.Map<EmergencyContactDto>(entity));
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var entity = await _unitOfWork.EmergencyContacts.GetByIdAsync(id);
        if (entity == null) return Result.Failure("Không tìm thấy số khẩn cấp.");

        _unitOfWork.EmergencyContacts.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}
