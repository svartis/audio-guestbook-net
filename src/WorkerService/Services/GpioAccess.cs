using System.Device.Gpio;

namespace AudioGuestbook.WorkerService.Services;

public interface IGpioAccess
{
    bool HandsetLifted { get; }
}

public sealed class GpioAccess : IGpioAccess, IDisposable
{
    private readonly GpioController _gpioController;
    
    private const int HandsetPinNumber = 5;
    private readonly GpioPin _handsetGpioPin;

    public GpioAccess()
    {
        _gpioController = new GpioController();
        _handsetGpioPin = _gpioController.OpenPin(HandsetPinNumber, PinMode.InputPullUp);
    }

    public bool HandsetLifted => _handsetGpioPin.Read() == PinValue.Low;

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