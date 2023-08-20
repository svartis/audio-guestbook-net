using AudioGuestbook.WorkerService.Enums;
using AudioGuestbook.WorkerService.Services;

namespace AudioGuestbook.WorkerService;

public sealed class ProcessWorker : BackgroundService
{
    private readonly ILogger<ProcessWorker> _logger;
    private readonly IAppStatus _appStatus;
    private readonly IAudioOutput _audioOutput;
    private readonly IAudioRecorder _audioRecorder;
    private readonly IGpioAccess _gpioAccess;

    public ProcessWorker(ILogger<ProcessWorker> logger,
        IAppStatus appStatus,
        IAudioOutput audioOutput,
        IAudioRecorder audioRecorder,
        IGpioAccess gpioAccess)
    {
        _logger = logger;
        _appStatus = appStatus;
        _audioOutput = audioOutput;
        _audioRecorder = audioRecorder;
        _gpioAccess = gpioAccess;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Set Initialising mode
        _appStatus.Mode = Mode.Initialising;

        // Required for the method to be executed asynchronously, allowing startup to continue.
        await Task.Yield();
        await Task.Delay(100, stoppingToken);

        // PlayAsync startup sound
        await _audioOutput.PlayStartupAsync(stoppingToken);

        // Set ready mode
        _appStatus.Mode = Mode.Ready;

        while (!stoppingToken.IsCancellationRequested)
        {
            switch (_appStatus.Mode)
            {
                case Mode.Initialising:
                    // Do nothing
                    break;
                case Mode.Ready:
                    if (_gpioAccess.HandsetLifted)
                    {
                        _logger.LogInformation("Handset lifted");
                        _appStatus.Mode = Mode.Prompting;
                    }
                    else if (_gpioAccess.PlaybackPressed)
                    {
                        _logger.LogInformation("Playback pressed");
                        _appStatus.Mode = Mode.Playback;
                    }
                    break;
                case Mode.Prompting:
                    // Wait a second for users to put the handset to their ear
                    await Task.Delay(1000, stoppingToken);

                    // PlayAsync the greeting inviting them to record their message
                    var promptingCanceled = await _audioOutput.PlayGreetingAsync(() =>
                    {
                        // Check whether the handset is replaced
                        if (!_gpioAccess.HandsetLifted)
                        {
                            _appStatus.Mode = Mode.Ready;
                            return true;
                        }

                        // Playback button pressed
                        if (_gpioAccess.PlaybackPressed)
                        {
                            _appStatus.Mode = Mode.Playback;
                            return true;
                        }

                        return false;
                    }, stoppingToken);

                    if (promptingCanceled)
                    {
                        break;
                    }

                    // Debug message
                    _logger.LogInformation("Starting Recording");

                    // PlayAsync the tone sound effect
                    await _audioOutput.PlayBeepAsync(stoppingToken);

                    _logger.LogInformation("Start the recording function");
                    _audioRecorder.Start();
                    _appStatus.Mode = Mode.Recording;
                    break;
                case Mode.Recording:
                    // Handset is replaced
                    if (!_gpioAccess.HandsetLifted)
                    {
                        // Debug log
                        _logger.LogInformation("Stopping Recording");
                        // Stop recording
                        _audioRecorder.Stop();
                        // PlayAsync audio tone to confirm recording has ended
                        await _audioOutput.PlayBeepAsync(stoppingToken);

                        _appStatus.Mode = Mode.Ready;
                    }
                    break;
                case Mode.Playback:
                    // Find latest recording
                    var fileName = _audioRecorder.GetLatestRecordingFilePath();

                    // Skip if none found
                    if (string.IsNullOrWhiteSpace(fileName))
                    {
                        _appStatus.Mode = Mode.Ready;
                        return;
                    }

                    // PlayAsync latest file
                    var playbackCanceled = await _audioOutput.PlayAsync(fileName, () =>
                    {
                        // Playback button is released or Handset is replaced
                        if (!_gpioAccess.PlaybackPressed || !_gpioAccess.HandsetLifted)
                        {
                            return true;
                        }
                        return false;
                    }, stoppingToken);

                    if (playbackCanceled)
                    {
                        _appStatus.Mode = Mode.Ready;
                        break;
                    }

                    // file has been played
                    await _audioOutput.PlayBeepAsync(stoppingToken);
                    _appStatus.Mode = Mode.Ready;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Task.Delay(50, stoppingToken);
        }
    }
}