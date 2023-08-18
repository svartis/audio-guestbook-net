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

    public void Start()
    {
        // TODO: Cleanup
        int inputDeviceIndex = -1;
        for (var n = -1; n < WaveIn.DeviceCount; n++)
        {
            var caps = WaveIn.GetCapabilities(n);
            Console.WriteLine($"{n}: {caps.ProductName}");
            inputDeviceIndex = n;
        }

        //TODO: Set same as other solution
        _sourceStream = new WaveInEvent
        {
            DeviceNumber = inputDeviceIndex,
            WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(inputDeviceIndex).Channels)
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
        var pattern = "*.wav";
        var myFile = directory.GetFiles(pattern)
            .OrderByDescending(f => f.LastWriteTime)
            .FirstOrDefault();
        return myFile?.FullName;
    }
}