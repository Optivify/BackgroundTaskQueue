using Optivify.BackgroundTaskQueue.WorkItems;

namespace Optivify.BackgroundTaskQueue.Queues;

public interface ITaskQueue : IDisposable
{
    void Enqueue(Func<CancellationToken, Task> task);

    void Enqueue(WorkItem workItem);

    Task<(bool, WorkItem?)> DequeueAsync(CancellationToken cancellationToken);

    int Count { get; }
}
