using TaskManager.Domain.Common;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

public class Usuario : BaseEntity<int>, IAggregateRoot
{
    public string Nombre { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    // Navigation collection
    private readonly List<Tarea> _tareas = new();
    public IReadOnlyCollection<Tarea> Tareas => _tareas.AsReadOnly();

    // Required by EF Core
    protected Usuario() { }

    public Usuario(string nombre, string email, string passwordHash)
    {
        SetNombre(nombre);
        SetEmail(email);
        SetPasswordHash(passwordHash);
        CreatedAt = DateTime.UtcNow;
    }

    public void SetNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre del usuario no puede estar vacío.");

        if (nombre.Trim().Length > 100)
            throw new DomainException("El nombre no puede exceder los 100 caracteres.");

        Nombre = nombre.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DomainException("El correo electrónico es requerido.");

        string trimmedEmail = email.Trim().ToLowerInvariant();
        if (!trimmedEmail.Contains('@') || !trimmedEmail.Contains('.'))
            throw new DomainException("El formato del correo electrónico es inválido.");

        Email = trimmedEmail;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new DomainException("El hash de la contraseña es requerido.");

        PasswordHash = passwordHash;
        UpdatedAt = DateTime.UtcNow;
    }
}
