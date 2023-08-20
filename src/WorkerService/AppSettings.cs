namespace AudioGuestbook.WorkerService;

public sealed class AppSettings
{
    public string AudioRecordingPath { get; set; } = null!;

    public float BeepVolume { get; set; } = 0.1f;
    public float GreetingVolume { get; set; } = 0.3f;
    public float StartupVolume { get; set; } = 0.4f;
}