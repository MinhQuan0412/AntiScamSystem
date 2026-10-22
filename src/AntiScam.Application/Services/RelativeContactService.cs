using AntiScam.Application.Common;
using AntiScam.Application.DTOs.RelativeContact;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AntiScam.Application.Services;

public interface IRelativeContactService
{
    Task<Result<List<RelativeContactDto>>> GetUserRelativesAsync(string userId);
    Task<Result<RelativeContactDto>> CreateAsync(CreateRelativeContactRequest request, string userId);
    Task<Result<RelativeContactDto>> UpdateAsync(Guid id, CreateRelativeContactRequest request, string userId);
    Task<Result> DeleteAsync(Guid id, string userId);
}

public class RelativeContactService : IRelativeContactService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RelativeContactService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<List<RelativeContactDto>>> GetUserRelativesAsync(string userId)
    {
        var items = await _unitOfWork.RelativeContacts.Query()
            .Where(r => r.UserId == userId)
            .ToListAsync();

        return Result<List<RelativeContactDto>>.Success(_mapper.Map<List<RelativeContactDto>>(items));
    }

    public async Task<Result<RelativeContactDto>> CreateAsync(CreateRelativeContactRequest request, string userId)
    {
        var entity = new RelativeContact
        {
            UserId = userId,
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            Email = request.Email,
            TelegramChatId = request.TelegramChatId,
            ContactType = request.ContactType,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.RelativeContacts.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return Result<RelativeContactDto>.Success(_mapper.Map<RelativeContactDto>(entity));
    }

    public async Task<Result<RelativeContactDto>> UpdateAsync(Guid id, CreateRelativeContactRequest request, string userId)
    {
        var entity = await _unitOfWork.RelativeContacts.Query()
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (entity == null) return Result<RelativeContactDto>.Failure("Không tìm thấy thông tin liên hệ người thân.");

        entity.Name = request.Name;
        entity.PhoneNumber = request.PhoneNumber;
        entity.Email = request.Email;
        entity.TelegramChatId = request.TelegramChatId;
        entity.ContactType = request.ContactType;
        entity.Notes = request.Notes;
        entity.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.RelativeContacts.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return Result<RelativeContactDto>.Success(_mapper.Map<RelativeContactDto>(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, string userId)
    {
        var entity = await _unitOfWork.RelativeContacts.Query()
            .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

        if (entity == null) return Result.Failure("Không tìm thấy thông tin.");

        _unitOfWork.RelativeContacts.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        return Result.Success();
    }
}
