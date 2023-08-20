using NAudio.Wave;

namespace AudioGuestbook.WorkerService.Tests.Mocks;

public sealed class WaveOutEventMock : IWavePlayer, IWavePosition
{
    private int _playbackStateCheckCount;
    private PlaybackState _playbackState;

    public void Play()
    {
        _playbackState = PlaybackState.Playing;
    }

    public void Stop()
    {
        _playbackState = PlaybackState.Stopped;
    }

    public void Pause()
    {
        _playbackState = PlaybackState.Paused;
    }

    public void Init(IWaveProvider waveProvider)
    {
    }

    public float Volume { get; set; }

    public PlaybackState PlaybackState
    {
        get
        {
            _playbackStateCheckCount += 1;
            if (_playbackStateCheckCount > 1)
            {
                _playbackState = PlaybackState.Stopped;
            }
            return _playbackState;
        }
    }

    public long GetPosition()
    {
        return 0;
    }

    WaveFormat IWavePosition.OutputWaveFormat { get; } = new();

    WaveFormat IWavePlayer.OutputWaveFormat { get; } = new();

    public event EventHandler<StoppedEventArgs>? PlaybackStopped;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
        }
    }

    ~WaveOutEventMock() => Dispose(false);
}