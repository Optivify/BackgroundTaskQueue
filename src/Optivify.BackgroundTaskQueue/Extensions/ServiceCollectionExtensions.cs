using Microsoft.Extensions.DependencyInjection;
using Optivify.BackgroundTaskQueue.Queues;
using Optivify.BackgroundTaskQueue.Services;

namespace Optivify.BackgroundTaskQueue.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackgroundTaskQueue(this IServiceCollection services)
    {
        services
            .AddSingleton<ITaskQueue, TaskQueue>()
            .AddHostedService<BackgroundTaskQueueService>();

        return services;
    }
}