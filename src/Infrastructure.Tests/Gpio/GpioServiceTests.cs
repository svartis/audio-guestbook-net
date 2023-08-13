using AudioGuestbook.Infrastructure.Sound;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioGuestbook.Infrastructure.Gpio;

namespace AudioGuestbook.Infrastructure.Tests.Gpio;

public class GpioServiceTests
{
    private readonly IGpioService _service;

    public GpioServiceTests()
    {
        //var logger = Substitute.For<ILogger<SoundService>>();
        //var soundSettings = new SoundSettings
        //{
        //    MasterVolume = 0f
        //};
        _service = new GpioService(/*logger, soundSettings*/);
    }

    [Fact]
    public void A()
    {
        var a = _service.IsPinOpen();
    }
}