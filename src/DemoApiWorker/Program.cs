using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService;
using Serilog.Events;

namespace DemoApiWorker
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.RunApiWorker(args, (hostContext, webHostContext, collection) => { },
                app => { }, LogEventLevel.Debug, new ListenerInfo
                {
                    Port = 8055
                }
            );
        }
    }
}
