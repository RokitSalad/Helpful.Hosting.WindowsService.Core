using Helpful.Hosting.WindowsService.Core;

namespace DemoService
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new HostRunner<CustomStartup>("DemoService", "http://*:8050", "http://*:8051");
            var exit = runner.RunWebService();
        }
    }
}
