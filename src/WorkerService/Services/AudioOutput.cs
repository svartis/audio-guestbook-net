﻿using NAudio.Wave;

namespace AudioGuestbook.WorkerService.Services;

public interface IAudioOutput
{
    Task PlayStartupAsync(CancellationToken cancellationToken);
    Task PlayBeepAsync(CancellationToken cancellationToken);
    Task<bool> PlayGreetingAsync(Func<bool> cancelCondition, CancellationToken cancellationToken);
    Task<bool> PlayAsync(string fileName, Func<bool> cancelCondition, CancellationToken cancellationToken);
}

public sealed class AudioOutput : IAudioOutput
{
    private readonly ILogger<AudioOutput> _logger;
    private readonly INSoundFactory _nSoundFactory;

    private readonly string _startupAudioFile;
    private readonly string _beepAudioFile;
    private readonly string _greetingAudioFile;

    private readonly float _masterVolume = 0.5f;

    public AudioOutput(ILogger<AudioOutput> logger, INSoundFactory nSoundFactory)
    {
        _logger = logger;
        _nSoundFactory = nSoundFactory;

        var systemMediaFolderPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "Media");
        _startupAudioFile = Path.Combine(systemMediaFolderPath, "startup.wav");
        _beepAudioFile = Path.Combine(systemMediaFolderPath, "beep.wav");
        _greetingAudioFile = Path.Combine(systemMediaFolderPath, "greeting.wav");
    }

    public async Task PlayStartupAsync(CancellationToken cancellationToken)
    {
        await PlayAsync(_startupAudioFile, cancellationToken);
    }

    public async Task PlayBeepAsync(CancellationToken cancellationToken)
    {
        await PlayAsync(_beepAudioFile, cancellationToken);
    }

    public async Task<bool> PlayGreetingAsync(Func<bool> cancelCondition, CancellationToken cancellationToken)
    {
        return await PlayAsync(_greetingAudioFile, cancelCondition, cancellationToken);
    }

    private async Task PlayAsync(string fileName, CancellationToken cancellationToken)
    {
        await PlayAsync(fileName, () => true, cancellationToken);
    }

    public async Task<bool> PlayAsync(string fileName, Func<bool> cancelCondition, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start audio {fileName}", fileName);
        await using var audioFile = new AudioFileReader(fileName);
        using var outputDevice = _nSoundFactory.GetWaveOutEvent();
        outputDevice.Volume = _masterVolume;
        outputDevice.Init(audioFile);
        outputDevice.Play();
        _logger.LogInformation("Wait until audio has finished playing");
        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            var canceled = cancelCondition.Invoke();
            if (canceled)
            {
                _logger.LogInformation("Cancel start audio {fileName}", fileName);
                outputDevice.Stop();
                return true;
            }

            await Task.Delay(50, cancellationToken);
        }
        _logger.LogInformation("Stop audio {fileName}", fileName);
        return false;
    }
}