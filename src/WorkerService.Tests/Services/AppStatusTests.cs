using AudioGuestbook.WorkerService.Enums;
using AudioGuestbook.WorkerService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AudioGuestbook.WorkerService.Tests.Services;

public sealed class AppStatusTests
{
    private readonly IAppStatus _service;

    public AppStatusTests()
    {
        var logger = Substitute.For<ILogger<AppStatus>>();
        _service = new AppStatus(logger);
    }

    [Theory]
    [InlineData(Mode.Initialising)]
    [InlineData(Mode.Ready)]
    [InlineData(Mode.Prompting)]
    [InlineData(Mode.Recording)]
    public void Mode_ShouldSetAndReadValueCorrectly(Mode mode)
    {
        // Act
        _service.Mode = mode;
        
        // Assert
        _service.Mode.Should().Be(mode);
    }
}