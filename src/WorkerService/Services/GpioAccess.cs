using System.Device.Gpio;

namespace AudioGuestbook.WorkerService.Services;

public interface IGpioAccess
{
    bool HandsetLifted { get; }
    bool GreenLedOn { get; set; }
    bool YellowLedOn { get; set; }
    bool RedLedOn { get; set; }
}

public sealed class GpioAccess : IGpioAccess, IDisposable
{
    private readonly GpioController _gpioController;

    internal const int HandsetPinNumber = 5;
    private readonly GpioPin _handsetGpioPin;

    internal const int GreenLedPinNumber = 18;
    private readonly GpioPin _greenLedGpioPin;

    internal const int YellowLedPinNumber = 24;
    private readonly GpioPin _yellowLedGpioPin;

    internal const int RedLedPinNumber = 25;
    private readonly GpioPin _redLedGpioPin;

    public GpioAccess(GpioController gpioController)
    {
        _gpioController = gpioController;

        _handsetGpioPin = _gpioController.OpenPin(HandsetPinNumber, PinMode.InputPullUp);

        _greenLedGpioPin = _gpioController.OpenPin(GreenLedPinNumber, PinMode.Output);
        _yellowLedGpioPin = _gpioController.OpenPin(YellowLedPinNumber, PinMode.Output);
        _redLedGpioPin = _gpioController.OpenPin(RedLedPinNumber, PinMode.Output);
    }

    public bool HandsetLifted => _handsetGpioPin.Read() == PinValue.Low;

    public bool GreenLedOn
    {
        get => _greenLedGpioPin.Read() == PinValue.High;
        set => _greenLedGpioPin.Write(value ? PinValue.High : PinValue.Low);
    }

    public bool YellowLedOn
    {
        get => _yellowLedGpioPin.Read() == PinValue.High;
        set => _yellowLedGpioPin.Write(value ? PinValue.High : PinValue.Low);
    }

    public bool RedLedOn
    {
        get => _redLedGpioPin.Read() == PinValue.High;
        set => _redLedGpioPin.Write(value ? PinValue.High : PinValue.Low);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            _gpioController.Dispose();
        }
    }

    ~GpioAccess() => Dispose(false);
}