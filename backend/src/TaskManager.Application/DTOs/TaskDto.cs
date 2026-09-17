using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs;

public record TaskDto(
    int Id,
    string Titulo,
    string Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre,
    EstadoTarea Estado,
    string EstadoDescripcion,
    int CategoriaId,
    string? CategoriaNombre,
    int UsuarioId,
    string? UsuarioNombre,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateTaskRequest(
    string Titulo,
    string Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre,
    int CategoriaId,
    EstadoTarea Estado = EstadoTarea.Pendiente
);

public record UpdateTaskRequest(
    string Titulo,
    string Descripcion,
    DateTime FechaInicio,
    DateTime FechaCierre,
    int CategoriaId,
    EstadoTarea Estado
);
