using Microsoft.Extensions.Logging;
using NAudio.Wave;

namespace AudioGuestbook.Infrastructure.Sound;

public interface ISoundService
{
    Task PlayStartup();
    Task PlayBeep();
    Task Play(string filePath);
}

public sealed class SoundService : ISoundService
{
    private readonly ILogger<SoundService> _logger;
    private readonly SoundSettings _soundSettings;
    private readonly string _systemMediaFolderPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;

    public SoundService(ILogger<SoundService> logger, SoundSettings soundSettings)
    {
        _logger = logger;
        _soundSettings = soundSettings;
        //TODO: Cache system sounds & Play sounds
    }

    public async Task PlayStartup()
    {
        var path = Path.Combine(_systemMediaFolderPath, "Media/startup.wav");
        await Play(path);
    }

    public async Task PlayBeep()
    {
        var path = Path.Combine(_systemMediaFolderPath, "Media/beep.wav");
        await Play(path);
    }

    public async Task Play(string filePath)
    {
        _logger.LogInformation("Start playing sound: {filePath}", filePath);
        await using var audioFile = new AudioFileReader(filePath);
        using var outputDevice = new WaveOutEvent();
        outputDevice.Volume = _soundSettings.MasterVolume;
        outputDevice.Init(audioFile);
        outputDevice.Play();
        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            await Task.Delay(100);
        }
        _logger.LogInformation("End playing sound: {filePath}", filePath);
    }
}