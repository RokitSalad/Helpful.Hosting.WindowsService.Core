﻿using System;
using System.Threading.Tasks;
using Helpful.Hosting.WorkerService.HostFactoryParams;
using Helpful.Hosting.WorkerService.Windows;
using Serilog.Events;

// Running the async task as a Worker Service on Windows and declaring the type of Worker class to use.
// This is passing the BackgroundTaskWorker class, but any class which implements IHostedService will work.
// This instance will not expose any controller actions, and will not include Swagger nor a health check.
HostFactory.RunBackgroundTaskWorker(new RunBackgroundTaskWorkerParams
{
    Args = args,
    LogLevel = LogEventLevel.Debug,
    IocDelegate = (hostContext, webHostContext, collection) => { },
    WorkerProcess = async (cancellationToken) =>
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine(DateTime.Now);
            await Task.Delay(4000);
        }
    }
});