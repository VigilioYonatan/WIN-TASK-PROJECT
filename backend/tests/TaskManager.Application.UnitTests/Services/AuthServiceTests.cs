using FluentAssertions;
using Moq;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;
using TaskManager.Application.Validators;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Interfaces;
using Xunit;

namespace TaskManager.Application.UnitTests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUsuarioRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IPasswordHasher> _hasherMock = new();
    private readonly Mock<IJwtTokenGenerator> _jwtMock = new();
    private readonly RegisterRequestValidator _registerValidator = new();
    private readonly LoginRequestValidator _loginValidator = new();

    private AuthService CreateService()
    {
        return new AuthService(
            _userRepoMock.Object,
            _unitOfWorkMock.Object,
            _hasherMock.Object,
            _jwtMock.Object,
            _registerValidator,
            _loginValidator
        );
    }

    [Fact]
    public async Task RegisterAsync_WithNewEmail_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest("Carlos Rivas", "carlos@rimac.com", "Secret123!");
        _userRepoMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), default))
            .ReturnsAsync(false);
        _hasherMock.Setup(h => h.HashPassword(request.Password))
            .Returns("Hashed_Secret123!");
        _jwtMock.Setup(j => j.GenerateToken(It.IsAny<Usuario>()))
            .Returns("sample.jwt.token");

        var service = CreateService();

        // Act
        var result = await service.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("sample.jwt.token");
        result.Usuario.Email.Should().Be("carlos@rimac.com");
        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<Usuario>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ShouldThrowEmailAlreadyExistsException()
    {
        // Arrange
        var request = new RegisterRequest("Carlos Rivas", "carlos@rimac.com", "Secret123!");
        _userRepoMock.Setup(r => r.ExistsByEmailAsync("carlos@rimac.com", default))
            .ReturnsAsync(true);

        var service = CreateService();

        // Act
        Func<Task> act = async () => await service.RegisterAsync(request);

        // Assert
        await act.Should().ThrowAsync<EmailAlreadyExistsException>();
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new LoginRequest("carlos@rimac.com", "Secret123!");
        var user = new Usuario("Carlos Rivas", "carlos@rimac.com", "Hashed_Secret123!");

        _userRepoMock.Setup(r => r.GetByEmailAsync("carlos@rimac.com", default))
            .ReturnsAsync(user);
        _hasherMock.Setup(h => h.VerifyPassword("Secret123!", "Hashed_Secret123!"))
            .Returns(true);
        _jwtMock.Setup(j => j.GenerateToken(user))
            .Returns("token.valido");

        var service = CreateService();

        // Act
        var result = await service.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("token.valido");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldThrowDomainException()
    {
        // Arrange
        var request = new LoginRequest("carlos@rimac.com", "WrongPassword");
        var user = new Usuario("Carlos Rivas", "carlos@rimac.com", "Hashed_Secret123!");

        _userRepoMock.Setup(r => r.GetByEmailAsync("carlos@rimac.com", default))
            .ReturnsAsync(user);
        _hasherMock.Setup(h => h.VerifyPassword("WrongPassword", "Hashed_Secret123!"))
            .Returns(false);

        var service = CreateService();

        // Act
        Func<Task> act = async () => await service.LoginAsync(request);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Credenciales inválidas*");
    }
}
