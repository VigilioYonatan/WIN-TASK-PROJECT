using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Domain.Interfaces;

public interface ITareaRepository
{
    Task<IEnumerable<Tarea>> GetAllAsync(
        int? usuarioId = null,
        int? categoriaId = null,
        EstadoTarea? estado = null,
        CancellationToken cancellationToken = default);

    Task<Tarea?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Tarea tarea, CancellationToken cancellationToken = default);
    void Update(Tarea tarea);
    void Delete(Tarea tarea);
}
