using FluentValidation;
using TaskManager.Application.DTOs;

namespace TaskManager.Application.Validators;

public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryRequestValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la categoría es requerido.")
            .MaximumLength(80).WithMessage("El nombre no debe exceder los 80 caracteres.");
    }
}

public class UpdateCategoryRequestValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre de la categoría es requerido.")
            .MaximumLength(80).WithMessage("El nombre no debe exceder los 80 caracteres.");
    }
}

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título de la tarea es requerido.")
            .MaximumLength(150).WithMessage("El título no debe exceder los 150 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(1000).WithMessage("La descripción no debe superar los 1000 caracteres.");

        RuleFor(x => x.FechaInicio)
            .NotEmpty().WithMessage("La fecha de inicio es requerida.");

        RuleFor(x => x.FechaCierre)
            .NotEmpty().WithMessage("La fecha de cierre es requerida.")
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("La fecha de cierre no puede ser anterior a la fecha de inicio.");

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("Debe seleccionar una categoría válida.");
    }
}

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Titulo)
            .NotEmpty().WithMessage("El título de la tarea es requerido.")
            .MaximumLength(150).WithMessage("El título no debe exceder los 150 caracteres.");

        RuleFor(x => x.Descripcion)
            .MaximumLength(1000).WithMessage("La descripción no debe superar los 1000 caracteres.");

        RuleFor(x => x.FechaInicio)
            .NotEmpty().WithMessage("La fecha de inicio es requerida.");

        RuleFor(x => x.FechaCierre)
            .NotEmpty().WithMessage("La fecha de cierre es requerida.")
            .GreaterThanOrEqualTo(x => x.FechaInicio)
            .WithMessage("La fecha de cierre no puede ser anterior a la fecha de inicio.");

        RuleFor(x => x.CategoriaId)
            .GreaterThan(0).WithMessage("Debe seleccionar una categoría válida.");
    }
}
