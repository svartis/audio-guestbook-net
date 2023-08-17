using AudioGuestbook.WorkerService;
using AudioGuestbook.WorkerService.Services;

//TODO: Seal all classes
//TODO: Create collected settings
//TODO: Git action, build, test, sonarqube
//TODO: ReadMe
//TODO: Tests
//TODO: fritzing diagram 

try
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services
                .AddSingleton<IAppStatus, AppStatus>()
                .AddSingleton<IGpioAccess, GpioAccess>()
                .AddSingleton<IAudioOutput, AudioOutput>()
                .AddSingleton<IAudioRecorder, AudioRecorder>()
                .AddHostedService<LedStatusWorker>()
                .AddHostedService<ProcessWorker>();

        })
        .Build();

    host.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
}