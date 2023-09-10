using System.Device.Gpio;

namespace AudioGuestbook.WorkerService.Tests.Mocks;

internal sealed class VirtualGpioDriver : GpioDriver
{
    internal event PinChangeEventHandler? InputPinValueChanged;
    internal event PinChangeEventHandler? OutputPinValueChanged;

    protected override int PinCount { get; }

    private bool[] _openStatus;
    private PinMode[] _pinModes;
    private PinValue[] _pinValues;

    internal VirtualGpioDriver(int pinCount)
    {
        PinCount = pinCount;

        _openStatus = new bool[PinCount];
        _pinModes = new PinMode[PinCount];
        _pinValues = new PinValue[PinCount];
    }

    internal void Input(int pinNumber, PinValue? value)
    {
        if (_pinModes[pinNumber] == PinMode.Output && _pinValues[pinNumber] != value)
        {
            throw new SystemException("The pin is shorted.");
        }

        var lastPinValue = _pinValues[pinNumber];

        var actualValue = value ?? (_pinModes[pinNumber] == PinMode.InputPullUp ? PinValue.High : _pinModes[pinNumber] == PinMode.InputPullDown ? PinValue.Low : _pinValues[pinNumber]);
        if (actualValue != lastPinValue)
        {
            _pinValues[pinNumber] = actualValue;
            InputPinValueChanged?.Invoke(this, new PinValueChangedEventArgs(value == PinValue.Low ? PinEventTypes.Falling : PinEventTypes.Rising, pinNumber));
        }
    }

    protected override void AddCallbackForPinValueChangedEvent(int pinNumber, PinEventTypes eventTypes, PinChangeEventHandler callback)
    {
        InputPinValueChanged += callback;
    }

    protected override void ClosePin(int pinNumber)
    {
        if (_openStatus[pinNumber])
        {
            _openStatus[pinNumber] = false;
        }
        else
        {
            throw new InvalidOperationException("Cannot close pin vale while it is closed.");
        }
    }

    protected override int ConvertPinNumberToLogicalNumberingScheme(int pinNumber)
    {
        return pinNumber;
    }

    protected override PinMode GetPinMode(int pinNumber)
    {
        return _pinModes[pinNumber];
    }

    protected override bool IsPinModeSupported(int pinNumber, PinMode mode)
    {
        return true;
    }

    protected override void OpenPin(int pinNumber)
    {
        if (_openStatus[pinNumber])
        {
            throw new InvalidOperationException("Cannot open pin vale while it is opened.");
        }

        _openStatus[pinNumber] = true;
    }

    protected override PinValue Read(int pinNumber)
    {
        if (_openStatus[pinNumber])
        {
            return _pinValues[pinNumber];
        }

        throw new InvalidOperationException("Cannot read pin vale while the pin is closed.");
    }

    protected override void RemoveCallbackForPinValueChangedEvent(int pinNumber, PinChangeEventHandler callback)
    {
        InputPinValueChanged -= callback;
    }

    protected override void SetPinMode(int pinNumber, PinMode mode)
    {
        _pinModes[pinNumber] = mode;
    }

    protected override WaitForEventResult WaitForEvent(int pinNumber, PinEventTypes eventTypes, CancellationToken cancellationToken)
    {
        var lastPinValue = _pinValues[pinNumber];
        while (true)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return new WaitForEventResult
                {
                    EventTypes = PinEventTypes.None,
                    TimedOut = true
                };
            }

            if (_pinValues[pinNumber] != lastPinValue)
            {
                return new WaitForEventResult
                {
                    EventTypes = lastPinValue == PinValue.Low ? PinEventTypes.Rising : PinEventTypes.Falling,
                    TimedOut = true
                };
            }

            Thread.Sleep(0);
        }
    }

    protected override void Write(int pinNumber, PinValue value)
    {
        if (_pinModes[pinNumber] == PinMode.Output)
        {
            var lastPinValue = _pinValues[pinNumber];
            _pinValues[pinNumber] = value;

            if (value != lastPinValue)
            {
                OutputPinValueChanged?.Invoke(this, new PinValueChangedEventArgs(lastPinValue == PinValue.Low ? PinEventTypes.Rising : PinEventTypes.Falling, pinNumber));
            }
        }
        else
        {
            throw new InvalidOperationException("Cannot write pin value while the pin is not in output mode.");
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        _openStatus = null!;
        _pinModes = null!;
        _pinValues = null!;

        base.Dispose(disposing);
    }
}