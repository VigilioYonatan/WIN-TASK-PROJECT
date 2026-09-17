using FluentAssertions;
using FluentValidation.TestHelper;
using TaskManager.Application.DTOs;
using TaskManager.Application.Validators;
using Xunit;

namespace TaskManager.Application.UnitTests.Validators;

public class TaskValidatorTests
{
    private readonly CreateTaskRequestValidator _validator = new();

    [Fact]
    public void Validator_WhenDatesAreInvalid_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateTaskRequest(
            Titulo: "Tarea fechas erradas",
            Descripcion: "Desc",
            FechaInicio: DateTime.UtcNow.AddDays(5),
            FechaCierre: DateTime.UtcNow, // Invalid: FechaCierre < FechaInicio
            CategoriaId: 1
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.FechaCierre)
            .WithErrorMessage("La fecha de cierre no puede ser anterior a la fecha de inicio.");
    }

    [Fact]
    public void Validator_WhenTituloIsEmpty_ShouldHaveValidationError()
    {
        // Arrange
        var request = new CreateTaskRequest(
            Titulo: "",
            Descripcion: "Desc",
            FechaInicio: DateTime.UtcNow,
            FechaCierre: DateTime.UtcNow.AddDays(1),
            CategoriaId: 1
        );

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Titulo)
            .WithErrorMessage("El título de la tarea es requerido.");
    }
}
