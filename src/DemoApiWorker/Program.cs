using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService.HostFactoryParams;
using Helpful.Hosting.WorkerService.Windows;
using Serilog.Events;

namespace DemoApiWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.RunApiWorker(new RunApiWorkerParams
            {
                Args = args,
                LogLevel = LogEventLevel.Debug,
                IocDelegate = (hostContext, webHostContext, collection) => { },
                WebAppBuilderDelegate = app => { },
                ListenerInfo = new[]
                {
                    new ListenerInfo
                    {
                        Port = 8150
                    }
                }
            });
        }
    }
}
