using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using Xunit;

namespace TaskManager.Domain.UnitTests.Entities;

public class UsuarioTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateUsuario()
    {
        // Arrange & Act
        var usuario = new Usuario("Juan Perez", "juan.perez@rimac.com", "H@shedPassword123");

        // Assert
        usuario.Nombre.Should().Be("Juan Perez");
        usuario.Email.Should().Be("juan.perez@rimac.com");
        usuario.PasswordHash.Should().Be("H@shedPassword123");
        usuario.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_WithEmptyNombre_ShouldThrowDomainException(string? invalidNombre)
    {
        // Act
        Action act = () => new Usuario(invalidNombre!, "test@rimac.com", "Hash123");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*nombre del usuario no puede estar vacío*");
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("no-at-domain.com")]
    [InlineData("at@nodot")]
    public void Constructor_WithInvalidEmail_ShouldThrowDomainException(string invalidEmail)
    {
        // Act
        Action act = () => new Usuario("Test User", invalidEmail, "Hash123");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*formato del correo electrónico es inválido*");
    }
}
