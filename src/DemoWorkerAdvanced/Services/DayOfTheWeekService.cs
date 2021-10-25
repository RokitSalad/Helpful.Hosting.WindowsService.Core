using System;

namespace DemoWorkerAdvanced.Services
{
    public class DayOfTheWeekService : IDayOfTheWeekService
    {
        public string GetDayOfTheWeek()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }
    }
}