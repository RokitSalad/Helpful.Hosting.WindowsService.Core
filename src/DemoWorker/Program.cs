using System;
using System.Threading.Tasks;
using Helpful.Hosting.WorkerService;

namespace DemoWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            WorkerProcessRunner.Run<DefaultWorker>(args, async (cancellationToken) =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine(DateTime.Now);
                    await Task.Delay(4000);
                }
            });
        }
    }
}
