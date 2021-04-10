using System.Threading;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService.DefaultWorkers
{
    public class WebWorker : IHostedService
    {
        private readonly ListenerInfo[] _listenerInfo;
        private IHost _webHost;

        public WebWorker(params ListenerInfo[] listenerInfo)
        {
            _listenerInfo = listenerInfo;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _webHost = WorkerProcessRunner.BuildKestrelWebHost<DefaultWebStartup>(_listenerInfo);
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
