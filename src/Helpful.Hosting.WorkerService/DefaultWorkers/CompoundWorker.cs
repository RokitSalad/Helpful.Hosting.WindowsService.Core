using System;
using System.Threading;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService.DefaultWorkers
{
    public class CompoundWorker : BackgroundService
    {
        private readonly Func<CancellationToken, Task> _workerProcess;
        private readonly ListenerInfo[] _listenerInfo;
        private IHost _webHost;

        public CompoundWorker(Func<CancellationToken, Task> workerProcess, params ListenerInfo[] listenerInfo)
        {
            _workerProcess = workerProcess;
            _listenerInfo = listenerInfo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _webHost = HostFactory.BuildKestrelWebHost<DefaultWebStartup>(_listenerInfo);
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
