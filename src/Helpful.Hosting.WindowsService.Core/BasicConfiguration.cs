using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Helpful.Hosting.WindowsService.Core
{
    public static class BasicConfiguration
    {
        private static string ApplicationTitle => Assembly.GetEntryAssembly()?.GetName().ToString();

        public static void ConfigureBasicServices(IServiceCollection services)
        {
            var assembly = Assembly.GetEntryAssembly();
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(Assembly.GetExecutingAssembly()));
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(assembly));
            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = ApplicationTitle, Version = "v1" }); });
        }
        
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApplicationTitle} V1"); });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
