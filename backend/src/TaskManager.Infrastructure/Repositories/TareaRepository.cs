using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Interfaces;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TareaRepository : ITareaRepository
{
    private readonly ApplicationDbContext _context;

    public TareaRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tarea>> GetAllAsync(
        int? usuarioId = null,
        int? categoriaId = null,
        EstadoTarea? estado = null,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Tarea> query = _context.Tareas
            .Include(t => t.Categoria)
            .Include(t => t.Usuario)
            .AsNoTracking();

        if (usuarioId.HasValue)
        {
            query = query.Where(t => t.UsuarioId == usuarioId.Value);
        }

        if (categoriaId.HasValue)
        {
            query = query.Where(t => t.CategoriaId == categoriaId.Value);
        }

        if (estado.HasValue)
        {
            query = query.Where(t => t.Estado == estado.Value);
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tarea?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Tareas
            .Include(t => t.Categoria)
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task AddAsync(Tarea tarea, CancellationToken cancellationToken = default)
    {
        await _context.Tareas.AddAsync(tarea, cancellationToken);
    }

    public void Update(Tarea tarea)
    {
        _context.Tareas.Update(tarea);
    }

    public void Delete(Tarea tarea)
    {
        _context.Tareas.Remove(tarea);
    }
}
