using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService;
using Helpful.Hosting.WorkerService.HostFactoryParams;
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
                        Port = 8055
                    }
                }
            });
        }
    }
}
