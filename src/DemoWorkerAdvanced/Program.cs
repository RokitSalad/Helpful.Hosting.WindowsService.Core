using DemoWorkerAdvanced;
using DemoWorkerAdvanced.Services;
using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;

HostFactory.RunCustomWorker<CustomWorker>(args, (hostContext, webHostContext, collection) =>
    {
        collection.AddScoped<IDayOfTheWeekService, DayOfTheWeekService>();
    }, LogEventLevel.Debug, new ListenerInfo
    {
        Port = 8053
    }
);