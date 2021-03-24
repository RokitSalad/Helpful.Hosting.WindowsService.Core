using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService
{
    public class DefaultWebWorker : BackgroundService
    {
        public DefaultWebWorker()
        {
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
            }
        }
    }
}
