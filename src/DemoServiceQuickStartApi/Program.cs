using Helpful.Hosting.Dto;
using Helpful.Hosting.WindowsService.Core;

namespace DemoServiceQuickStartApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new HostRunner("DemoService_QuickStartApi", new ListenerInfo
            {
                Port = 8053
            });
            var exit = runner.RunWebService();
        }
    }
}
