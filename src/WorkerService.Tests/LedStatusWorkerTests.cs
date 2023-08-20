using AudioGuestbook.WorkerService.Services;
using NSubstitute;
using AudioGuestbook.WorkerService.Enums;

namespace AudioGuestbook.WorkerService.Tests;

public sealed class LedStatusWorkerTests
{
    private readonly IAppStatus _appStatus;
    private readonly IGpioAccess _gpioAccess;
    private readonly LedStatusWorker _worker;

    public LedStatusWorkerTests()
    {
        _appStatus = Substitute.For<IAppStatus>();
        _gpioAccess = Substitute.For<IGpioAccess>();

        _worker = new LedStatusWorker(_appStatus, _gpioAccess);
    }

    [Theory]
    [InlineData(Mode.Initialising, true, true, true)]
    [InlineData(Mode.Ready, true, false, false)]
    [InlineData(Mode.Prompting, false, true, false)]
    [InlineData(Mode.Recording, false, false, true)]
    [InlineData(Mode.Playback, false, true, false)]
    public void SwitchLeds_Should_Set_GpioAccess_Properties(Mode mode, bool greenLedOn, bool yellowLedOn, bool redLedOn)
    {
        // Arrange
        _appStatus.Mode.Returns(mode);

        // Act
        _worker.SwitchLeds(mode);

        // Assert
        _gpioAccess.Received().GreenLedOn = greenLedOn;
        _gpioAccess.Received().YellowLedOn = yellowLedOn;
        _gpioAccess.Received().RedLedOn = redLedOn;
    }
}