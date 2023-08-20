using NAudio.Wave;

namespace AudioGuestbook.WorkerService.Services;

public interface IAudioRecorder
{
    void Start();
    void Stop();
}

public sealed class AudioRecorder : IAudioRecorder
{
    private readonly ILogger<AudioRecorder> _logger;
    private readonly INSoundFactory _nSoundFactory;
    private readonly AppSettings _appSettings;
    private IWaveIn? _sourceStream;
    private WaveFileWriter? _waveWriter;

    public AudioRecorder(ILogger<AudioRecorder> logger, INSoundFactory nSoundFactory, AppSettings appSettings)
    {
        _logger = logger;
        _nSoundFactory = nSoundFactory;
        _appSettings = appSettings;
    }

    public void Start()
    {
        EnsureFolderExists();

        _sourceStream = _nSoundFactory.GetWaveInEvent();
        _sourceStream.DataAvailable += SourceStreamDataAvailable;

        var filename = (DateTime.Now.ToString("s") + ".wav")
            .Replace("-", "")
            .Replace(":", "");

        _waveWriter = new WaveFileWriter(Path.Combine(_appSettings.AudioRecordingPath, filename), _sourceStream.WaveFormat);
        _logger.LogInformation("Starting Recording");
        _sourceStream.StartRecording();
    }

    public void Stop()
    {
        _logger.LogInformation("Stop Recording");
        if (_sourceStream != null)
        {
            _sourceStream.StopRecording();
            _sourceStream.Dispose();
            _sourceStream = null;
        }
        if (_waveWriter != null)
        {
            _waveWriter.Dispose();
            _waveWriter = null;
        }
    }

    private void SourceStreamDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (_waveWriter == null)
        {
            return;
        }
        _waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
        _waveWriter.Flush();
    }

    private void EnsureFolderExists()
    {
        if (!Directory.Exists(_appSettings.AudioRecordingPath))
        {
            Directory.CreateDirectory(_appSettings.AudioRecordingPath);
        }
    }
}