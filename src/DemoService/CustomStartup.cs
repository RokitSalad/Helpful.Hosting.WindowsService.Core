using Helpful.Hosting.WindowsService.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DemoService
{
    class CustomStartup
    {
        public IConfiguration Configuration { get; }

        public CustomStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            BasicConfiguration.ConfigureBasicServices(services);
            // Add your additional service setup code here
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            BasicConfiguration.Configure(app, env);
            // Add your additional configuration code here
        }
    }
}
