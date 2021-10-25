using System;
using Helpful.Hosting.Dto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;

namespace Helpful.Hosting.WorkerService.HostFactoryParams
{
    public class RunCustomWorkerParams
    {
        public string[] Args { get; set; } = { };

        public Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> IocDelegate { get; set; } =
            (hostContext, webHostContext, services) => { };

        public Action<IApplicationBuilder> WebAppBuilderDelegate { get; set; } = app => { };
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
        public ListenerInfo[] ListenerInfo { get; set; }
    }
}