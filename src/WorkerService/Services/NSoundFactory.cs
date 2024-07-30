using System.Diagnostics.CodeAnalysis;
using NAudio.Wave;

namespace AudioGuestbook.WorkerService.Services;

public interface INSoundFactory
{
    IWavePlayer GetWaveOutEvent();
    IWaveIn GetWaveInEvent();
}

[ExcludeFromCodeCoverage]
public sealed class NSoundFactory(AppSettings appSettings) : INSoundFactory
{
    public IWavePlayer GetWaveOutEvent()
    {
        return new WaveOutEvent();
    }

    public IWaveIn GetWaveInEvent()
    {
        return new WaveInEvent
        {
            // Default: 44100,16,2
            WaveFormat = new WaveFormat(appSettings.RecorderRate, appSettings.RecorderBits, appSettings.RecorderChannels),
        };
    }
}