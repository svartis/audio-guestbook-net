using AudioGuestbook.WorkerService.Enums;
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
        _gpioAccess.GreenLedOn = true;
        _gpioAccess.YellowLedOn = true;
        _gpioAccess.RedLedOn = true;

        // Required for the method to be executed asynchronously, allowing startup to continue.
        await Task.Yield();
        await Task.Delay(100, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            switch (_appStatus.Mode)
            {
                case Mode.Initialising:
                    _gpioAccess.GreenLedOn = true;
                    _gpioAccess.YellowLedOn = true;
                    _gpioAccess.RedLedOn = true;
                    break;
                case Mode.Ready:
                    _gpioAccess.GreenLedOn = true;
                    _gpioAccess.YellowLedOn = false;
                    _gpioAccess.RedLedOn = false;
                    break;
                case Mode.Prompting:
                case Mode.Playing:
                    _gpioAccess.GreenLedOn = false;
                    _gpioAccess.YellowLedOn = true;
                    _gpioAccess.RedLedOn = false;
                    break;
                case Mode.Recording:
                    _gpioAccess.GreenLedOn = false;
                    _gpioAccess.YellowLedOn = false;
                    _gpioAccess.RedLedOn = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Task.Delay(100, stoppingToken);
        }
    }
}