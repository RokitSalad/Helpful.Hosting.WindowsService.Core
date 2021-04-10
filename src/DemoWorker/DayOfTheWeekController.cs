using DemoWorker.Services;
using Microsoft.AspNetCore.Mvc;

namespace DemoWorker
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
