using System;
using System.Threading;
using System.Threading.Tasks;
using DemoWorkerAdvanced.Services;
using Helpful.Hosting.Dto;
using Helpful.Hosting.WorkerService.DefaultWorkers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DemoWorkerAdvanced
{
    public class CustomWorker : CustomWorkerBase
    {
        private readonly IDayOfTheWeekService _dayOfTheWeekService;

        public CustomWorker(IDayOfTheWeekService dayOfTheWeekService, 
            Action<HostBuilderContext, WebHostBuilderContext, IServiceCollection> iocDelegate, 
            params ListenerInfo[] listenerInfo)
        : base(iocDelegate, listenerInfo)
        {
            _dayOfTheWeekService = dayOfTheWeekService;
        }

        protected override async Task CustomWorkerLogic(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine($"The day is {_dayOfTheWeekService.GetDayOfTheWeek()}");
                Console.WriteLine($"The time is {DateTime.Now.ToLongTimeString()}");
                Console.WriteLine(" --- ");
                await Task.Delay(4000);
            }
        }
    }
}
