using FluentValidation;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITareaRepository _tareaRepository;
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateTaskRequest> _createValidator;
    private readonly IValidator<UpdateTaskRequest> _updateValidator;

    public TaskService(
        ITareaRepository tareaRepository,
        ICategoriaRepository categoriaRepository,
        IUsuarioRepository usuarioRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IValidator<CreateTaskRequest> createValidator,
        IValidator<UpdateTaskRequest> updateValidator)
    {
        _tareaRepository = tareaRepository;
        _categoriaRepository = categoriaRepository;
        _usuarioRepository = usuarioRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<TaskDto>> GetAllAsync(
        int? categoriaId = null,
        EstadoTarea? estado = null,
        bool onlyCurrentUser = true,
        CancellationToken cancellationToken = default)
    {
        int? filterUserId = onlyCurrentUser ? _currentUserService.UserId : null;

        var tareas = await _tareaRepository.GetAllAsync(filterUserId, categoriaId, estado, cancellationToken);
        return tareas.Select(MapToDto);
    }

    public async Task<TaskDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarea = await _tareaRepository.GetByIdAsync(id, cancellationToken);
        if (tarea == null)
        {
            throw new EntityNotFoundException(nameof(Tarea), id);
        }

        int? currentUserId = _currentUserService.UserId;
        if (currentUserId.HasValue && tarea.UsuarioId != currentUserId.Value)
        {
            throw new DomainException("No tiene permisos para acceder a esta tarea perteneciente a otro usuario.");
        }

        return MapToDto(tarea);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);
        if (categoria == null)
        {
            throw new EntityNotFoundException(nameof(Categoria), request.CategoriaId);
        }

        int usuarioId = _currentUserService.UserId 
            ?? throw new DomainException("Usuario no autenticado para crear la tarea.");

        var usuario = await _usuarioRepository.GetByIdAsync(usuarioId, cancellationToken);
        if (usuario == null)
        {
            throw new EntityNotFoundException(nameof(Usuario), usuarioId);
        }

        var tarea = new Tarea(
            request.Titulo,
            request.Descripcion,
            request.FechaInicio,
            request.FechaCierre,
            request.CategoriaId,
            usuarioId,
            request.Estado
        );

        await _tareaRepository.AddAsync(tarea, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Fetch populated entity with navigation properties
        var createdTarea = await _tareaRepository.GetByIdAsync(tarea.Id, cancellationToken);
        return MapToDto(createdTarea ?? tarea);
    }

    public async Task<TaskDto> UpdateAsync(int id, UpdateTaskRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var tarea = await _tareaRepository.GetByIdAsync(id, cancellationToken);
        if (tarea == null)
        {
            throw new EntityNotFoundException(nameof(Tarea), id);
        }

        int? currentUserId = _currentUserService.UserId;
        if (currentUserId.HasValue && tarea.UsuarioId != currentUserId.Value)
        {
            throw new DomainException("No tiene permisos para modificar esta tarea perteneciente a otro usuario.");
        }

        if (tarea.CategoriaId != request.CategoriaId)
        {
            var categoria = await _categoriaRepository.GetByIdAsync(request.CategoriaId, cancellationToken);
            if (categoria == null)
            {
                throw new EntityNotFoundException(nameof(Categoria), request.CategoriaId);
            }
        }

        tarea.Update(
            request.Titulo,
            request.Descripcion,
            request.FechaInicio,
            request.FechaCierre,
            request.CategoriaId,
            request.Estado
        );

        _tareaRepository.Update(tarea);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedTarea = await _tareaRepository.GetByIdAsync(id, cancellationToken);
        return MapToDto(updatedTarea ?? tarea);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var tarea = await _tareaRepository.GetByIdAsync(id, cancellationToken);
        if (tarea == null)
        {
            throw new EntityNotFoundException(nameof(Tarea), id);
        }

        int? currentUserId = _currentUserService.UserId;
        if (currentUserId.HasValue && tarea.UsuarioId != currentUserId.Value)
        {
            throw new DomainException("No tiene permisos para eliminar esta tarea perteneciente a otro usuario.");
        }

        _tareaRepository.Delete(tarea);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static TaskDto MapToDto(Tarea t)
    {
        return new TaskDto(
            t.Id,
            t.Titulo,
            t.Descripcion,
            t.FechaInicio,
            t.FechaCierre,
            t.Estado,
            t.Estado.ToString(),
            t.CategoriaId,
            t.Categoria?.Nombre ?? "General",
            t.UsuarioId,
            t.Usuario?.Nombre ?? "Asignado",
            t.CreatedAt,
            t.UpdatedAt
        );
    }
}
