using AudioGuestbook.WorkerService.Enums;
using AudioGuestbook.WorkerService.Services;
using NAudio.Wave;

namespace AudioGuestbook.WorkerService;

public sealed class ProcessWorker : BackgroundService
{
    private readonly ILogger<ProcessWorker> _logger;
    private readonly IAppStatus _appStatus;
    private readonly IAudioOutput _audioOutput;
    private readonly IAudioRecorder _audioRecorder;
    private readonly IGpioAccess _gpioAccess;

    private readonly float _masterVolume = 0.5f;

    private readonly string _greetingAudioFile;

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

        var systemMediaFolderPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!, "Media");
        _greetingAudioFile = Path.Combine(systemMediaFolderPath, "greeting.wav");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Required for the method to be executed asynchronously, allowing startup to continue.
        await Task.Yield();
        await Task.Delay(100, stoppingToken);

        // Set Initialising mode
        _appStatus.SetMode(Mode.Initialising);

        // Play startup sound
        await _audioOutput.PlayStartupAsync(stoppingToken);

        // Set ready mode
        _appStatus.SetMode(Mode.Ready);

        while (!stoppingToken.IsCancellationRequested)
        {
            switch (_appStatus.GetMode())
            {
                case Mode.Initialising:
                    // Do nothing
                    break;
                case Mode.Ready:
                    if (_gpioAccess.HandsetLifted)
                    {
                        _logger.LogInformation("Handset lifted");
                        _appStatus.SetMode(Mode.Prompting);
                    }
                    break;
                case Mode.Prompting:
                    // Wait a second for users to put the handset to their ear
                    await Task.Delay(1000, stoppingToken);

                    // Play the greeting inviting them to record their message
                    //TODO: Move to audio service
                    using (var audioFile = new AudioFileReader(_greetingAudioFile))
                    using (var outputDevice = new WaveOutEvent())
                    {
                        outputDevice.Volume = _masterVolume;
                        outputDevice.Init(audioFile);
                        outputDevice.Play();
                        _logger.LogInformation("Wait until the message has finished playing");
                        while (outputDevice.PlaybackState == PlaybackState.Playing)
                        {
                            // Check whether the handset is replaced
                            if (!_gpioAccess.HandsetLifted)
                            {
                                _logger.LogInformation("Stop audio");
                                outputDevice.Stop();
                                _appStatus.SetMode(Mode.Ready);
                                goto AbortPrompting;
                            }

                            await Task.Delay(50, stoppingToken);
                        }
                    }

                    // Debug message
                    _logger.LogInformation("Starting Recording");

                    // Play the tone sound effect
                    await _audioOutput.PlayBeepAsync(stoppingToken);

                    _logger.LogInformation("Start the recording function");
                    _audioRecorder.Start();
                    _appStatus.SetMode(Mode.Recording);
                AbortPrompting:
                    break;
                case Mode.Recording:
                    // Handset is replaced
                    if (!_gpioAccess.HandsetLifted)
                    {
                        // Debug log
                        _logger.LogInformation("Stopping Recording");
                        // Stop recording
                        _audioRecorder.Stop();
                        // Play audio tone to confirm recording has ended
                        await _audioOutput.PlayBeepAsync(stoppingToken);

                        _appStatus.SetMode(Mode.Ready);
                    }
                    break;
                case Mode.Playing:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            await Task.Delay(50, stoppingToken);
        }
    }
}