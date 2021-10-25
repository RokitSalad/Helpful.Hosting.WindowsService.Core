using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Helpful.Hosting.WorkerService.DefaultWorkers
{
    /// <summary>
    /// The <c>BackgroundTaskWorker</c> is a simplified version of <c>CompoundWorker</c>. It runs a simple <c>Task</c> until cancelled.
    /// The <c>BackgroundTaskWorker</c> does not expose any HTTP endpoints.
    /// The workerProcess is passed in from the consuming assembly and is executed asynchronously over and over until the <c>CancellationToken</c> is flagged as cancelled.
    /// A consumer should not need to reference this class manually as it is used by default.
    /// </summary>
    public class BackgroundTaskWorker : BackgroundService
    {
        private readonly Func<CancellationToken, Task> _workerProcess;

        /// <summary>
        /// Constructor. All constructor params are passed into <code>HostFactory.RunBackgroundTaskWorker()</code>.
        /// </summary>
        /// <param name="workerProcess">The async process to run until the task is cancelled</param>
        public BackgroundTaskWorker(Func<CancellationToken, Task> workerProcess)
        {
            _workerProcess = workerProcess;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _workerProcess(stoppingToken);
            }
        }
    }
}
