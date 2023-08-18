using AudioGuestbook.WorkerService.Services;

//TODO: Seal all classes
//TODO: Create collected settings
//TODO: ReadMe
//TODO: Tests
//TODO: Fritzing diagram 
//TODO: Timeout for recording
//TODO: TagetFramework latest to allow build on linux

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