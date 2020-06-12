using System.Security.Cryptography.X509Certificates;
using Helpful.Hosting.WindowsService.Core;
using Serilog.Events;

namespace DemoService
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new HostRunner<CustomStartup>("DemoService", new ListenerInfo
                {
                    Port = 8050
                },
                new ListenerInfo
                {
                    AllowInvalidCert = true,
                    Port = 8051,
                    SslCertStoreName = StoreName.My,
                    SslCertSubject = "CN=localhost", //"ec2-3-104-124-78.ap-southeast-2.compute.amazonaws.com",
                    UseSsl = true
                });
            var exit = runner.RunWebService(LogEventLevel.Debug);
        }
    }
}
