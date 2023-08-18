using AudioGuestbook.WorkerService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AudioGuestbook.WorkerService.Tests.Services;

public class AudioOutputTests
{
    private readonly AudioOutput _service;

    public AudioOutputTests()
    {
        var logger = Substitute.For<ILogger<AudioOutput>>();
        _service = new AudioOutput(logger);
    }

    [Fact]
    public async Task PlayStartupAsync_Test()
    {
        // Act
        var act = () => _service.PlayStartupAsync(CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task PlayBeepAsync_Test()
    {
        // Act
        var act = () => _service.PlayBeepAsync(CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }
}