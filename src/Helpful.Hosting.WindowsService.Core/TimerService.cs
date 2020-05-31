using System;
using System.Timers;
using Topshelf;

namespace Helpful.Hosting.WindowsService.Core
{
    public class TimerService : TimerService<DefaultStartup>
    {
        public TimerService(Action<object> singleRun, object state, int scheduleMilliseconds) : base(singleRun, state, scheduleMilliseconds)
        {
        }
    }

    public class TimerService<TStartup> : BasicWebService<TStartup> where TStartup : class
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

        public override bool Start(HostControl hostControl)
        {
            _timer.Start();
            return base.Start(hostControl);
        }

        public override bool Stop(HostControl hostControl)
        {
            _timer.Stop();
            return base.Stop(hostControl);
        }
    }
}
