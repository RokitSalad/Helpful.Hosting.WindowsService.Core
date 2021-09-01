using System;

namespace DemoWorkerDocker.Services
{
    public class DayOfTheWeekService : IDayOfTheWeekService
    {
        public string GetDayOfTheWeek()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }
    }
}