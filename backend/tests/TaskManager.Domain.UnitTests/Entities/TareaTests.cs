using FluentAssertions;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;
using Xunit;

namespace TaskManager.Domain.UnitTests.Entities;

public class TareaTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateTarea()
    {
        // Arrange
        var fechaInicio = DateTime.UtcNow;
        var fechaCierre = fechaInicio.AddDays(2);

        // Act
        var tarea = new Tarea(
            titulo: "Implementar endpoints REST",
            descripcion: "CRUD de tareas en ASP.NET Core 8",
            fechaInicio: fechaInicio,
            fechaCierre: fechaCierre,
            categoriaId: 1,
            usuarioId: 10,
            estado: EstadoTarea.Pendiente
        );

        // Assert
        tarea.Titulo.Should().Be("Implementar endpoints REST");
        tarea.Descripcion.Should().Be("CRUD de tareas en ASP.NET Core 8");
        tarea.FechaInicio.Should().Be(fechaInicio);
        tarea.FechaCierre.Should().Be(fechaCierre);
        tarea.CategoriaId.Should().Be(1);
        tarea.UsuarioId.Should().Be(10);
        tarea.Estado.Should().Be(EstadoTarea.Pendiente);
    }

    [Fact]
    public void Constructor_WhenFechaCierreBeforeFechaInicio_ShouldThrowDomainException()
    {
        // Arrange
        var fechaInicio = DateTime.UtcNow.AddDays(5);
        var fechaCierre = DateTime.UtcNow; // Before fechaInicio

        // Act
        Action act = () => new Tarea(
            titulo: "Tarea Inválida",
            descripcion: "Descripción",
            fechaInicio: fechaInicio,
            fechaCierre: fechaCierre,
            categoriaId: 1,
            usuarioId: 1
        );

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*fecha de cierre no puede ser anterior a la fecha de inicio*");
    }

    [Fact]
    public void CambiarEstado_ShouldUpdateStateAndTimestamp()
    {
        // Arrange
        var tarea = new Tarea("Tarea", "Desc", DateTime.UtcNow, DateTime.UtcNow.AddDays(1), 1, 1);

        // Act
        tarea.CambiarEstado(EstadoTarea.Completada);

        // Assert
        tarea.Estado.Should().Be(EstadoTarea.Completada);
        tarea.UpdatedAt.Should().NotBeNull();
    }
}
