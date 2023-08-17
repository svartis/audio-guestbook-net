using AudioGuestbook.WorkerService.Enums;
using AudioGuestbook.WorkerService.Services;
using NAudio.Wave;

namespace AudioGuestbook.WorkerService;

public sealed class ProcessWorker : BackgroundService
{
    private readonly ILogger<ProcessWorker> _logger;
    private readonly IAppStatus _appStatus;
    private readonly IAudioOutput _audioOutput;
    private readonly IGpioAccess _gpioAccess;

    private readonly float _masterVolume = 0.5f;

    private readonly string _greetingAudioFile;

    public ProcessWorker(ILogger<ProcessWorker> logger, IAppStatus appStatus, IAudioOutput audioOutput, IGpioAccess gpioAccess)
    {
        _logger = logger;
        _appStatus = appStatus;
        _audioOutput = audioOutput;
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
                    StartRecording();
                AbortPrompting:
                    break;
                case Mode.Recording:
                    // Handset is replaced
                    if (!_gpioAccess.HandsetLifted)
                    {
                        // Debug log
                        _logger.LogInformation("Stopping Recording");
                        // Stop recording
                        StopRecording();
                        // Play audio tone to confirm recording has ended
                        await _audioOutput.PlayBeepAsync(stoppingToken);
                    }
                    else
                    {
                        ContinueRecording();
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

    private void ContinueRecording()
    {
        //#if defined(INSTRUMENT_SD_WRITE)
        //  uint32_t started = micros();
        //#endif // defined(INSTRUMENT_SD_WRITE)
        //#define NBLOX 16  
        //        // Check if there is data in the queue
        //        if (queue1.available() >= NBLOX)
        //        {
        //            byte buffer[NBLOX * AUDIO_BLOCK_SAMPLES * sizeof(int16_t)];
        //            // Fetch 2 blocks from the audio library and copy
        //            // into a 512 byte buffer.  The Arduino SD library
        //            // is most efficient when full 512 byte sector size
        //            // writes are used.
        //            for (int i = 0; i < NBLOX; i++)
        //            {
        //                memcpy(buffer + i * AUDIO_BLOCK_SAMPLES * sizeof(int16_t), queue1.readBuffer(), AUDIO_BLOCK_SAMPLES * sizeof(int16_t));
        //                queue1.freeBuffer();
        //            }
        //            // Write all 512 bytes to the SD card
        //            frec.write(buffer, sizeof buffer);
        //            recByteSaved += sizeof buffer;
        //        }

        //#if defined(INSTRUMENT_SD_WRITE)
        //  started = micros() - started;
        //  if (started > worstSDwrite)
        //    worstSDwrite = started;

        //  if (millis() >= printNext)
        //  {
        //    Serial.printf("Worst write took %luus\n",worstSDwrite);
        //    worstSDwrite = 0;
        //    printNext = millis()+250;
        //  }
        //#endif // defined(INSTRUMENT_SD_WRITE)
    }

    private void StopRecording()
    {
        //// Stop adding any new data to the queue
        //queue1.end();
        //// Flush all existing remaining data from the queue
        //while (queue1.available() > 0)
        //{
        //    // Save to open file
        //    frec.write((byte*)queue1.readBuffer(), AUDIO_BLOCK_SAMPLES * sizeof(int16_t));
        //    queue1.freeBuffer();
        //    recByteSaved += AUDIO_BLOCK_SAMPLES * sizeof(int16_t);
        //}
        //writeOutHeader();
        //// Close the file
        //frec.close();
        //Serial.println("Closed file");
        _appStatus.SetMode(Mode.Ready);
        //setMTPdeviceChecks(true); // enable MTP device checks, recording is finished
    }

    private void StartRecording()
    {
        //setMTPdeviceChecks(false); // disable MTP device checks while recording
        //// Find the first available file number
        ////  for (uint8_t i=0; i<9999; i++) { // BUGFIX uint8_t overflows if it reaches 255  
        //for (uint16_t i = 0; i < 9999; i++)
        //{
        //    // Format the counter as a five-digit number with leading zeroes, followed by file extension
        //    snprintf(filename, 11, " %05d.wav", i);
        //    // Create if does not exist, do not open existing, write, sync after write
        //    if (!SD.exists(filename))
        //    {
        //        break;
        //    }
        //}
        //frec = SD.open(filename, FILE_WRITE);
        //Serial.println("Opened file !");
        //if (frec)
        //{
        //    Serial.print("Recording to ");
        //    Serial.println(filename);
        //    queue1.begin();
        _appStatus.SetMode(Mode.Recording);
        //    recByteSaved = 0L;
        //}
        //else
        //{
        //    Serial.println("Couldn't open file to record!");
        //}
    }
}