using System;
using System.Threading;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService.DefaultWorkers
{
    public class WebWorker : IHostedService
    {
        private readonly Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> _iocDelegate;
        private readonly ListenerInfo[] _listenerInfo;
        private IHost _webHost;

        public WebWorker(Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, params ListenerInfo[] listenerInfo)
        {
            _iocDelegate = iocDelegate;
            _listenerInfo = listenerInfo;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _webHost = HostFactory.BuildKestrelWebHost<DefaultWebStartup>(_iocDelegate, _listenerInfo);
            await _webHost.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _webHost.StopAsync(cancellationToken);
            }
            finally
            {
                _webHost.Dispose();
            }
        }
    }
}
