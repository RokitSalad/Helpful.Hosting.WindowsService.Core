using System;
using System.Threading;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService
{
    public class DefaultWorker : BackgroundService
    {
        private readonly Func<CancellationToken, Task> _workerProcess;
        private IHost _webHost;

        public DefaultWorker(Func<CancellationToken, Task> workerProcess)
        {
            _workerProcess = workerProcess;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _webHost = WorkerProcessRunner.BuildKestrelWebHost<DefaultWebStartup>(new ListenerInfo
                {
                    Port = 8053
                });
                await _webHost.StartAsync(stoppingToken);
                while (!stoppingToken.IsCancellationRequested)
                {
                    await _workerProcess(stoppingToken);
                }

                await _webHost.StopAsync(stoppingToken);
            }
            finally
            {
                _webHost?.Dispose();
            }
        }
    }
}
