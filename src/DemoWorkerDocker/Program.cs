using System;
using System.Threading.Tasks;
using DemoWorkerDocker.Services;
using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService.HostFactoryParams;
using Helpful.Hosting.WorkerService.Systemd;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;

// Running the async task as a Worker Service on Windows and declaring the type of Worker class to use.
// This is passing the CompoundWorker class, but any class which implements IHostedService will work.
// This instance will also expose any controller actions you declare, along with a health check and Swagger endpoints.
HostFactory.RunCompoundWorker(new RunCompoundWorkerParams
{
    Args = args,
    WorkerProcess = async (cancellationToken) =>
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine(DateTime.Now);
            
            await Task.Delay(4000);
        }
    },
    IocDelegate = (hostContext, webHostContext, collection) =>
    {
        collection.AddScoped<IDayOfTheWeekService, DayOfTheWeekService>();
    },
    WebAppBuilderDelegate = app => { },
    LogLevel = LogEventLevel.Debug,
    ListenerInfo = new[]{
        new ListenerInfo
        {
            Port = 8151
        }
    }
});