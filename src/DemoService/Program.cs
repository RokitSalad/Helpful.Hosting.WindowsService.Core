using Helpful.Hosting.WindowsService.Core;

namespace DemoService
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new HostRunner<CustomStartup>("DemoService", "http://*:5001", "https://*:5011");
            var exit = runner.RunWebService();
        }
    }
}
