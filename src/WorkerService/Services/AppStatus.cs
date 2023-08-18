using AudioGuestbook.WorkerService.Enums;

namespace AudioGuestbook.WorkerService.Services;

public interface IAppStatus
{
    public Mode Mode { get; set; }
}

public sealed class AppStatus : IAppStatus
{
    private readonly ILogger<AppStatus> _logger;

    private Mode _mode = Mode.Initialising;

    public AppStatus(ILogger<AppStatus> logger)
    {
        _logger = logger;
    }

    public Mode Mode
    {
        get => _mode;
        set
        {
            _logger.LogInformation("Mode switched to: {mode}", value);
            _mode = value;
        }
    }
}