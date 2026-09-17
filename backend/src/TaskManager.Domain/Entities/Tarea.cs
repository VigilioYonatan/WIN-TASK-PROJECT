using TaskManager.Domain.Common;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;

namespace TaskManager.Domain.Entities;

public class Tarea : BaseEntity<int>, IAggregateRoot
{
    public string Titulo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaCierre { get; private set; }
    public EstadoTarea Estado { get; private set; }
    public int CategoriaId { get; private set; }
    public int UsuarioId { get; private set; }

    // Navigation properties
    public Categoria? Categoria { get; private set; }
    public Usuario? Usuario { get; private set; }

    protected Tarea() { }

    public Tarea(
        string titulo,
        string descripcion,
        DateTime fechaInicio,
        DateTime fechaCierre,
        int categoriaId,
        int usuarioId,
        EstadoTarea estado = EstadoTarea.Pendiente)
    {
        SetTitulo(titulo);
        SetDescripcion(descripcion);
        SetFechas(fechaInicio, fechaCierre);
        SetCategoriaId(categoriaId);
        SetUsuarioId(usuarioId);
        Estado = estado;
        CreatedAt = DateTime.UtcNow;
    }

    public void Update(
        string titulo,
        string descripcion,
        DateTime fechaInicio,
        DateTime fechaCierre,
        int categoriaId,
        EstadoTarea estado)
    {
        SetTitulo(titulo);
        SetDescripcion(descripcion);
        SetFechas(fechaInicio, fechaCierre);
        SetCategoriaId(categoriaId);
        Estado = estado;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTitulo(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new DomainException("El título de la tarea es requerido.");

        if (titulo.Trim().Length > 150)
            throw new DomainException("El título de la tarea no puede superar los 150 caracteres.");

        Titulo = titulo.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDescripcion(string descripcion)
    {
        Descripcion = descripcion?.Trim() ?? string.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFechas(DateTime fechaInicio, DateTime fechaCierre)
    {
        if (fechaCierre < fechaInicio)
            throw new DomainException("La fecha de cierre no puede ser anterior a la fecha de inicio.");

        FechaInicio = fechaInicio;
        FechaCierre = fechaCierre;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CambiarEstado(EstadoTarea nuevoEstado)
    {
        Estado = nuevoEstado;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetCategoriaId(int categoriaId)
    {
        if (categoriaId <= 0)
            throw new DomainException("El identificador de categoría debe ser mayor a 0.");

        CategoriaId = categoriaId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetUsuarioId(int usuarioId)
    {
        if (usuarioId <= 0)
            throw new DomainException("El identificador de usuario debe ser mayor a 0.");

        UsuarioId = usuarioId;
        UpdatedAt = DateTime.UtcNow;
    }
}
