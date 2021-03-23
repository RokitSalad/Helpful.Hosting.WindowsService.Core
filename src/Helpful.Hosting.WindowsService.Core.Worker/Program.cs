using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WindowsService.Core.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostBuilderFactory.CreateHostBuilder<DefaultWorker>(args, async (cancellationToken) =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Console.WriteLine(DateTime.Now);
                    await Task.Delay(4000);
                }
            }).Build().Run();
        }
    }
}
