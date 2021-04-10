using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService.DefaultWorkers
{
    public class BackgroundTaskWorker : BackgroundService
    {
        private readonly Func<CancellationToken, Task> _workerProcess;

        public BackgroundTaskWorker(Func<CancellationToken, Task> workerProcess)
        {
            _workerProcess = workerProcess;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _workerProcess(stoppingToken);
            }
        }
    }
}