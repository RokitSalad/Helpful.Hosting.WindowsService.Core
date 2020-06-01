using System;
using Helpful.Hosting.WindowsService.Core;

namespace DemoServiceBackgroundProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new HostRunner("DemoService_BackgroundTask", 5003);
            var exit = runner.RunCompoundService(obj => Console.WriteLine($"The time is: {DateTime.Now:T}"), null, 5000);
        }
    }
}
