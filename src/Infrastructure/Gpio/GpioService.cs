using System.Device.Gpio;

namespace AudioGuestbook.Infrastructure.Gpio;

public interface IGpioService
{
    bool IsPinOpen();
}

public class GpioService : IGpioService
{
    private readonly GpioController _controller = new();

    public bool IsPinOpen()
    {
        return _controller.IsPinOpen(18);
    }
}