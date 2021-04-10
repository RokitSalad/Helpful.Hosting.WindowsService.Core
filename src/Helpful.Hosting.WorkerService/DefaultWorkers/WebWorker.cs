using System;
using System.Threading;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService.DefaultWorkers
{
    /// <summary>
    /// The <c>WebWorker</c> class is used by default in <code>HostFactory.RunApiWorker()</code>.
    /// The <c>WebWorker</c> simply exposes an API over HTTP or HTTPS based on the <c>ListenerInfo</c> array passed into <code>HostFactory.RunApiWorker()</code>.
    /// Any controller classes in your project will be automatically loaded and exposed.
    /// The API includes a health check end point and Swagger, automatically.
    /// A consumer should not need to reference this class manually as it is used by default.
    /// </summary>
    public class WebWorker : IHostedService
    {
        private readonly Action<IApplicationBuilder> _webAppBuilderDelegate;
        private readonly Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> _iocDelegate;
        private readonly ListenerInfo[] _listenerInfo;
        private IHost _webHost;

        /// <summary>
        /// Constructor. All constructor params are passed into <code>HostFactory.RunApiWorker()</code>.
        /// </summary>
        /// <param name="webAppBuilderDelegate">A delegate to allow custom actions on the web app builder</param>
        /// <param name="iocDelegate">A delegate for setting IOC bindings</param>
        /// <param name="listenerInfo">A collection of endpoints which determine how the web host listens for requests</param>
        public WebWorker(Action<IApplicationBuilder> webAppBuilderDelegate, Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, params ListenerInfo[] listenerInfo)
        {
            _webAppBuilderDelegate = webAppBuilderDelegate;
            _iocDelegate = iocDelegate;
            _listenerInfo = listenerInfo;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _webHost = HostFactory.BuildKestrelWebHost<DefaultWebStartup>(_webAppBuilderDelegate, _iocDelegate, _listenerInfo);
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
