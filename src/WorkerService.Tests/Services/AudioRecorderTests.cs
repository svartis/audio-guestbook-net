using AudioGuestbook.WorkerService.Services;
using AudioGuestbook.WorkerService.Tests.Mocks;
using NSubstitute;

namespace AudioGuestbook.WorkerService.Tests.Services;

public class AudioRecorderTests
{
    private readonly IAudioRecorder _service;

    public AudioRecorderTests()
    {
        var nSoundFactory = Substitute.For<INSoundFactory>();
        nSoundFactory.GetWaveInEvent().Returns(new WaveInEventMock());
        _service = new AudioRecorder(nSoundFactory);
    }

    [Fact]
    public void Abc()
    {
        _service.Start();
        Thread.Sleep(1000);
        _service.Stop();
    }
}