using AntiScam.Application.DTOs.Auth;
using AntiScam.Application.Services;
using AntiScam.Domain.Entities;
using AntiScam.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace AntiScam.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
        _signInManagerMock = new Mock<SignInManager<User>>(
            _userManagerMock.Object,
            contextAccessorMock.Object,
            userClaimsPrincipalFactoryMock.Object,
            null!, null!, null!, null!);

        _jwtServiceMock = new Mock<IJwtService>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _jwtServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_EmailAlreadyExists_ReturnsFailure()
    {
        var existingUser = new User { Email = "test@example.com" };
        _userManagerMock.Setup(m => m.FindByEmailAsync("test@example.com"))
            .ReturnsAsync(existingUser);

        var request = new RegisterRequest
        {
            Email = "test@example.com",
            Password = "Password123!",
            FullName = "Nguyen Van A"
        };

        var result = await _authService.RegisterAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Email này đã được sử dụng");
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsTokens()
    {
        var user = new User
        {
            Id = Guid.NewGuid().ToString(),
            Email = "user@test.com",
            FullName = "Nguyen Van B",
            IsActive = true
        };

        _userManagerMock.Setup(m => m.FindByEmailAsync("user@test.com"))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, "Password123!", false))
            .ReturnsAsync(SignInResult.Success);

        _userManagerMock.Setup(m => m.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "User" });

        _jwtServiceMock.Setup(j => j.GenerateAccessToken(user, It.IsAny<IList<string>>()))
            .Returns("fake-jwt-token");

        _jwtServiceMock.Setup(j => j.GenerateRefreshToken())
            .Returns("fake-refresh-token");

        var request = new LoginRequest { Email = "user@test.com", Password = "Password123!" };

        var result = await _authService.LoginAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AccessToken.Should().Be("fake-jwt-token");
        result.Value.RefreshToken.Should().Be("fake-refresh-token");
    }

    [Fact]
    public async Task LoginAsync_WrongPassword_ReturnsFailure()
    {
        var user = new User { Email = "user@test.com", IsActive = true };

        _userManagerMock.Setup(m => m.FindByEmailAsync("user@test.com"))
            .ReturnsAsync(user);

        _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(user, "WrongPass", false))
            .ReturnsAsync(SignInResult.Failed);

        var request = new LoginRequest { Email = "user@test.com", Password = "WrongPass" };

        var result = await _authService.LoginAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Email hoặc mật khẩu không chính xác");
    }
}
