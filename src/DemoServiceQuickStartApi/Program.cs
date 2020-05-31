using System;
using Helpful.Hosting.WindowsService.Core;
using Topshelf;

namespace DemoServiceQuickStartApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new HostRunner("DemoService_QuickStartApi", 5002);
            var exit = runner.Run();
        }
    }
}
