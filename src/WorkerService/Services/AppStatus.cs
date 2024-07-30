using AudioGuestbook.WorkerService.Enums;

namespace AudioGuestbook.WorkerService.Services;

public interface IAppStatus
{
    public Mode Mode { get; set; }
}

public sealed class AppStatus(ILogger<AppStatus> logger) : IAppStatus
{
    private Mode _mode = Mode.Initialising;

    public Mode Mode
    {
        get => _mode;
        set
        {
            logger.LogInformation("Mode switched to: {mode}", value);
            _mode = value;
        }
    }
}