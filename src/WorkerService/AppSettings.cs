namespace AudioGuestbook.WorkerService;

public sealed class AppSettings
{
    public float BeepVolume { get; set; } = 0.1f;
    public float GreetingVolume { get; set; } = 0.3f;
    public float StartupVolume { get; set; } = 0.4f;

    public int PromptingDelay { get; set; } = 1500;

    public string AudioRecordingPath { get; set; } = null!;
    public int RecordLimitInSeconds { get; set; } = 90;

    public int RecorderRate { get; set; } = 48000;
    public int RecorderBits { get; set; } = 24;
    public int RecorderChannels { get; set; } = 1;
}
