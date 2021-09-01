using DemoWorkerDocker.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoWorkerDocker
{
    [ApiController]
    [Route("[controller]")]
    public class DayOfTheWeekController : ControllerBase
    {
        private readonly IDayOfTheWeekService _dayOfTheWeekService;

        public DayOfTheWeekController(IDayOfTheWeekService dayOfTheWeekService)
        {
            _dayOfTheWeekService = dayOfTheWeekService;
        }

        [HttpGet]
        public string Get()
        {
            return _dayOfTheWeekService.GetDayOfTheWeek();
        }
    }
}
