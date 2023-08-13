using AudioGuestbook.Infrastructure.Sound;
using System.Device.Gpio;
using System.Device.Gpio.Drivers;
using System.Net.NetworkInformation;

namespace AudioGuestbook.WorkerService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ISoundService _soundService;

    public Worker(ILogger<Worker> logger, ISoundService soundService)
    {
        _logger = logger;
        _soundService = soundService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Required for the method to be executed asynchronously, allowing startup to continue.
        await Task.Yield();

        // Play startup sound
        await _soundService.PlayStartup();

        const int Pin = 21;
        const string Alert = "ALERT";
        const string Ready = "READY";

        using var controller = new GpioController();
        controller.OpenPin(Pin, PinMode.InputPullUp);

        Console.WriteLine(
            $"Initial status ({DateTime.Now}): {(controller.Read(Pin) == PinValue.High ? Alert : Ready)}");

        controller.RegisterCallbackForPinValueChangedEvent(
            Pin,
            PinEventTypes.Falling | PinEventTypes.Rising,
            OnPinEvent);

        await Task.Delay(Timeout.Infinite, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }

    static void OnPinEvent(object sender, PinValueChangedEventArgs args)
    {
        const string Alert = "ALERT 🚨";
        const string Ready = "READY ✅";


        Console.WriteLine(
            $"({DateTime.Now}) {(args.ChangeType is PinEventTypes.Rising ? Alert : Ready)}");
    }
}