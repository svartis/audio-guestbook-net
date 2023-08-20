using AudioGuestbook.WorkerService.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using WaveOutEventMock = AudioGuestbook.WorkerService.Tests.Mocks.WaveOutEventMock;

namespace AudioGuestbook.WorkerService.Tests.Services;

public sealed class AudioOutputTests
{
    private readonly IAudioOutput _service;

    public AudioOutputTests()
    {
        var logger = Substitute.For<ILogger<AudioOutput>>();
        var nSoundFactory = Substitute.For<INSoundFactory>();
        nSoundFactory.GetWaveOutEvent().Returns(new WaveOutEventMock());
        _service = new AudioOutput(logger, nSoundFactory, new AppSettings());
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

    [Fact]
    public async Task PlayGreetingAsync_Success_Test()
    {
        // Act
        var result = await _service.PlayGreetingAsync(() => true, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PlayGreetingAsync_Cancel_Test()
    {
        // Act
        var result = await _service.PlayGreetingAsync(() => false, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}