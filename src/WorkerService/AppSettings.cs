namespace AudioGuestbook.WorkerService;

public sealed class AppSettings
{
    public string AudioRecordingPath { get; set; } = null!;

    public float MasterVolume { get; set; } = 0.5f;
}