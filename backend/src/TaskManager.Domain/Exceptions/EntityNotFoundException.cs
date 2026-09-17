namespace TaskManager.Domain.Exceptions;

public class EntityNotFoundException : DomainException
{
    public string EntityName { get; }
    public object Key { get; }

    public EntityNotFoundException(string entityName, object key)
        : base($"La entidad '{entityName}' con el identificador '{key}' no fue encontrada.")
    {
        EntityName = entityName;
        Key = key;
    }
}
