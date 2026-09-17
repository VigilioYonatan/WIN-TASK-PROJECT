using TaskManager.Domain.Entities;

namespace TaskManager.Domain.Interfaces;

public interface ICategoriaRepository
{
    Task<IEnumerable<Categoria>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Categoria?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string nombre, int? excludeId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Categoria categoria, CancellationToken cancellationToken = default);
    void Update(Categoria categoria);
    void Delete(Categoria categoria);
}
