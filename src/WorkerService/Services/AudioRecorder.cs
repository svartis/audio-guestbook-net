using NAudio.Wave;

namespace AudioGuestbook.WorkerService.Services;

public interface IAudioRecorder
{
    void Start();
    void Stop();
    string? GetLatestRecordingFilePath();
}

public sealed class AudioRecorder : IAudioRecorder
{
    private readonly INSoundFactory _nSoundFactory;
    private readonly AppSettings _appSettings;
    private IWaveIn? _sourceStream;
    private WaveFileWriter? _waveWriter;

    public AudioRecorder(INSoundFactory nSoundFactory, AppSettings appSettings)
    {
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
        _sourceStream.StartRecording();
    }

    public void Stop()
    {
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

    public string? GetLatestRecordingFilePath()
    {
        EnsureFolderExists();
        var directory = new DirectoryInfo(_appSettings.AudioRecordingPath);
        var fileInfo = directory.GetFiles("*.wav").MaxBy(f => f.LastWriteTime);
        return fileInfo?.FullName;
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