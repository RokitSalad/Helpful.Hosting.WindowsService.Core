﻿using System;
using System.Threading.Tasks;
using DemoWorker;
using DemoWorker.Services;
using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;

// Running the async task as a Worker Service on Windows and declaring the type of Worker class to use.
// This is passing the CompoundWorker class, but any class which implements IHostedService will work.
// This instance will also expose any controller actions you declare, along with a health check and Swagger endpoints.
HostFactory.RunCompoundWorker(args, async (cancellationToken) =>
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine(DateTime.Now);
            await Task.Delay(4000);
        }
    }, (hostContext, webHostContext, collection) =>
    {
        collection.AddScoped<IDayOfTheWeekService, DayOfTheWeekService>();
    }, app => {}, LogEventLevel.Debug, new ListenerInfo
        {
            Port = 8053
        }
);