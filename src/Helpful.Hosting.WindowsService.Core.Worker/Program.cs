using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WindowsService.Core.Worker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HostBuilderFactory.CreateHostBuilder<Worker>(args).Build().Run();
        }
    }
}
