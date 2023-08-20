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
    private WaveInEvent? _sourceStream;
    private WaveFileWriter? _waveWriter;

    private const string FilePath = @"c:\Temp\Recordings";
    private const int Rate = 44100;
    private const int Bits = 16;
    private const int Channels = 1;

    public void Start()
    {
        _sourceStream = new WaveInEvent
        {
            WaveFormat = new WaveFormat(Rate, Bits, Channels)
        };

        _sourceStream.DataAvailable += SourceStreamDataAvailable;

        if (!Directory.Exists(FilePath))
        {
            Directory.CreateDirectory(FilePath);
        }

        var filename = (DateTime.Now.ToString("s") + ".wav")
            .Replace("-", "")
            .Replace(":", "");

        _waveWriter = new WaveFileWriter(Path.Combine(FilePath, filename), _sourceStream.WaveFormat);
        _sourceStream.StartRecording();
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

    public void Stop()
    {
        if (_sourceStream != null)
        {
            _sourceStream.StopRecording();
            _sourceStream.Dispose();
            _sourceStream = null;
        }
        if (_waveWriter == null)
        {
            return;
        }
        _waveWriter.Dispose();
        _waveWriter = null;
    }

    public string? GetLatestRecordingFilePath()
    {
        var directory = new DirectoryInfo(FilePath);
        var fileInfo = directory.GetFiles("*.wav").MaxBy(f => f.LastWriteTime);
        return fileInfo?.FullName;
    }
}