using AudioGuestbook.WorkerService.Services;
using System.Device.Gpio;
using Microsoft.Extensions.Options;

//TODO: Maximum timeout for recording?
//TODO: Bat file to auto install sc.exe create ".NET Joke Service" binpath="C:\Path\To\App.WindowsService.exe"
//sc.exe start ".NET Joke Service"
//sc.exe stop ".NET Joke Service"
//sc.exe delete ".NET Joke Service"
//TODO: Final update of diagram

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
            .ConfigureServices((context, services) =>
            {
                services
                    .Configure<AppSettings>(context.Configuration.GetSection(nameof(AppSettings)))
                    .AddSingleton(resolver => resolver.GetRequiredService<IOptions<AppSettings>>().Value)
                    .AddSingleton<IAppStatus, AppStatus>()
                    //TODO: GpioController for VS debug?
                    .AddSingleton<IGpioAccess, GpioAccess>(_ => new GpioAccess(new GpioController()))
                    .AddSingleton<INSoundFactory, NSoundFactory>()
                    .AddSingleton<IAudioOutput, AudioOutput>()
                    .AddSingleton<IAudioRecorder, AudioRecorder>()
                    .AddHostedService<LedStatusWorker>()
                    .AddHostedService<ProcessWorker>();
            });
}