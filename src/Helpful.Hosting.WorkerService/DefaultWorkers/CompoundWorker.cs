using System;
using System.Threading;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService.DefaultWorkers
{
    public class CompoundWorker : BackgroundService
    {
        private readonly Func<CancellationToken, Task> _workerProcess;
        private readonly Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> _iocDelegate;
        private readonly ListenerInfo[] _listenerInfo;
        private IHost _webHost;

        public CompoundWorker(Func<CancellationToken, Task> workerProcess, Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, params ListenerInfo[] listenerInfo)
        {
            _workerProcess = workerProcess;
            _iocDelegate = iocDelegate;
            _listenerInfo = listenerInfo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _webHost = HostFactory.BuildKestrelWebHost<DefaultWebStartup>(_iocDelegate, _listenerInfo);
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
