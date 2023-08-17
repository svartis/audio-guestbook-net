using AudioGuestbook.WorkerService.Services;

//TODO: Seal all classes
//TODO: Create collected settings
//TODO: Git action, build, test, sonarqube
//TODO: ReadMe
//TODO: Tests
//TODO: fritzing diagram 

namespace AudioGuestbook.WorkerService;

internal static class Program
{
    internal static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    internal static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services
                    .AddSingleton<IAppStatus, AppStatus>()
                    .AddSingleton<IGpioAccess, GpioAccess>()
                    .AddSingleton<IAudioOutput, AudioOutput>()
                    .AddSingleton<IAudioRecorder, AudioRecorder>()
                    .AddHostedService<LedStatusWorker>()
                    .AddHostedService<ProcessWorker>();
            });
}