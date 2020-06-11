using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
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
                        foreach (var info in listenerInfo)
                        {
                            if (string.IsNullOrWhiteSpace(info.IpAddress))
                            {
                                if (info.UseSsl)
                                {
                                    opt.ListenAnyIP(info.Port, options =>
                                    {
                                        options.UseHttps(info.SslCertStoreName, info.SslCertSubject, info.AllowInvalidCert);
                                    });
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
                    webBuilder.UseStartup<T>();
                    //webBuilder.UseUrls(listenerInfo);
                });
    }
}