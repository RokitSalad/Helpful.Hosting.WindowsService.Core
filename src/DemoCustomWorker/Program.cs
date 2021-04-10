using System;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService;
using Serilog.Events;

HostFactory.RunCompoundWorker(args, async (cancellationToken) =>
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine(DateTime.Now);
            await Task.Delay(4000);
        }
    }, (hostContext, webHostContext, collection) => { }, LogEventLevel.Debug, new ListenerInfo
    {
        Port = 8053
    }
);
