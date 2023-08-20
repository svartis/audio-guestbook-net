using AudioGuestbook.WorkerService.Services;
using AudioGuestbook.WorkerService.Tests.Mocks;
using FluentAssertions;
using NSubstitute;

namespace AudioGuestbook.WorkerService.Tests.Services;

public class AudioRecorderTests
{
    private readonly IAudioRecorder _service;
    private readonly AppSettings _appSettings;

    public AudioRecorderTests()
    {
        var nSoundFactory = Substitute.For<INSoundFactory>();
        nSoundFactory.GetWaveInEvent().Returns(new WaveInEventMock());
        _appSettings = new AppSettings();
        _service = new AudioRecorder(nSoundFactory, _appSettings);
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

    [Fact]
    public void GetAudioRecordingPath_WhenFilesExists_ShouldReturnExpectedFile()
    {
        // Arrange
        _appSettings.AudioRecordingPath = GetAudioRecordingPath("Latest", false);

        // Act
        var result = _service.GetLatestRecordingFilePath();

        // Assert
        Path.GetFileName(result).Should().Be("startup.wav");
    }

    [Fact]
    public void GetAudioRecordingPath_WhenNoFiles_ShouldReturnNull()
    {
        // Arrange
        _appSettings.AudioRecordingPath = GetAudioRecordingPath("Latest-Empty", false);

        // Act
        var result = _service.GetLatestRecordingFilePath();

        // Assert
        result.Should().BeNull();
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