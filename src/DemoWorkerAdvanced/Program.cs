using DemoWorkerAdvanced;
using DemoWorkerAdvanced.Services;
using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService;
using Helpful.Hosting.WorkerService.HostFactoryParams;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Events;


HostFactory.RunCustomWorker<CustomWorker>(new RunCustomWorkerParams{
    Args = args,
    IocDelegate = (hostContext, webHostContext, collection) =>
    {
        collection.AddScoped<IDayOfTheWeekService, DayOfTheWeekService>();
    },
    WebAppBuilderDelegate = app =>
    {
        // can be left out - this is defaulted
    },
    ListenerInfo = new []
    {
        new ListenerInfo
        {
            Port = 8152
        }
    },
    LogLevel = LogEventLevel.Debug
});