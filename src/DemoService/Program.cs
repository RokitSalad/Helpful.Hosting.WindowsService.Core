using System;
using Helpful.Hosting.WindowsService.Core;
using Topshelf;

namespace DemoService
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new Helpful.Hosting.WindowsService.Core.HostRunner("DemoService", 5001);
            var exit = runner.Run();
        }
    }
}
