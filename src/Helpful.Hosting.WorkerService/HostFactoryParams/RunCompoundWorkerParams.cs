using System;
using System.Threading;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;

namespace Helpful.Hosting.WorkerService.HostFactoryParams
{
    public class RunCompoundWorkerParams
    {
        public ListenerInfo[] ListenerInfo { get; set; }
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
        public Action<IApplicationBuilder> WebAppBuilderDelegate { get; set; } = app => { };
        public Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> IocDelegate { get; set; } =
            (hostContext, webHostContext, services) => { };
        public Func<CancellationToken, Task> WorkerProcess { get; set; }
        public string[] Args { get; set; } = { };
    }
}