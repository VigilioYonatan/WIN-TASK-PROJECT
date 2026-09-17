using FluentValidation;
using TaskManager.Application.DTOs;
using TaskManager.Application.Services;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Exceptions;
using TaskManager.Domain.Interfaces;

namespace TaskManager.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoriaRepository _categoriaRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidator<CreateCategoryRequest> _createValidator;
    private readonly IValidator<UpdateCategoryRequest> _updateValidator;

    public CategoryService(
        ICategoriaRepository categoriaRepository,
        IUnitOfWork unitOfWork,
        IValidator<CreateCategoryRequest> createValidator,
        IValidator<UpdateCategoryRequest> updateValidator)
    {
        _categoriaRepository = categoriaRepository;
        _unitOfWork = unitOfWork;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categorias = await _categoriaRepository.GetAllAsync(cancellationToken);
        return categorias.Select(c => new CategoryDto(
            c.Id,
            c.Nombre,
            c.CreatedAt,
            c.Tareas.Count
        ));
    }

    public async Task<CategoryDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id, cancellationToken);
        if (categoria == null)
        {
            throw new EntityNotFoundException(nameof(Categoria), id);
        }

        return new CategoryDto(categoria.Id, categoria.Nombre, categoria.CreatedAt, categoria.Tareas.Count);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _createValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        string nombre = request.Nombre.Trim();
        bool exists = await _categoriaRepository.ExistsByNameAsync(nombre, null, cancellationToken);
        if (exists)
        {
            throw new DomainException($"Ya existe una categoría con el nombre '{nombre}'.");
        }

        var categoria = new Categoria(nombre);
        await _categoriaRepository.AddAsync(categoria, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CategoryDto(categoria.Id, categoria.Nombre, categoria.CreatedAt, 0);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            throw new ValidationException(validation.Errors);
        }

        var categoria = await _categoriaRepository.GetByIdAsync(id, cancellationToken);
        if (categoria == null)
        {
            throw new EntityNotFoundException(nameof(Categoria), id);
        }

        string nombre = request.Nombre.Trim();
        bool exists = await _categoriaRepository.ExistsByNameAsync(nombre, id, cancellationToken);
        if (exists)
        {
            throw new DomainException($"Ya existe otra categoría con el nombre '{nombre}'.");
        }

        categoria.SetNombre(nombre);
        _categoriaRepository.Update(categoria);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CategoryDto(categoria.Id, categoria.Nombre, categoria.CreatedAt, categoria.Tareas.Count);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var categoria = await _categoriaRepository.GetByIdAsync(id, cancellationToken);
        if (categoria == null)
        {
            throw new EntityNotFoundException(nameof(Categoria), id);
        }

        if (categoria.Tareas.Any())
        {
            throw new DomainException("No se puede eliminar una categoría que tiene tareas asociadas.");
        }

        _categoriaRepository.Delete(categoria);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
