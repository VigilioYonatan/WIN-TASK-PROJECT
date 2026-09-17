namespace TaskManager.Application.DTOs;

public record CategoryDto(
    int Id,
    string Nombre,
    DateTime CreatedAt,
    int TotalTasks = 0
);

public record CreateCategoryRequest(string Nombre);

public record UpdateCategoryRequest(string Nombre);
