using NAudio.Wave;

namespace AudioGuestbook.WorkerService.Services;

public interface IAudioOutput
{
    Task PlayStartupAsync(CancellationToken cancellationToken);
    Task PlayBeepAsync(CancellationToken cancellationToken);
    Task PlayGreetingAsync(CancellationToken cancellationToken);
}

public sealed class AudioOutput : IAudioOutput
{
    private readonly ILogger<AudioOutput> _logger;

    private readonly string _startupAudioFile;
    private readonly string _beepAudioFile;
    private readonly string _greetingAudioFile;

    private readonly float _masterVolume = 0.5f;


    public AudioOutput(ILogger<AudioOutput> logger)
    {
        _logger = logger;
        var systemMediaFolderPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "Media");
        _startupAudioFile = Path.Combine(systemMediaFolderPath, "startup.wav");
        _beepAudioFile = Path.Combine(systemMediaFolderPath, "beep.wav");
        _greetingAudioFile = Path.Combine(systemMediaFolderPath, "greeting.wav");
    }


    public async Task PlayStartupAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Playing startup audio");

        await using var audioFile = new AudioFileReader(_startupAudioFile);
        using var outputDevice = new WaveOutEvent();
        outputDevice.Volume = _masterVolume;
        outputDevice.Init(audioFile);
        outputDevice.Play();
        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            await Task.Delay(50, cancellationToken);
        }
    }

    public async Task PlayBeepAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Playing beep audio");

        await using var audioFile = new AudioFileReader(_beepAudioFile);
        using var outputDevice = new WaveOutEvent();
        outputDevice.Volume = _masterVolume;
        outputDevice.Init(audioFile);
        outputDevice.Play();
        while (outputDevice.PlaybackState == PlaybackState.Playing)
        {
            await Task.Delay(50, cancellationToken);
        }
    }

    public Task PlayGreetingAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Playing greeting audio");

        throw new NotImplementedException();
    }
}