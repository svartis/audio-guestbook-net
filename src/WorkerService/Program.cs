using AudioGuestbook.WorkerService.Services;
using System.Device.Gpio;
using Microsoft.Extensions.Options;

//https://learn.microsoft.com/en-us/dotnet/core/extensions/windows-service?pivots=dotnet-7-0
//sc.exe create "AudioGuestbook Service" binpath="C:\Path\To\App.WindowsService.exe"
//sc qfailure "AudioGuestbook Service"
//sc.exe failure "AudioGuestbook Service" reset=0 actions=restart/60000/restart/60000/run/1000
//TODO: Bat file to auto install sc.exe create ".NET Joke Service" binpath="C:\Path\To\App.WindowsService.exe"
//sc.exe start "AudioGuestbook Service"
//sc.exe stop "AudioGuestbook Service"
//sc.exe delete "AudioGuestbook Service"

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
                    //TODO: GpioController for VS debug? (See VirtualGpioController in Tests project)
                    .AddSingleton<IGpioAccess, GpioAccess>(_ => new GpioAccess(new GpioController()))
                    .AddSingleton<INSoundFactory, NSoundFactory>()
                    .AddSingleton<IAudioOutput, AudioOutput>()
                    .AddSingleton<IAudioRecorder, AudioRecorder>()
                    .AddHostedService<LedStatusWorker>()
                    .AddHostedService<ProcessWorker>()
                    .AddWindowsService(options =>
                    {
                        options.ServiceName = "AudioGuestbook Service";
                    });
            });
}