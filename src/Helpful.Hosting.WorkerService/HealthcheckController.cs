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
        private HealthcheckResponse _healthcheckResponse;
        private ILogger Logger => this.GetLogger();

        private HealthcheckResponse HealthcheckResponse => _healthcheckResponse ??= GetHealthcheckResponse();

        [HttpGet]
        public HealthcheckResponse Get()
        {
            Logger.LogDebugWithContext("Basic healthcheck hit");
            return HealthcheckResponse;
        }

        private HealthcheckResponse GetHealthcheckResponse()
        {
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
                Logger.LogDebugWithContext("Building version specific healthcheck response: {0} {1}", response.ServiceAssemblyName, response.AssemblyVersion);
                return response;
            }
            Logger.LogDebugWithContext("Building empty healthcheck response");
            return new HealthcheckResponse();
        }


    }
}
