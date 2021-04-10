using System;
using System.Threading;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService.DefaultWorkers
{
    /// <summary>
    /// The <c>CustomWorkerBase</c> abstract class can be inherited to allow more complicated background tasks than <c>CompoundWorker</c> is designed for.
    /// <c>CustomWorkerBase</c> is designed specifically with IOC in mind. Create a derived type which overrides <code>CustomWorkerLogic()</code> and includes
    /// your custom resources injected via constructor. Define the relevant IOC bindings in the iocDelegate when calling <code>HostFactory.RunCustomWorker<TWorker>()</TWorker></code>.
    /// An example of usage is in the DemoWorkerAdvanced sample project.
    /// Any derived worker will run with API endpoints, including Swagger and a health check.
    /// </summary>
    public abstract class CustomWorkerBase : BackgroundService
    {
        private readonly Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> _iocDelegate;
        private readonly ListenerInfo[] _listenerInfo;
        private IHost _webHost;

        /// <summary>
        /// Constructor. All constructor params are passed into <code>HostFactory.RunCompoundWorker()</code>.
        /// </summary>
        /// <param name="iocDelegate">A delegate for setting IOC bindings</param>
        /// <param name="listenerInfo">A collection of endpoints which determine how the web host listens for requests</param>
        protected CustomWorkerBase(Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, params ListenerInfo[] listenerInfo)
        {
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
                    await CustomWorkerLogic(stoppingToken);
                }

                await _webHost.StopAsync(stoppingToken);
            }
            finally
            {
                _webHost?.Dispose();
            }
        }

        /// <summary>
        /// Override this method as the entry point to your service logic.
        /// </summary>
        /// <param name="stoppingToken">The cancellation token - managed within the <c>BackgroundService</c> class</param>
        /// <returns><c>Task</c> - the intention is for overrides to be async/await</returns>
        protected abstract Task CustomWorkerLogic(CancellationToken stoppingToken);
    }
}
