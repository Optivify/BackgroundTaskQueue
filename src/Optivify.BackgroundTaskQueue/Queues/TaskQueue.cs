using Optivify.BackgroundTaskQueue.WorkItems;
using System.Collections.Concurrent;

namespace Optivify.BackgroundTaskQueue.Queues;

public class TaskQueue : ITaskQueue
{
    protected readonly ConcurrentQueue<WorkItem> workItems = new ConcurrentQueue<WorkItem>();

    protected readonly ConcurrentDictionary<string, WorkItem> workItemsDictionary = new ConcurrentDictionary<string, WorkItem>();

    public int Count => this.workItems.Count;

    // Use semaphore to control the access of external threads.
    // Set the initial count to 0 to make sure the queue service cannot access the queue
    // in order to dequeue until there's at least one item in the queue.
    protected readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(0);

    protected bool disposed;

    #region Enqueue

    public virtual void Enqueue(Func<CancellationToken, Task> task)
    {
        this.Enqueue(new WorkItem(task));
    }

    public virtual void Enqueue(string id, Func<CancellationToken, Task> task)
    {
        this.Enqueue(new WorkItem(id, task));
    }

    public virtual void Enqueue(WorkItem workItem)
    {
        var shouldEnqueue = workItem.Id == null ? true : this.workItemsDictionary.TryAdd(workItem.Id, workItem);

        if (shouldEnqueue)
        {
            this.workItems.Enqueue(workItem);

            workItem.OnEnqueued();

            // Release once to allow dequeueing this work item.
            this.semaphoreSlim.Release();
        }
    }

    #endregion

    #region Dequeue

    public virtual async Task<(bool, WorkItem?)> DequeueAsync(CancellationToken cancellationToken)
    {
        await this.semaphoreSlim.WaitAsync(cancellationToken);

        var success = this.workItems.TryDequeue(out var workItem);

        if (success && workItem != null && workItem.Id != null)
        {
            this.workItemsDictionary.TryRemove(workItem.Id, out var _);
        }

        return (success, workItem);
    }

    #endregion

    #region Dispose

    public void Dispose()
    {
        if (!this.disposed)
        {
            this.semaphoreSlim.Dispose();
            this.disposed = true;
        }
    }

    #endregion
}