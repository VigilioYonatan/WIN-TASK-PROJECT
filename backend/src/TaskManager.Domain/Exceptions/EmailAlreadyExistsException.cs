namespace TaskManager.Domain.Exceptions;

public class EmailAlreadyExistsException : DomainException
{
    public string Email { get; }

    public EmailAlreadyExistsException(string email)
        : base($"El correo electrónico '{email}' ya se encuentra registrado.")
    {
        Email = email;
    }
}
