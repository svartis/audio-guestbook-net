using AudioGuestbook.WorkerService.Enums;
using AudioGuestbook.WorkerService.Services;
using System.Diagnostics;

namespace AudioGuestbook.WorkerService;

public sealed class ProcessWorker : BackgroundService
{
    private readonly ILogger<ProcessWorker> _logger;
    private readonly IAppStatus _appStatus;
    private readonly IAudioOutput _audioOutput;
    private readonly IAudioRecorder _audioRecorder;
    private readonly IGpioAccess _gpioAccess;
    private readonly AppSettings _appSettings;
    private readonly Stopwatch _recordingStopwatch = new();

    public ProcessWorker(ILogger<ProcessWorker> logger,
        IAppStatus appStatus,
        IAudioOutput audioOutput,
        IAudioRecorder audioRecorder,
        IGpioAccess gpioAccess,
        AppSettings appSettings)
    {
        _logger = logger;
        _appStatus = appStatus;
        _audioOutput = audioOutput;
        _audioRecorder = audioRecorder;
        _gpioAccess = gpioAccess;
        _appSettings = appSettings;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Set Initialising mode
        _appStatus.Mode = Mode.Initialising;

        // PlayAsync startup sound
        await _audioOutput.PlayStartupAsync(stoppingToken);

        // Set ready mode
        _appStatus.Mode = Mode.Ready;

        while (!stoppingToken.IsCancellationRequested)
        {
            await SwitchModes(_appStatus.Mode, stoppingToken);
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
        if (_gpioAccess.HandsetLifted)
        {
            _logger.LogInformation("Handset lifted");
            _appStatus.Mode = Mode.Prompting;
        }
    }

    internal async Task ModePrompting(CancellationToken cancellationToken)
    {
        // Wait a second for users to put the handset to their ear
        await Task.Delay(_appSettings.PromptingDelay, cancellationToken);

        // PlayAsync the greeting inviting them to record their message
        var promptingCanceled = await _audioOutput.PlayGreetingAsync(() =>
        {
            // Check whether the handset is replaced
            if (!_gpioAccess.HandsetLifted)
            {
                _appStatus.Mode = Mode.Ready;
                return true;
            }
            return false;
        }, cancellationToken);

        if (promptingCanceled)
        {
            return;
        }

        // PlayAsync the tone sound effect
        await _audioOutput.PlayBeepAsync(cancellationToken);

        // Start recording
        RecordingStart();
    }

    internal async Task ModeRecording(CancellationToken cancellationToken)
    {
        // Handset is replaced or recording limit exceeded
        if (!_gpioAccess.HandsetLifted || _recordingStopwatch.Elapsed.Seconds > _appSettings.RecordLimitInSeconds)
        {
            // Stop recording
            RecordingStop();

            // PlayAsync audio tone to confirm recording has ended
            await _audioOutput.PlayBeepAsync(cancellationToken);

            _appStatus.Mode = Mode.Ready;
        }
    }

    internal void RecordingStart()
    {
        _audioRecorder.Start();
        _recordingStopwatch.Reset();
        _recordingStopwatch.Start();
        _appStatus.Mode = Mode.Recording;
    }

    private void RecordingStop()
    {
        _audioRecorder.Stop();
        _recordingStopwatch.Stop();
        _recordingStopwatch.Reset();
    }
}