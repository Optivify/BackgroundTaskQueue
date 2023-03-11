using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Optivify.BackgroundTaskQueue.Queues;
using Optivify.BackgroundTaskQueue.Services;
using Optivify.BackgroundTaskQueue.WorkItems;

namespace Optivify.BackgroundTaskQueue.Tests;

[TestClass]
public class TaskQueueTests
{
    private int totalItemsEnqueued;

    private int totalItemsCompleted;

    private int totalItemsFailed;

    private void WorkItem_Failed(object sender, Events.WorkItemFailedEventArgs eventArgs)
    {
        this.totalItemsFailed++;
    }

    private void WorkItem_Completed(object sender, Events.WorkItemCompletedEventArgs eventArgs)
    {
        this.totalItemsCompleted++;
    }

    private void WorkItem_Enqueued(object sender, Events.WorkItemEnqueuedEventArgs eventArgs)
    {
        this.totalItemsEnqueued++;
    }

    [TestMethod]
    [DataRow(
        100         // numberOfWorkItems
    )]
    public async Task VerifyBackgroundTaskQueue(int numberOfWorkItems)
    {
        var number = 0;
        var services = new ServiceCollection();
        var loggerFactory = new NullLoggerFactory();
        var queue = new TaskQueue();
        var backgroundTaskQueueService = new BackgroundTaskQueueService(loggerFactory, queue);
        var random = new Random();
        this.totalItemsEnqueued = 0;
        this.totalItemsCompleted = 0;
        this.totalItemsFailed = 0;

        var itemsCompleted = 0;
        var itemsFailed = 0;

        // Enqueue
        for (var i = 0; i < numberOfWorkItems; i++)
        {
            Func<CancellationToken, Task> func;

            if (i % 2 == 0)
            {
                func = ct => { number++; return Task.CompletedTask; };
                itemsCompleted++;
            }
            else
            {
                func = ct => { throw new Exception(); };
                itemsFailed++;
            }

            var workItem = new WorkItem(id: i.ToString(), func);
            workItem.Enqueued += this.WorkItem_Enqueued;
            workItem.Completed += this.WorkItem_Completed;
            workItem.Failed += this.WorkItem_Failed;

            // Enqueue work item
            queue.Enqueue(workItem);

            // Try to enqueue work items with duplicated IDs
            queue.Enqueue(workItem);
        }

        // Dequeue
        var cancellationToken = new CancellationToken();

        for (var i = 0; i < numberOfWorkItems; i++)
        {
            await backgroundTaskQueueService.StartAsync(cancellationToken);
            await backgroundTaskQueueService.StopAsync(cancellationToken);
        }

        Assert.AreEqual(0, queue.Count);

        Assert.AreEqual(numberOfWorkItems, this.totalItemsEnqueued);
        Assert.AreEqual(itemsCompleted, this.totalItemsCompleted);
        Assert.AreEqual(itemsFailed, this.totalItemsFailed);
    }
}