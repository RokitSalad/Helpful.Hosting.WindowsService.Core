using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService.DefaultWorkers;
using Helpful.Hosting.WorkerService.HostFactoryParams;
using Helpful.Logging.Standard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService.Windows
{
    public static class HostFactory
    {
        public static void RunCustomWorker<TWorker>(RunCustomWorkerParams runCustomWorkerParams) where TWorker : class, IHostedService
        {
            ConfigureLogger.StandardSetup(logLevel: runCustomWorkerParams.LogLevel);
            Host.CreateDefaultBuilder(runCustomWorkerParams.Args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    runCustomWorkerParams.IocDelegate(hostContext, null, services);
                    services.AddSingleton(provider => runCustomWorkerParams.ListenerInfo);
                    services.AddSingleton(provider => runCustomWorkerParams.IocDelegate);
                    services.AddSingleton(provider => runCustomWorkerParams.WebAppBuilderDelegate);
                    services.AddHostedService<TWorker>();
                })
                .Build().Run();
        }

        public static void RunCompoundWorker(RunCompoundWorkerParams runCompoundWorkerParams)
        {
            RunCompoundWorker<CompoundWorker>(runCompoundWorkerParams);
        }

        public static void RunCompoundWorker<TWorker>(RunCompoundWorkerParams runCompoundWorkerParams) where TWorker : class, IHostedService
        {
            ConfigureLogger.StandardSetup(logLevel: runCompoundWorkerParams.LogLevel);
            Host.CreateDefaultBuilder(runCompoundWorkerParams.Args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    runCompoundWorkerParams.IocDelegate(hostContext, null, services);
                    services.AddSingleton(provider => runCompoundWorkerParams.WorkerProcess);
                    services.AddSingleton(provider => runCompoundWorkerParams.ListenerInfo);
                    services.AddSingleton(provider => runCompoundWorkerParams.IocDelegate);
                    services.AddSingleton(provider => runCompoundWorkerParams.WebAppBuilderDelegate);
                    services.AddHostedService<TWorker>();
                })
                .Build().Run();
        }

        public static void RunBackgroundTaskWorker(RunBackgroundTaskWorkerParams runBackgroundTaskWorkerParams) 
        {
            RunBackgroundTaskWorker<BackgroundTaskWorker>(runBackgroundTaskWorkerParams);
        }

        public static void RunBackgroundTaskWorker<TWorker>(RunBackgroundTaskWorkerParams runBackgroundTaskWorkerParams) where TWorker : class, IHostedService
        {
            ConfigureLogger.StandardSetup(logLevel: runBackgroundTaskWorkerParams.LogLevel);
            Host.CreateDefaultBuilder(runBackgroundTaskWorkerParams.Args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    runBackgroundTaskWorkerParams.IocDelegate(hostContext, null, services);
                    services.AddSingleton(provider => runBackgroundTaskWorkerParams.WorkerProcess);
                    services.AddSingleton(provider => runBackgroundTaskWorkerParams.IocDelegate);
                    services.AddHostedService<TWorker>();
                })
                .Build().Run();
        }

        public static void RunApiWorker(RunApiWorkerParams runApiWorkerParams)
        {
            RunApiWorker<WebWorker>(runApiWorkerParams);
        }

        public static void RunApiWorker<TWorker>(RunApiWorkerParams runApiWorkerParams)
            where TWorker : class, IHostedService
        {
            ConfigureLogger.StandardSetup(logLevel: runApiWorkerParams.LogLevel);
            Host.CreateDefaultBuilder(runApiWorkerParams.Args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    runApiWorkerParams.IocDelegate(hostContext, null, services);
                    services.AddSingleton(provider => runApiWorkerParams.ListenerInfo);
                    services.AddSingleton(provider => runApiWorkerParams.IocDelegate);
                    services.AddSingleton(provider => runApiWorkerParams.WebAppBuilderDelegate);
                    services.AddHostedService<TWorker>();
                })
                .Build().Run();
        }
        
    }
}