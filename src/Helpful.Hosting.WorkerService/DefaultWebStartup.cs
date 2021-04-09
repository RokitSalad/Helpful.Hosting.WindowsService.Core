using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Helpful.Hosting.WorkerService
{
    public class DefaultWebStartup
    {
        private static string ApplicationTitle => Assembly.GetEntryAssembly()?.GetName().ToString();

        private static string ApplicationMajorVersion
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                if (assembly == null) return "0";
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                return fvi.ProductVersion;
            }
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetEntryAssembly();
            var executingAssembly = Assembly.GetExecutingAssembly();
            
            services.AddControllers()
                .AddApplicationPart(assembly)
                .AddApplicationPart(executingAssembly);
            
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = ApplicationTitle, Version = $"v{ApplicationMajorVersion}" }); });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            }); 
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApplicationTitle} V{ApplicationMajorVersion}"); });
        }
    }
}
