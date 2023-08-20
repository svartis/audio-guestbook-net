using NAudio.Wave;
using System.Threading.Channels;

namespace AudioGuestbook.WorkerService.Services;

public interface INSoundFactory
{
    IWavePlayer GetWaveOutEvent();
    IWaveIn GetWaveInEvent();
}

public sealed class NSoundFactory : INSoundFactory
{
    private const int Rate = 44100;
    private const int Bits = 16;
    private const int Channels = 1;

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