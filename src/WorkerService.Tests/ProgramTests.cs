using AudioGuestbook.WorkerService.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace AudioGuestbook.WorkerService.Tests;

public sealed class ProgramTests
{
    [Fact]
    public void Program_CreateHostBuilder_HasRequiredServices()
    {
        // Setup
        // When running this test it thinks it's in Production, so force it to Development
        Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Development");
        using var host = Program.CreateHostBuilder(Array.Empty<string>()).Build();

        // Act
        var serviceProvider = host.Services.GetRequiredService<IServiceProvider>();
        var appStatus = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IAppStatus>();

        // Assert
        appStatus.Should().NotBeNull();
    }
}