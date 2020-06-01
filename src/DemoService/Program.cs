using Helpful.Hosting.WindowsService.Core;

namespace DemoService
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new HostRunner<CustomStartup>("DemoService", 5001);
            var exit = runner.RunWebService();
        }
    }
}
