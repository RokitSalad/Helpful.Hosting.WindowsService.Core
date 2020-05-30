using System;
using System.Timers;

namespace Helpful.Hosting.WindowsService.Core
{
    internal class TimerService
    {
        private readonly Timer _timer;

        public TimerService(Action<object> singleRun, object state, int scheduleMilliseconds)
        {
            _timer = new Timer(scheduleMilliseconds)
            {
                AutoReset = true
            };
            _timer.Elapsed += (sender, args) => singleRun(state);
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }
}
