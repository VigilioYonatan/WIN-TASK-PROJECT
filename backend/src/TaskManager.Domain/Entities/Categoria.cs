using TaskManager.Domain.Common;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

public class Categoria : BaseEntity<int>, IAggregateRoot
{
    public string Nombre { get; private set; } = string.Empty;

    // Navigation collection
    private readonly List<Tarea> _tareas = new();
    public IReadOnlyCollection<Tarea> Tareas => _tareas.AsReadOnly();

    protected Categoria() { }

    public Categoria(string nombre)
    {
        SetNombre(nombre);
        CreatedAt = DateTime.UtcNow;
    }

    public void SetNombre(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new DomainException("El nombre de la categoría no puede estar vacío.");

        if (nombre.Trim().Length > 80)
            throw new DomainException("El nombre de la categoría no puede exceder los 80 caracteres.");

        Nombre = nombre.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
