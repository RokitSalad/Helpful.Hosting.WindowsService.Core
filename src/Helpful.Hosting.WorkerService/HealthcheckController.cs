using System.Diagnostics;
using System.Reflection;
using Helpful.Hosting.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Helpful.Hosting.WorkerService
{
    [ApiController]
    [Route("[controller]")]
    public class HealthcheckController : ControllerBase
    {
        [HttpGet]
        public HealthcheckResponse Get()
        {
            var assembly = Assembly.GetEntryAssembly();
            if (assembly != null)
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                string version = fvi.ProductVersion;

                return new HealthcheckResponse
                {
                    ServiceAssemblyName = assembly.GetName().Name,
                    AssemblyVersion = version
                };
            }
            return new HealthcheckResponse();
        }

    }
}
