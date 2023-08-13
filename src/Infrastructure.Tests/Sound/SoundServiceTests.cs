using AudioGuestbook.Infrastructure.Sound;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AudioGuestbook.Infrastructure.Tests.Sound;

public class SoundServiceTests
{
    private readonly ISoundService _service;

    public SoundServiceTests()
    {
        var logger = Substitute.For<ILogger<SoundService>>();
        var soundSettings = new SoundSettings
        {
            MasterVolume = 0f
        };
        _service = new SoundService(logger, soundSettings);
    }

    [Fact]
    public async Task PlayStartup_Test()
    {
        // Act
        var act = () => _service.PlayStartup();

        // Assert
        await act.Should().NotThrowAsync();
    }


    [Fact]
    public async Task PlayBeep_Test()
    {
        // Act
        var act = () => _service.PlayBeep();

        // Assert
        await act.Should().NotThrowAsync();
    }
}