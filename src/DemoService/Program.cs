﻿using System;
using Helpful.Hosting.WindowsService.Core;
using Topshelf;

namespace DemoService
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service(() => new BasicWebService<CustomStartup>("http://localhost:5001"));
                x.EnableServiceRecovery(r => r.RestartService(TimeSpan.FromSeconds(10)));
                x.SetServiceName("DemoService");
            });
        }
    }
}