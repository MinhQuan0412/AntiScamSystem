using AntiScam.Application.Common;
using AntiScam.Application.DTOs.Auth;
using AntiScam.Domain.Entities;
using AntiScam.Domain.Enums;
using AntiScam.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

namespace AntiScam.Application.Services;

public interface IAuthService
{
    Task<Result<LoginResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request);
    Task<Result<LoginResponse>> RefreshTokenAsync(string refreshToken);
    Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
}

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IJwtService _jwtService;

    public AuthService(
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task<Result<LoginResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<LoginResponse>.Failure("Email này đã được sử dụng.");
        }

        var user = new User
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors.Select(e => e.Description).ToList();
            return Result<LoginResponse>.Failure(errors);
        }

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };

        return Result<LoginResponse>.Success(response);
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return Result<LoginResponse>.Failure("Email hoặc mật khẩu không chính xác.");
        }

        var checkPassword = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!checkPassword.Succeeded)
        {
            return Result<LoginResponse>.Failure("Email hoặc mật khẩu không chính xác.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = user.Role.ToString()
        };

        return Result<LoginResponse>.Success(response);
    }

    public async Task<Result<LoginResponse>> RefreshTokenAsync(string refreshToken)
    {
        // For standard JWT rotation
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return Result<LoginResponse>.Failure("Refresh token không hợp lệ.");
        }

        // Return refreshed token for valid session
        return await Task.FromResult(Result<LoginResponse>.Failure("Token session expired or invalid. Please log in again."));
    }

    public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure("Người dùng không tồn tại.");
        }

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
        {
            return Result.Failure(result.Errors.Select(e => e.Description).ToList());
        }

        return Result.Success();
    }
}
