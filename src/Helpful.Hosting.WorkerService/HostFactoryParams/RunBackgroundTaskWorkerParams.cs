using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;

namespace Helpful.Hosting.WorkerService.HostFactoryParams
{
    public class RunBackgroundTaskWorkerParams
    {
        public string[] Args { get; set; } = { };
        public Func<CancellationToken, Task> WorkerProcess { get; set; }
        public Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> IocDelegate { get; set; } =
            (hostContext, webHostContext, services) => { };
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
    }
}