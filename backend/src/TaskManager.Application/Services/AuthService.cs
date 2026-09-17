using FluentValidation;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IValidator<RegisterRequest> _registerValidator;
    private readonly IValidator<LoginRequest> _loginValidator;

    public AuthService(
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<RegisterRequest> registerValidator,
        IValidator<LoginRequest> loginValidator)
    {
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _registerValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        string normalizedEmail = request.Email.Trim().ToLowerInvariant();
        bool exists = await _usuarioRepository.ExistsByEmailAsync(normalizedEmail, cancellationToken);
        if (exists)
        {
            throw new EmailAlreadyExistsException(normalizedEmail);
        }

        string hashedPassword = _passwordHasher.HashPassword(request.Password);
        var usuario = new Usuario(request.Nombre, normalizedEmail, hashedPassword);

        await _usuarioRepository.AddAsync(usuario, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        string token = _jwtTokenGenerator.GenerateToken(usuario);
        var userDto = new UserDto(usuario.Id, usuario.Nombre, usuario.Email, usuario.CreatedAt);

        return new AuthResponseDto(token, userDto, DateTime.UtcNow.AddMinutes(120));
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validationResult = await _loginValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        string normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var usuario = await _usuarioRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (usuario == null)
        {
            throw new DomainException("Credenciales inválidas.");
        }

        bool passwordValid = _passwordHasher.VerifyPassword(request.Password, usuario.PasswordHash);
        if (!passwordValid)
        {
            throw new DomainException("Credenciales inválidas.");
        }

        string token = _jwtTokenGenerator.GenerateToken(usuario);
        var userDto = new UserDto(usuario.Id, usuario.Nombre, usuario.Email, usuario.CreatedAt);

        return new AuthResponseDto(token, userDto, DateTime.UtcNow.AddMinutes(120));
    }
}
