using AudioGuestbook.WorkerService;
using AudioGuestbook.WorkerService.Services;

//TODO: Seal all classes

try
{
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services
                .AddSingleton<IAppStatus, AppStatus>()
                .AddSingleton<IGpioAccess, GpioAccess>()
                .AddSingleton<IAudioOutput, AudioOutput>()
                .AddHostedService<LedStatusWorker>()
                .AddHostedService<ProcessWorker>();

        })
        .Build();

    host.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    Console.ReadLine();
}