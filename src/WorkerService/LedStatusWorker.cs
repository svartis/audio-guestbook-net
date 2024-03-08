using AudioGuestbook.WorkerService.Enums;
using AudioGuestbook.WorkerService.Services;

namespace AudioGuestbook.WorkerService;

public sealed class LedStatusWorker(IAppStatus appStatus, IGpioAccess gpioAccess) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        SetLedLights(true, true, true);

        while (!stoppingToken.IsCancellationRequested)
        {
            SwitchModes(appStatus.Mode);
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
        gpioAccess.GreenLedOn = greenLedOn;
        gpioAccess.YellowLedOn = yellowLedOn;
        gpioAccess.RedLedOn = redLedOn;
    }
}