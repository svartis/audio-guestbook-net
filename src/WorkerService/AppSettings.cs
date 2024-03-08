namespace AudioGuestbook.WorkerService;

public sealed class AppSettings
{
    public float BeepVolume { get; init; } = 0.1f;
    public float GreetingVolume { get; init; } = 0.3f;
    public float StartupVolume { get; init; } = 0.4f;

    public int PromptingDelay { get; init; } = 1500;

    public string AudioRecordingPath { get; set; } = null!;
    public int RecordLimitInSeconds { get; init; } = 90;

    public int RecorderRate { get; init; } = 48000;
    public int RecorderBits { get; init; } = 24;
    public int RecorderChannels { get; init; } = 1;
}
