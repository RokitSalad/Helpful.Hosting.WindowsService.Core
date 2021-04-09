using System.Diagnostics;
using System.Reflection;
using Helpful.Hosting.Dto;
using Helpful.Logging.Standard;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Helpful.Hosting.WorkerService
{
    [ApiController]
    [Route("[controller]")]
    public class HealthcheckController : ControllerBase
    {
        private ILogger Logger => this.GetLogger();

        [HttpGet]
        public HealthcheckResponse Get()
        {
            Logger.LogDebugWithContext("Basic healthcheck hit");
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.ProductVersion;

                var response = new HealthcheckResponse
                {
                    ServiceAssemblyName = assembly.GetName().Name,
                    AssemblyVersion = version
                };
                Logger.LogDebugWithContext("Returning version specific healthcheck response: {0} {1}", response.ServiceAssemblyName, response.AssemblyVersion);
                return response;
            }
            Logger.LogDebugWithContext("Returning empty healthcheck response");
            return new HealthcheckResponse();
        }

    }
}
