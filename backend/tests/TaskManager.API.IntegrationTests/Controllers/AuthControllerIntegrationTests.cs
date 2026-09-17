using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.DTOs;
using TaskManager.Infrastructure.Data;
using Xunit;

namespace TaskManager.API.IntegrationTests.Controllers;

public class AuthControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove SQL Server DbContext registration and use InMemory database for tests
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("IntegrationTestingDb_" + Guid.NewGuid().ToString());
                });
            });
        });
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsCreatedStatus()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new RegisterRequest("Test User", $"test_{Guid.NewGuid()}@rimac.com", "Password123!");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        content.Should().NotBeNull();
        content!.Token.Should().NotBeNullOrEmpty();
        content.Usuario.Email.Should().Be(request.Email.ToLower());
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        string email = $"user_{Guid.NewGuid()}@rimac.com";
        var regRequest = new RegisterRequest("User Test", email, "Password123!");
        await client.PostAsJsonAsync("/api/auth/register", regRequest);

        var loginRequest = new LoginRequest(email, "WrongPassword123!");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
