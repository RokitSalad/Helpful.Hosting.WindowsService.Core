using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Helpful.Hosting.Dto;
using Helpful.Logging.Standard;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Serilog;
using Topshelf;

namespace Helpful.Hosting.WindowsService.Core
{
    public class BasicWebService : BasicWebService<DefaultStartup>
    {
        public BasicWebService(params ListenerInfo[] listenerInfo) : base(listenerInfo)
        {

        }
    }

    public class BasicWebService<T> : ServiceControl where T : class
    {
        private readonly ListenerInfo[] _listenerInfo;
        private static IHost WebServiceHolder { get; set; }

        public BasicWebService(params ListenerInfo[] listenerInfo)
        {
            _listenerInfo = listenerInfo;
        }

        ~BasicWebService()
        {
            WebServiceHolder?.Dispose();
        }

        public virtual bool Start(HostControl hostControl)
        {
            WebServiceHolder = CreateHostBuilder(_listenerInfo, null).Build();
            WebServiceHolder.StartAsync();
            return true;
        }

        public virtual bool Stop(HostControl hostControl)
        {
            WebServiceHolder.StopAsync();
            return true;
        }

        private static IHostBuilder CreateHostBuilder(ListenerInfo[] listenerInfo, string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseKestrel(opt =>
                    {
                        var logger = typeof(BasicWebService).GetLogger();
                        foreach (var info in listenerInfo)
                        {
                            if (string.IsNullOrWhiteSpace(info.IpAddress))
                            {
                                logger.LogDebugWithContext("Ip address not present.");
                                if (info.UseTls)
                                {
                                    logger.LogDebugWithContext($"Using ssl: {info.Port}");
                                    opt.ListenAnyIP(info.Port, ConfigureKestrelForSsl(logger, info));
                                }
                                else
                                {
                                    logger.LogDebugWithContext($"Not using ssl: {info.Port}");
                                    opt.ListenAnyIP(info.Port);
                                }
                            }
                            else
                            {
                                logger.LogDebugWithContext($"Ip address provided: {info.IpAddress}");
                                opt.Listen(IPAddress.Parse(info.IpAddress), info.Port);
                            }
                        }
                    });
                    webBuilder.UseStartup<T>();
                });

        private static Action<ListenOptions> ConfigureKestrelForSsl(ILogger logger, ListenerInfo info)
        {
            return options =>
            {
                logger.LogDebugWithContext($"Configuring ssl: {info.SslCertStoreName}, {info.SslCertSubject}, {info.AllowInvalidCert}.");
                X509Store store = new X509Store(info.SslCertStoreName,
                    StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);

                foreach (var cert in store.Certificates)
                {
                    if (string.Equals(cert.Subject, info.SslCertSubject,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        logger.LogDebugWithContext("Found certificate.");
                        options.UseHttps(cert);
                        break;
                    }
                }
            };
        }
    }
}