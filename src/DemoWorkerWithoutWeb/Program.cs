using System;
using System.Threading.Tasks;
using Helpful.Hosting.WorkerService;
using Serilog.Events;

WorkerProcessRunner.RunWithoutWeb<DefaultWorkerWithoutWeb>(args, async (cancellationToken) =>
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine(DateTime.Now);
            await Task.Delay(4000);
        }
    }, LogEventLevel.Debug
);
