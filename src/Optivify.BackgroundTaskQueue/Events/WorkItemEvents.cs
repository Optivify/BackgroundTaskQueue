using Optivify.BackgroundTaskQueue.WorkItems;

namespace Optivify.BackgroundTaskQueue.Events;

#region Enqueued

public class WorkItemEnqueuedEventArgs
{
    public WorkItemEnqueuedEventArgs(WorkItem workItem)
    {
        this.WorkItem = workItem;
    }

    public WorkItem WorkItem { get; }
}

public delegate void WorkItemEnqueued(object sender, WorkItemEnqueuedEventArgs eventArgs);

#endregion


#region Completed

public class WorkItemCompletedEventArgs
{
    public WorkItemCompletedEventArgs(WorkItem workItem)
    {
        this.WorkItem = workItem;
    }

    public WorkItem WorkItem { get; }
}

public delegate void WorkItemCompleted(object sender, WorkItemCompletedEventArgs eventArgs);

#endregion

#region Failed

public class WorkItemFailedEventArgs
{
    public WorkItemFailedEventArgs(WorkItem workItem, Exception exception)
    {
        this.WorkItem = workItem;
        this.Exception = exception;
    }

    public WorkItem WorkItem { get; }

    public Exception Exception { get; }
}

public delegate void WorkItemFailed(object sender, WorkItemFailedEventArgs eventArgs);

#endregion
