using FluentAssertions;
using Moq;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;
using TaskManager.Application.Validators;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Interfaces;
using Xunit;

namespace TaskManager.Application.UnitTests.Services;

public class TaskServiceTests
{
    private readonly Mock<ITareaRepository> _tareaRepoMock = new();
    private readonly Mock<ICategoriaRepository> _catRepoMock = new();
    private readonly Mock<IUsuarioRepository> _userRepoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ICurrentUserService> _currentUserMock = new();
    private readonly CreateTaskRequestValidator _createValidator = new();
    private readonly UpdateTaskRequestValidator _updateValidator = new();

    private TaskService CreateService()
    {
        return new TaskService(
            _tareaRepoMock.Object,
            _catRepoMock.Object,
            _userRepoMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _createValidator,
            _updateValidator
        );
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateAndReturnTaskDto()
    {
        // Arrange
        var request = new CreateTaskRequest(
            Titulo: "Documentar arquitectura",
            Descripcion: "Detalles DDD",
            FechaInicio: DateTime.UtcNow,
            FechaCierre: DateTime.UtcNow.AddDays(2),
            CategoriaId: 1
        );

        var categoria = new Categoria("Desarrollo");
        var usuario = new Usuario("Maria", "maria@rimac.com", "hash");

        _catRepoMock.Setup(c => c.GetByIdAsync(1, default)).ReturnsAsync(categoria);
        _currentUserMock.Setup(u => u.UserId).Returns(5);
        _userRepoMock.Setup(u => u.GetByIdAsync(5, default)).ReturnsAsync(usuario);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Titulo.Should().Be("Documentar arquitectura");
        _tareaRepoMock.Verify(t => t.AddAsync(It.IsAny<Tarea>(), default), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenCategoryNotFound_ShouldThrowEntityNotFoundException()
    {
        // Arrange
        var request = new CreateTaskRequest(
            Titulo: "Tarea sin categoría",
            Descripcion: "Desc",
            FechaInicio: DateTime.UtcNow,
            FechaCierre: DateTime.UtcNow.AddDays(1),
            CategoriaId: 99
        );

        _catRepoMock.Setup(c => c.GetByIdAsync(99, default)).ReturnsAsync((Categoria?)null);

        var service = CreateService();

        // Act
        Func<Task> act = async () => await service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task CreateAsync_WhenUserNotAuthenticated_ShouldThrowDomainException()
    {
        // Arrange
        var request = new CreateTaskRequest(
            Titulo: "Tarea sin usuario",
            Descripcion: "Desc",
            FechaInicio: DateTime.UtcNow,
            FechaCierre: DateTime.UtcNow.AddDays(1),
            CategoriaId: 1
        );

        var categoria = new Categoria("General");
        _catRepoMock.Setup(c => c.GetByIdAsync(1, default)).ReturnsAsync(categoria);
        _currentUserMock.Setup(u => u.UserId).Returns((int?)null); // Not authenticated

        var service = CreateService();

        // Act
        Func<Task> act = async () => await service.CreateAsync(request);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*Usuario no autenticado*");
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskBelongsToAnotherUser_ShouldThrowDomainException()
    {
        // Arrange
        var existingTask = new Tarea(
            titulo: "Tarea de otro usuario",
            descripcion: "Desc",
            fechaInicio: DateTime.UtcNow,
            fechaCierre: DateTime.UtcNow.AddDays(2),
            categoriaId: 1,
            usuarioId: 999 // Owner is user 999
        );

        _tareaRepoMock.Setup(r => r.GetByIdAsync(50, default)).ReturnsAsync(existingTask);
        _currentUserMock.Setup(u => u.UserId).Returns(10); // Authenticated user is 10

        var updateRequest = new UpdateTaskRequest(
            Titulo: "Intento de hackeo de tarea",
            Descripcion: "Modificado",
            FechaInicio: DateTime.UtcNow,
            FechaCierre: DateTime.UtcNow.AddDays(1),
            CategoriaId: 1,
            Estado: EstadoTarea.Completada
        );

        var service = CreateService();

        // Act
        Func<Task> act = async () => await service.UpdateAsync(50, updateRequest);

        // Assert
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage("*No tiene permisos para modificar esta tarea perteneciente a otro usuario*");
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterByCurrentUserIdByDefault()
    {
        // Arrange
        _currentUserMock.Setup(u => u.UserId).Returns(7);
        _tareaRepoMock.Setup(r => r.GetAllAsync(7, null, null, default))
            .ReturnsAsync(new List<Tarea>());

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync();

        // Assert
        result.Should().NotBeNull();
        _tareaRepoMock.Verify(r => r.GetAllAsync(7, null, null, default), Times.Once);
    }
}
