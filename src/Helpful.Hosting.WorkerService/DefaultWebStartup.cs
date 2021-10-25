using System;
using System.Diagnostics;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Helpful.Hosting.WorkerService
{
    public class DefaultWebStartup
    {
        private static string _productVersion;
        private static string _applicationTitle;

        private static string ApplicationTitle => _applicationTitle ??= Assembly.GetEntryAssembly()?.GetName().Name;

        private static string ApplicationMajorVersion => _productVersion ??= GetAssemblyVersion();

        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetEntryAssembly();
            var executingAssembly = Assembly.GetExecutingAssembly();
            
            services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc($"v{ApplicationMajorVersion}", new OpenApiInfo { Title = ApplicationTitle, Version = $"v{ApplicationMajorVersion}" });
                })
                .AddControllers()
                .AddApplicationPart(assembly)
                .AddApplicationPart(executingAssembly);
        }

        public void Configure(IApplicationBuilder app, Action<IApplicationBuilder> appBuilderDelegate)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint($"/swagger/v{ApplicationMajorVersion}/swagger.json", $"{ApplicationTitle} V{ApplicationMajorVersion}"); });
            appBuilderDelegate(app);
        }

        private static string GetAssemblyVersion()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly == null) return "0";
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fvi.ProductVersion;
        }
    }
}
