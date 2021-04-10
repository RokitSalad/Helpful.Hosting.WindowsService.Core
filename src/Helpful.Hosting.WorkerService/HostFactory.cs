using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService.DefaultWorkers;
using Helpful.Logging.Standard;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;

namespace Helpful.Hosting.WorkerService
{
    public static class HostFactory
    {
        public static void RunCustomWorker<TWorker>(string[] args, Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, LogEventLevel logLevel, params ListenerInfo[] listenerInfo) where TWorker : class, IHostedService
        {
            ConfigureLogger.StandardSetup(logLevel: logLevel);
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    iocDelegate(hostContext, null, services);
                    services.AddSingleton(provider => listenerInfo);
                    services.AddSingleton(provider => iocDelegate);
                    services.AddHostedService<TWorker>();
                })
                .Build().Run();
        }

        public static void RunCompoundWorker(string[] args, Func<CancellationToken, Task> workerProcess, Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, LogEventLevel logLevel, params ListenerInfo[] listenerInfo)
        {
            RunCompoundWorker<CompoundWorker>(args, workerProcess, iocDelegate, logLevel, listenerInfo);
        }

        public static void RunCompoundWorker<TWorker>(string[] args, Func<CancellationToken, Task> workerProcess, Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, LogEventLevel logLevel, params ListenerInfo[] listenerInfo) where TWorker : class, IHostedService
        {
            ConfigureLogger.StandardSetup(logLevel: logLevel);
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    iocDelegate(hostContext, null, services);
                    services.AddSingleton(provider => workerProcess);
                    services.AddSingleton(provider => listenerInfo);
                    services.AddSingleton(provider => iocDelegate);
                    services.AddHostedService<TWorker>();
                })
                .Build().Run();
        }

        public static void RunBackgroundTaskWorker(string[] args, Func<CancellationToken, Task> workerProcess, Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, LogEventLevel logLevel) 
        {
            RunBackgroundTaskWorker<BackgroundTaskWorker>(args, workerProcess, iocDelegate, logLevel);
        }

        public static void RunBackgroundTaskWorker<TWorker>(string[] args, Func<CancellationToken, Task> workerProcess, Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, LogEventLevel logLevel) where TWorker : class, IHostedService
        {
            ConfigureLogger.StandardSetup(logLevel: logLevel);
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    iocDelegate(hostContext, null, services);
                    services.AddSingleton(provider => workerProcess);
                    services.AddSingleton(provider => iocDelegate);
                    services.AddHostedService<TWorker>();
                })
                .Build().Run();
        }

        public static void RunApiWorker(string[] args, Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, LogEventLevel logLevel, params ListenerInfo[] listenerInfo)
        {
            RunApiWorker<WebWorker>(args, iocDelegate, logLevel, listenerInfo);
        }

        public static void RunApiWorker<TWorker>(string[] args, Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, LogEventLevel logLevel, params ListenerInfo[] listenerInfo)
            where TWorker : class, IHostedService
        {
            ConfigureLogger.StandardSetup(logLevel: logLevel);
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    iocDelegate(hostContext, null, services);
                    services.AddSingleton(provider => listenerInfo);
                    services.AddSingleton(provider => iocDelegate);
                    services.AddHostedService<TWorker>();
                })
                .Build().Run();
        }

        public static IHost BuildKestrelWebHost<TWebStartup>(Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, params ListenerInfo[] listenerInfo) where TWebStartup : class =>
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