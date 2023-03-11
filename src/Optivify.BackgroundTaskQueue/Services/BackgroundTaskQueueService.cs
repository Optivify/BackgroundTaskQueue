using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Optivify.BackgroundTaskQueue.Queues;

namespace Optivify.BackgroundTaskQueue.Services;

public class BackgroundTaskQueueService : BackgroundService
{
    private readonly ILogger<BackgroundTaskQueueService> logger;

    public ITaskQueue BackgroundTaskQueue { get; }

    public BackgroundTaskQueueService(ILoggerFactory loggerFactory, ITaskQueue taskQueue)
    {
        this.logger = loggerFactory.CreateLogger<BackgroundTaskQueueService>();
        this.BackgroundTaskQueue = taskQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var (result, workItem) = await this.BackgroundTaskQueue.DequeueAsync(cancellationToken);

            if (result && workItem != null)
            {
                try
                {
                    await workItem.ExecuteAsync(cancellationToken);
                    workItem.OnCompleted();
                }
                catch (Exception ex)
                {
                    workItem.OnFailed(ex);
                    this.logger.LogError(ex, ex.Message);
                }
            }
        }
    }

    public override void Dispose()
    {
        this.BackgroundTaskQueue.Dispose();
        base.Dispose();
    }
}
