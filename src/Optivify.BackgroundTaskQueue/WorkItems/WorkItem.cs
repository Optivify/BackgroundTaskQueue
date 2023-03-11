using Optivify.BackgroundTaskQueue.Events;

namespace Optivify.BackgroundTaskQueue.WorkItems;

public class WorkItem
{
    private readonly Func<CancellationToken, Task> task;

    public string? Id { get; set; }

    public event WorkItemEnqueued? Enqueued;

    public event WorkItemCompleted? Completed;

    public event WorkItemFailed? Failed;

    public WorkItem(Func<CancellationToken, Task> task)
    {
        this.task = task;
    }

    public WorkItem(string id, Func<CancellationToken, Task> task) : this(task)
    {
        this.Id = id;
    }

    public Task ExecuteAsync(CancellationToken cancellationToken) => this.task(cancellationToken);

    internal void OnEnqueued() => this.Enqueued?.Invoke(this, new WorkItemEnqueuedEventArgs(this));

    internal void OnCompleted() => this.Completed?.Invoke(this, new WorkItemCompletedEventArgs(this));

    internal void OnFailed(Exception exception) => this.Failed?.Invoke(this, new WorkItemFailedEventArgs(this, exception));
}