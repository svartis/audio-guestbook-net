using AudioGuestbook.WorkerService.Services;

namespace AudioGuestbook.WorkerService.Tests.Services;

public class AudioRecorderTests
{
    private readonly IAudioRecorder _service;

    public AudioRecorderTests()
    {
        _service = new AudioRecorder();
    }

    [Fact]
    public void Abc()
    {
        _service.Start();
        Thread.Sleep(1000);
        _service.Stop();
    }
}