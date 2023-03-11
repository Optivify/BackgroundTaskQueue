using BenchmarkDotNet.Running;
using Optivify.BackgroundTaskQueue.Benchmarks;

var run = BenchmarkRunner.Run<TaskQueueBenchmarks>();

Console.WriteLine($"Benchmark done in: {run.TotalTime.TotalMilliseconds}ms.");
Console.ReadLine();