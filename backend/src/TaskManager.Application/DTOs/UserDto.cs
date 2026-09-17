namespace TaskManager.Application.DTOs;

public record UserDto(
    int Id,
    string Nombre,
    string Email,
    DateTime CreatedAt
);
