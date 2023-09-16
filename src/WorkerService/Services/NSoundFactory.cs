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
    private readonly AppSettings _appSettings;

    public NSoundFactory(AppSettings appSettings) {
        _appSettings = appSettings;
    }

    public IWavePlayer GetWaveOutEvent()
    {
        return new WaveOutEvent();
    }

    public IWaveIn GetWaveInEvent()
    {
        return new WaveInEvent
        {
            // Default: 44100,16,2
            WaveFormat = new WaveFormat(_appSettings.RecorderRate, _appSettings.RecorderBits, _appSettings.RecorderChannels),
        };
    }
}