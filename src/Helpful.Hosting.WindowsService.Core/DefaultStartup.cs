using Helpful.Logging.Standard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Helpful.Hosting.WindowsService.Core
{
    public class DefaultStartup
    {
        private ILogger Logger => this.GetLogger();
        public IConfiguration Configuration { get; }

        public DefaultStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Logger.LogInformationWithContext("Configuring service.");
            BasicConfiguration.ConfigureBasicServices(services);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Logger.LogInformationWithContext("Configuring app.");
            BasicConfiguration.Configure(app, env);
        }
    }
}