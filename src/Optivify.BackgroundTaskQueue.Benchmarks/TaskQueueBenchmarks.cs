using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.Logging.Abstractions;
using Optivify.BackgroundTaskQueue.Queues;
using Optivify.BackgroundTaskQueue.Services;

namespace Optivify.BackgroundTaskQueue.Benchmarks;

public class TaskQueueBenchmarks
{
    private readonly NullLoggerFactory loggerFactory;
    private readonly CancellationToken cancellationToken;

    private readonly TaskQueue queue;
    private readonly BackgroundTaskQueueService backgroundTaskQueueService;

    public TaskQueueBenchmarks()
    {
        this.loggerFactory = new NullLoggerFactory();
        this.cancellationToken = new CancellationToken();
        this.queue = new TaskQueue();
        this.backgroundTaskQueueService = new BackgroundTaskQueueService(this.loggerFactory, this.queue);
    }

    [Benchmark]
    public async Task TaskQueueBenchmark()
    {
        this.queue.Enqueue(ct => { return Task.CompletedTask; });
        await this.backgroundTaskQueueService.StartAsync(this.cancellationToken);
        await this.backgroundTaskQueueService.StopAsync(this.cancellationToken);
    }
}
