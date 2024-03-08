using AudioGuestbook.WorkerService.Enums;
using AudioGuestbook.WorkerService.Services;
using System.Diagnostics;

namespace AudioGuestbook.WorkerService;

public sealed class ProcessWorker(
    ILogger<ProcessWorker> logger,
    IAppStatus appStatus,
    IAudioOutput audioOutput,
    IAudioRecorder audioRecorder,
    IGpioAccess gpioAccess,
    AppSettings appSettings)
    : BackgroundService
{
    private readonly Stopwatch _recordingStopwatch = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Set Initialising mode
        appStatus.Mode = Mode.Initialising;

        // PlayAsync startup sound
        await audioOutput.PlayStartupAsync(stoppingToken);

        // Set ready mode
        appStatus.Mode = Mode.Ready;

        while (!stoppingToken.IsCancellationRequested)
        {
            await SwitchModes(appStatus.Mode, stoppingToken);
            await Task.Delay(50, stoppingToken);
        }
    }

    internal async Task SwitchModes(Mode mode, CancellationToken cancellationToken)
    {
        switch (mode)
        {
            case Mode.Initialising:
                // Do nothing
                break;
            case Mode.Ready:
                ModeReady();
                break;
            case Mode.Prompting:
                await ModePrompting(cancellationToken);
                break;
            case Mode.Recording:
                await ModeRecording(cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }
    }

    internal void ModeReady()
    {
        if (gpioAccess.HandsetLifted)
        {
            logger.LogInformation("Handset lifted");
            appStatus.Mode = Mode.Prompting;
        }
    }

    internal async Task ModePrompting(CancellationToken cancellationToken)
    {
        // Wait a second for users to put the handset to their ear
        await Task.Delay(appSettings.PromptingDelay, cancellationToken);

        // PlayAsync the greeting inviting them to record their message
        var promptingCanceled = await audioOutput.PlayGreetingAsync(() =>
        {
            // Check whether the handset is replaced
            if (!gpioAccess.HandsetLifted)
            {
                appStatus.Mode = Mode.Ready;
                return true;
            }
            return false;
        }, cancellationToken);

        if (promptingCanceled)
        {
            return;
        }

        // PlayAsync the tone sound effect
        await audioOutput.PlayBeepAsync(cancellationToken);

        // Start recording
        RecordingStart();
    }

    internal async Task ModeRecording(CancellationToken cancellationToken)
    {
        // Handset is replaced or recording limit exceeded
        if (!gpioAccess.HandsetLifted || _recordingStopwatch.Elapsed.Seconds > appSettings.RecordLimitInSeconds)
        {
            // Stop recording
            RecordingStop();

            // PlayAsync audio tone to confirm recording has ended
            await audioOutput.PlayBeepAsync(cancellationToken);

            appStatus.Mode = Mode.Ready;
        }
    }

    internal void RecordingStart()
    {
        audioRecorder.Start();
        _recordingStopwatch.Reset();
        _recordingStopwatch.Start();
        appStatus.Mode = Mode.Recording;
    }

    private void RecordingStop()
    {
        audioRecorder.Stop();
        _recordingStopwatch.Stop();
        _recordingStopwatch.Reset();
    }
}