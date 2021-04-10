using System;

namespace DemoWorker.Services
{
    public class DayOfTheWeekService : IDayOfTheWeekService
    {
        public string GetDayOfTheWeek()
        {
            return DateTime.Now.DayOfWeek.ToString();
        }
    }
}