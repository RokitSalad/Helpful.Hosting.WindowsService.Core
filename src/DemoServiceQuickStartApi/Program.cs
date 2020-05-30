using System;
using Helpful.Hosting.WindowsService.Core;
using Topshelf;

namespace DemoServiceQuickStartApi
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service(() => new BasicWebService("http://localhost:5002"));
                x.EnableServiceRecovery(r => r.RestartService(TimeSpan.FromSeconds(10)));
                x.SetServiceName("DemoService_QuickStartApi");
            });
        }
    }
}
