namespace TaskManager.Application.DTOs;

public record RegisterRequest(
    string Nombre,
    string Email,
    string Password
);

public record LoginRequest(
    string Email,
    string Password
);

public record AuthResponseDto(
    string Token,
    UserDto Usuario,
    DateTime ExpiresAt
);
