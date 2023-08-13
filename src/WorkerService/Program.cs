using AudioGuestbook.Infrastructure.Sound;
using AudioGuestbook.WorkerService;
using AudioGuestbook.WorkerService.Extensions;

//TODO: Seal all classes

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services
            .AddAppSettings<SoundSettings>(context.Configuration.GetSection(SoundSettings.SectionKey))
            .AddSingleton<ISoundService, SoundService>()
            .AddHostedService<Worker>();

    })
    .Build();

host.Run();
