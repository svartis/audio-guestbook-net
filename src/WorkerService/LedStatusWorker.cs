using AudioGuestbook.WorkerService.Services;

namespace AudioGuestbook.WorkerService;

public sealed class LedStatusWorker : BackgroundService
{
    private readonly IAppStatus _appStatus;
    private readonly IGpioAccess _gpioAccess;

    public LedStatusWorker(IAppStatus appStatus, IGpioAccess gpioAccess)
    {
        _appStatus = appStatus;
        _gpioAccess = gpioAccess;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Required for the method to be executed asynchronously, allowing startup to continue.
        await Task.Yield();
        await Task.Delay(100, stoppingToken);

        // TODO: Connect to GPIO controller
        // TODO: https://github.com/dotnet/iot/tree/main/samples/led-blink-multiple

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(50, stoppingToken);
        }
    }
}