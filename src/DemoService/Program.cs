using Helpful.Hosting.WindowsService.Core;

namespace DemoService
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new HostRunner<CustomStartup>("DemoService", "http://0.0.0.0:8050", "https://*:8051");
            var exit = runner.RunWebService();
        }
    }
}
