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
    /// The <c>CompoundWorker</c> class is used by default in <code>HostFactory.RunCompoundWorker()</code>.
    /// The <c>CompoundWorker</c> runs a simple Task in the background, and exposes an API with a health check and Swagger endpoints.
    /// Any controller classes in your project will be loaded automatically and exposed based on the <c>ListenerInfo[]</c>.
    /// The workerProcess is passed in from the consuming assembly and is executed asynchronously over and over until the <c>CancellationToken</c> is flagged as cancelled.
    /// A consumer should not need to reference this class manually as it is used by default.
    /// </summary>
    public class CompoundWorker : BackgroundService
    {
        private readonly Action<IApplicationBuilder> _webAppBuilderDelegate;
        private readonly Func<CancellationToken, Task> _workerProcess;
        private readonly Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> _iocDelegate;
        private readonly ListenerInfo[] _listenerInfo;
        private IHost _webHost;

        /// <summary>
        /// Constructor. All constructor params are passed into <code>HostFactory.RunCompoundWorker()</code>.
        /// </summary>
        /// <param name="webAppBuilderDelegate">A delegate to allow custom actions on the web app builder</param>
        /// <param name="workerProcess">The async process to run until the task is cancelled</param>
        /// <param name="iocDelegate">A delegate for setting IOC bindings</param>
        /// <param name="listenerInfo">A collection of endpoints which determine how the web host listens for requests</param>
        public CompoundWorker(Action<IApplicationBuilder> webAppBuilderDelegate, 
            Func<CancellationToken, Task> workerProcess, 
            Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, 
            params ListenerInfo[] listenerInfo)
        {
            _webAppBuilderDelegate = webAppBuilderDelegate;
            _workerProcess = workerProcess;
            _iocDelegate = iocDelegate;
            _listenerInfo = listenerInfo;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _webHost = HostFactory.BuildKestrelWebHost<DefaultWebStartup>(_webAppBuilderDelegate, _iocDelegate, _listenerInfo);
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
