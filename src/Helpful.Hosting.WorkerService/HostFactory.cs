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

namespace Helpful.Hosting.WorkerService
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

        public static IHost BuildKestrelWebHost<TWebStartup>(Action<IApplicationBuilder> webAppBuilderDelegate,
            Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate,
            params ListenerInfo[] listenerInfo) where TWebStartup : class =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(opt =>
                    {
                        foreach (var info in listenerInfo)
                        {
                            if (string.IsNullOrWhiteSpace(info.IpAddress))
                            {
                                if (info.UseTls)
                                {
                                    opt.ListenAnyIP(info.Port, ConfigureKestrelForSsl(info));
                                }
                                else
                                {
                                    opt.ListenAnyIP(info.Port);
                                }
                            }
                            else
                            {
                                opt.Listen(IPAddress.Parse(info.IpAddress), info.Port);
                            }
                        }
                    });
                    webBuilder.ConfigureServices((hostContext, services) =>
                    {
                        iocDelegate(null, hostContext, services);
                        services.AddSingleton(provider => webAppBuilderDelegate);
                    });
                    webBuilder.UseStartup<TWebStartup>();
                }).Build();

        private static Action<ListenOptions> ConfigureKestrelForSsl(ListenerInfo info)
        {
            return options =>
            {
                X509Store store = new X509Store(info.SslCertStoreName,
                    StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);

                foreach (var cert in store.Certificates)
                {
                    if (string.Equals(cert.Subject, info.SslCertSubject,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        options.UseHttps(cert);
                        break;
                    }
                }
            };
        }
        
    }
}