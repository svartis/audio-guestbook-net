using NAudio.Wave;

namespace AudioGuestbook.WorkerService.Services;

public interface IAudioRecorder
{
    void Start();
    void Stop();
}

public sealed class AudioRecorder(ILogger<AudioRecorder> logger, INSoundFactory nSoundFactory, AppSettings appSettings)
    : IAudioRecorder
{
    private IWaveIn? _sourceStream;
    private WaveFileWriter? _waveWriter;

    public void Start()
    {
        EnsureFolderExists();

        _sourceStream = nSoundFactory.GetWaveInEvent();
        _sourceStream.DataAvailable += SourceStreamDataAvailable;

        var filename = (DateTime.Now.ToString("s") + ".wav")
            .Replace("-", "")
            .Replace(":", "");

        _waveWriter = new WaveFileWriter(Path.Combine(appSettings.AudioRecordingPath, filename), _sourceStream.WaveFormat);
        logger.LogInformation("Starting Recording");
        _sourceStream.StartRecording();
    }

    public void Stop()
    {
        logger.LogInformation("Stop Recording");
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
        if (!Directory.Exists(appSettings.AudioRecordingPath))
        {
            Directory.CreateDirectory(appSettings.AudioRecordingPath);
        }
    }
}