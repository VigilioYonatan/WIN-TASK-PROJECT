using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Common.Interfaces;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        // Automatically apply pending migrations or ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Default User if empty
        if (!await context.Usuarios.AnyAsync())
        {
            var adminUser = new Usuario(
                nombre: "Administrador Sistema",
                email: "admin@rimac.com",
                passwordHash: passwordHasher.HashPassword("Admin1234!")
            );

            await context.Usuarios.AddAsync(adminUser);
            await context.SaveChangesAsync();

            // Seed Categories
            var catTrabajo = new Categoria("Trabajo");
            var catUrgente = new Categoria("Urgente");
            var catPersonal = new Categoria("Personal");
            var catSeguros = new Categoria("Pólizas y Seguros");

            await context.Categorias.AddRangeAsync(catTrabajo, catUrgente, catPersonal, catSeguros);
            await context.SaveChangesAsync();

            // Seed Sample Tasks
            var tarea1 = new Tarea(
                titulo: "Revisar liquidación de pólizas SCTR",
                descripcion: "Validar inconsistencias en tramas de inclusión de RRLL.",
                fechaInicio: DateTime.UtcNow,
                fechaCierre: DateTime.UtcNow.AddDays(3),
                categoriaId: catSeguros.Id,
                usuarioId: adminUser.Id,
                estado: EstadoTarea.EnProgreso
            );

            var tarea2 = new Tarea(
                titulo: "Migración de arquitectura a DDD .NET 8",
                descripcion: "Implementar pipeline CI/CD, DevSecOps y observabilidad con Serilog.",
                fechaInicio: DateTime.UtcNow,
                fechaCierre: DateTime.UtcNow.AddDays(7),
                categoriaId: catTrabajo.Id,
                usuarioId: adminUser.Id,
                estado: EstadoTarea.Pendiente
            );

            await context.Tareas.AddRangeAsync(tarea1, tarea2);
            await context.SaveChangesAsync();
        }
    }
}
