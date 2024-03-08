using AudioGuestbook.WorkerService.Services;
using AudioGuestbook.WorkerService.Tests.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace AudioGuestbook.WorkerService.Tests.Services;

public sealed class AudioRecorderTests
{
    private readonly AudioRecorder _service;
    private readonly AppSettings _appSettings;

    public AudioRecorderTests()
    {
        var logger = Substitute.For<ILogger<AudioRecorder>>();
        var nSoundFactory = Substitute.For<INSoundFactory>();
        nSoundFactory.GetWaveInEvent().Returns(new WaveInEventMock());
        _appSettings = new AppSettings();
        _service = new AudioRecorder(logger, nSoundFactory, _appSettings);
    }

    [Fact]
    public async Task StartStop_CreatesFile()
    {
        // Arrange
        var path = GetAudioRecordingPath("New", true);
        _appSettings.AudioRecordingPath = path;

        // Act
        _service.Start();
        await Task.Delay(1000);
        _service.Stop();

        // Assert
        Directory.GetFiles(path).Length.Should().Be(1);
    }

    private static string GetAudioRecordingPath(string folder, bool clean)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Recordings", folder);
        if (clean)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
        return path;
    }
}