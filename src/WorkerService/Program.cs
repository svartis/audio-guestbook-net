using AudioGuestbook.WorkerService.Services;

//TODO: Seal all classes

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
                    .AddHostedService<LedStatusWorker>()
                    .AddHostedService<ProcessWorker>();
            });
}