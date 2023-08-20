using System.Diagnostics.CodeAnalysis;
using NAudio.Wave;

namespace AudioGuestbook.WorkerService.Services;

public interface INSoundFactory
{
    IWavePlayer GetWaveOutEvent();
    IWaveIn GetWaveInEvent();
}

[ExcludeFromCodeCoverage]
public sealed class NSoundFactory : INSoundFactory
{
    public IWavePlayer GetWaveOutEvent()
    {
        return new WaveOutEvent();
    }

    public IWaveIn GetWaveInEvent()
    {
        return new WaveInEvent
        {
            WaveFormat = new WaveFormat(),
        };
    }
}