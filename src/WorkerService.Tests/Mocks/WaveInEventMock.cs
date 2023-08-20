using NAudio.Wave;

namespace AudioGuestbook.WorkerService.Tests.Mocks;

public sealed class WaveInEventMock : IWaveIn
{
    public void StartRecording()
    {
    }

    public void StopRecording()
    {
    }

    public WaveFormat WaveFormat { get; set; } = new();
    public event EventHandler<WaveInEventArgs>? DataAvailable = (sender, args) => { };
    public event EventHandler<StoppedEventArgs>? RecordingStopped = (sender, args) => { };

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private static void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    ~WaveInEventMock() => Dispose(false);
}