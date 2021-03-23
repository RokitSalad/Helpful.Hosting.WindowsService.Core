using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WindowsService.Core.Worker
{
    public class DefaultWorker : BackgroundService
    {
        private readonly Func<object, CancellationToken, Task> _workerProcess;
        private readonly object _state;

        public DefaultWorker(Func<object, CancellationToken, Task> workerProcess, object state)
        {
            _workerProcess = workerProcess;
            _state = state;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _workerProcess(_state, stoppingToken);
            }
        }
    }
}
