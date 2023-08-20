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
        SetLedLights(true, true, true);

        while (!stoppingToken.IsCancellationRequested)
        {
            SwitchModes(_appStatus.Mode);
            await Task.Delay(100, stoppingToken);
        }
    }

    private void SwitchModes(Mode mode)
    {
        switch (mode)
        {
            case Mode.Initialising:
                SetLedLights(true, true, true);
                break;
            case Mode.Ready:
                SetLedLights(true, false, false);
                break;
            case Mode.Prompting:
            case Mode.Playback:
                SetLedLights(false, true, false);
                break;
            case Mode.Recording:
                SetLedLights(false, false, true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    private void SetLedLights(bool greenLedOn, bool yellowLedOn, bool redLedOn)
    {
        _gpioAccess.GreenLedOn = greenLedOn;
        _gpioAccess.YellowLedOn = yellowLedOn;
        _gpioAccess.RedLedOn = redLedOn;
    }
}