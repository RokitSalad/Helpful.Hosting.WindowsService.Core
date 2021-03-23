using System;
using System.Threading.Tasks;
using Helpful.Hosting.WorkerService;

WorkerProcessRunner.Run<DefaultWorker>(args, async (cancellationToken) =>
{
    while (!cancellationToken.IsCancellationRequested)
    {
        Console.WriteLine(DateTime.Now);
        await Task.Delay(4000);
    }
});