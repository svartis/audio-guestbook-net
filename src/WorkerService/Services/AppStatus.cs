using AudioGuestbook.WorkerService.Enums;

namespace AudioGuestbook.WorkerService.Services;

public interface IAppStatus
{
    void SetMode(Mode mode);
    Mode GetMode();
}

public sealed class AppStatus : IAppStatus
{
    private readonly ILogger<AppStatus> _logger;

    private Mode _mode;

    public AppStatus(ILogger<AppStatus> logger)
    {
        _logger = logger;
        SetMode(Mode.Initialising);
    }

    public void SetMode(Mode mode)
    {
        _logger.LogInformation("Mode switched to: {mode}", mode);
        _mode = mode;
    }

    public Mode GetMode()
    {
        return _mode;
    }
}