using AudioGuestbook.WorkerService.Services;
using NSubstitute;
using AudioGuestbook.WorkerService.Enums;
using FluentAssertions;

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
    public async Task ExecuteAsync_Should_Set_GpioAccess_Properties(Mode mode, bool greenLedOn, bool yellowLedOn, bool redLedOn)
    {
        // Arrange
        _appStatus.Mode.Returns(mode);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(200));

        // Act
        await _worker.StartAsync(cts.Token);

        // Assert
        _gpioAccess.Received().GreenLedOn = greenLedOn;
        _gpioAccess.Received().YellowLedOn = yellowLedOn;
        _gpioAccess.Received().RedLedOn = redLedOn;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Set_GpioAccess_Properties2()
    {
        // Arrange
        _appStatus.Mode.Returns((Mode)99);
        var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(200));

        // Act
        var act = () => _worker.StartAsync(cts.Token);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }
}