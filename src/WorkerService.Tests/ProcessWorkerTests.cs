﻿using AudioGuestbook.WorkerService.Services;
using NSubstitute;
using AudioGuestbook.WorkerService.Enums;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace AudioGuestbook.WorkerService.Tests;

public sealed class ProcessWorkerTests
{
    private readonly IAppStatus _appStatus;
    private readonly IAudioOutput _audioOutput;
    private readonly IAudioRecorder _audioRecorder;
    private readonly IGpioAccess _gpioAccess;
    private readonly ProcessWorker _worker;

    public ProcessWorkerTests()
    {
        var logger = Substitute.For<ILogger<ProcessWorker>>();
        _appStatus = Substitute.For<IAppStatus>();
        _audioOutput = Substitute.For<IAudioOutput>();
        _audioRecorder = Substitute.For<IAudioRecorder>();
        _gpioAccess = Substitute.For<IGpioAccess>();

        _worker = new ProcessWorker(logger, _appStatus, _audioOutput, _audioRecorder, _gpioAccess);
    }

    [Fact]
    public async Task ExecuteAsync_SetStatusAndPlayStartupSound()
    {
        // Arrange
        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(200));

        // Act
        await _worker.StartAsync(cancellationTokenSource.Token);

        // Assert
        _appStatus.Mode.Should().Be(Mode.Ready);
        await _audioOutput.Received(1).PlayStartupAsync(Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(Mode.Initialising)]
    [InlineData(Mode.Ready)]
    [InlineData(Mode.Prompting)]
    [InlineData(Mode.Recording)]
    [InlineData(Mode.Playback)]
    public async Task SwitchModes_ShouldNotThrowErrors(Mode mode)
    {
        // Act
        var act = () => _worker.SwitchModes(mode, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SwitchModes_InvalidEnum_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => _worker.SwitchModes((Mode)99, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(false, false, Mode.Ready)]
    [InlineData(true, false, Mode.Prompting)]
    [InlineData(false, true, Mode.Playback)]
    [InlineData(true, true, Mode.Prompting)]
    public void ModeReady_Tests(bool handsetLifted, bool playbackPressed, Mode expectedMode)
    {
        // Arrange
        _appStatus.Mode = Mode.Ready;
        _gpioAccess.HandsetLifted.Returns(handsetLifted);
        _gpioAccess.PlaybackPressed.Returns(playbackPressed);

        // Act
        _worker.ModeReady();

        // Assert
        _appStatus.Mode.Should().Be(expectedMode);
    }

    [Theory]
    [InlineData(false, Mode.Recording)]
    [InlineData(true, Mode.Initialising)]
    public async Task ModePrompting_Tests(bool greetingCanceled, Mode expectedMode)
    {
        // Arrange
        _audioOutput
            .PlayGreetingAsync(Arg.Any<Func<bool>>(), Arg.Any<CancellationToken>())
            .Returns(greetingCanceled);

        // Act
        await _worker.ModePrompting(CancellationToken.None);

        // Assert
        _appStatus.Mode.Should().Be(expectedMode);
        if (greetingCanceled)
        {
            await _audioOutput.Received(0).PlayBeepAsync(Arg.Any<CancellationToken>());
            _audioRecorder.Received(0).Start();
        }
        else
        {
            await _audioOutput.Received(1).PlayBeepAsync(Arg.Any<CancellationToken>());
            _audioRecorder.Received(1).Start();
        }
    }


    [Theory]
    [InlineData(false, Mode.Ready)]
    [InlineData(true, Mode.Initialising)]
    public async Task ModeRecording_Tests(bool handsetLifted, Mode expectedMode)
    {
        // Arrange
        _appStatus.Mode = Mode.Initialising;
        _gpioAccess.HandsetLifted.Returns(handsetLifted);

        // Act
        await _worker.ModeRecording(CancellationToken.None);

        // Assert
        _appStatus.Mode.Should().Be(expectedMode);
    }

    [Theory]
    [InlineData("test.wav", false, Mode.Ready)]
    [InlineData("test.wav", true, Mode.Ready)]
    [InlineData("", false, Mode.Ready)]
    [InlineData(null, false, Mode.Ready)]
    public async Task ModePlayback_Tests(string? latestFile, bool playbackCanceled, Mode expectedMode)
    {
        // Arrange
        _appStatus.Mode = Mode.Playback;
        _audioRecorder.GetLatestRecordingFilePath().Returns(latestFile);
        _audioOutput
            .PlayAsync(Arg.Any<string>(), Arg.Any<Func<bool>>(), Arg.Any<CancellationToken>())
            .Returns(playbackCanceled);

        // Act
        await _worker.ModePlayback(CancellationToken.None);

        // Assert
        _appStatus.Mode.Should().Be(expectedMode);
        _audioRecorder.Received(1).GetLatestRecordingFilePath();
        if (!string.IsNullOrWhiteSpace(latestFile))
        {
            await _audioOutput.Received(1).PlayAsync(Arg.Any<string>(), Arg.Any<Func<bool>>(), Arg.Any<CancellationToken>());
        }
        else
        {
            await _audioOutput.Received(0).PlayAsync(Arg.Any<string>(), Arg.Any<Func<bool>>(), Arg.Any<CancellationToken>());
            return;
        }
        if (playbackCanceled)
        {
            await _audioOutput.Received(0).PlayBeepAsync(Arg.Any<CancellationToken>());
        }
        else
        {
            await _audioOutput.Received(1).PlayBeepAsync(Arg.Any<CancellationToken>());
        }
    }
}