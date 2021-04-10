using System;
using Microsoft.AspNetCore.Mvc;

namespace DemoWorker
{
    [ApiController]
    [Route("[controller]")]
    public class DayOfTheWeekController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }
    }
}
