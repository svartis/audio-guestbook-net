using AudioGuestbook.WorkerService.Services;
using System.Device.Gpio;
using AudioGuestbook.WorkerService.Tests.Mocks;
using FluentAssertions;

namespace AudioGuestbook.WorkerService.Tests.Services;

public sealed class GpioAccessTests
{
    private readonly VirtualGpioController _gpioController;
    private readonly IGpioAccess _gpioAccess;

    public GpioAccessTests()
    {
        _gpioController = VirtualGpioController.Create(99);
        _gpioAccess = new GpioAccess(_gpioController);
    }

    [Theory]
    [InlineData(GpioAccess.HandsetPinNumber, false, true)]
    [InlineData(GpioAccess.HandsetPinNumber, true, false)]
    public void HandsetLifted_ShouldSetAndReadValueCorrectly(int pinNumber, bool pinHigh, bool expectedResult)
    {
        // Arrange
        _gpioController.OpenPin(pinNumber, PinMode.Output);
        _gpioController.Write(pinNumber, pinHigh ? PinValue.High : PinValue.Low); 
        
        // Act
        var result = _gpioAccess.HandsetLifted;

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(GpioAccess.PlaybackPinNumber, false, true)]
    [InlineData(GpioAccess.PlaybackPinNumber, true, false)]
    public void PlaybackPressed_ShouldSetAndReadValueCorrectly(int pinNumber, bool pinHigh, bool expectedResult)
    {
        // Arrange
        _gpioController.OpenPin(pinNumber, PinMode.Output);
        _gpioController.Write(pinNumber, pinHigh ? PinValue.High : PinValue.Low);

        // Act
        var result = _gpioAccess.PlaybackPressed;

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(GpioAccess.GreenLedPinNumber, false, false)]
    [InlineData(GpioAccess.GreenLedPinNumber, true, true)]
    public void GreenLedOn_ShouldSetAndReadValueCorrectly(int pinNumber, bool pinHigh, bool expectedResult)
    {
        // Arrange
        _gpioController.OpenPin(pinNumber, PinMode.Output);
        _gpioController.Write(pinNumber, pinHigh ? PinValue.High : PinValue.Low);

        // Act
        var result = _gpioAccess.GreenLedOn;

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(GpioAccess.YellowLedPinNumber, false, false)]
    [InlineData(GpioAccess.YellowLedPinNumber, true, true)]
    public void YellowLedOn_ShouldSetAndReadValueCorrectly(int pinNumber, bool pinHigh, bool expectedResult)
    {
        // Arrange
        _gpioController.OpenPin(pinNumber, PinMode.Output);
        _gpioController.Write(pinNumber, pinHigh ? PinValue.High : PinValue.Low);

        // Act
        var result = _gpioAccess.YellowLedOn;

        // Assert
        result.Should().Be(expectedResult);
    }

    [Theory]
    [InlineData(GpioAccess.RedLedPinNumber, false, false)]
    [InlineData(GpioAccess.RedLedPinNumber, true, true)]
    public void RedLedOn_ShouldSetAndReadValueCorrectly(int pinNumber, bool pinHigh, bool expectedResult)
    {
        // Arrange
        _gpioController.OpenPin(pinNumber, PinMode.Output);
        _gpioController.Write(pinNumber, pinHigh ? PinValue.High : PinValue.Low);

        // Act
        var result = _gpioAccess.RedLedOn;

        // Assert
        result.Should().Be(expectedResult);
    }
}