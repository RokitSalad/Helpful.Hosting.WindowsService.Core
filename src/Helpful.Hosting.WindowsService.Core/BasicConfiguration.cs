using System.Reflection;
using Helpful.Logging.Standard;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace Helpful.Hosting.WindowsService.Core
{
    public static class BasicConfiguration
    {
        private static string ApplicationTitle => Assembly.GetEntryAssembly()?.GetName().ToString();

        private static ILogger Logger => typeof(BasicConfiguration).GetLogger();

        public static void ConfigureBasicServices(IServiceCollection services)
        {
            Logger.LogInformationWithContext("Starting to set up basic service configuration");

            var assembly = Assembly.GetEntryAssembly();
            var executingAssembly = Assembly.GetExecutingAssembly();

            Logger.LogInformationWithContext("Entry assembly identified as {EntryAssembly}.", assembly?.FullName);
            Logger.LogInformationWithContext("Executing assembly identified as {ExecutingAssembly}.", executingAssembly.FullName);

            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(executingAssembly));
            services.AddControllers().PartManager.ApplicationParts.Add(new AssemblyPart(assembly));

            Logger.LogInformationWithContext("Controllers added from executing assembly and entry assembly.");

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = ApplicationTitle, Version = "v1" }); });

            Logger.LogInformationWithContext("Swagger doc generated.");
        }
        
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Logger.LogInformationWithContext("Starting to set up basic configuration.");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApplicationTitle} V1"); });
            Logger.LogInformationWithContext("Swagger UI listening.");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
