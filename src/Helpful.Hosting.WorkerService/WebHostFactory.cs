using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Helpful.Hosting.Dto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService
{
    public static class WebHostFactory
    {
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
