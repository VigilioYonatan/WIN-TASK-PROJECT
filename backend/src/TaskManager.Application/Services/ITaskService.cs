using TaskManager.Application.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllAsync(
        int? categoriaId = null,
        EstadoTarea? estado = null,
        bool onlyCurrentUser = true,
        CancellationToken cancellationToken = default);

    Task<TaskDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default);
    Task<TaskDto> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
