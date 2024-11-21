using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Thinktecture.Benchmarks;

var config = ManualConfig.CreateMinimumViable()
                         .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()))
                         .AddJob(Job.Default.WithRuntime(CoreRuntime.Core70));

// BenchmarkRunner.Run<LoadingSmartEnums>(config);
// BenchmarkRunner.Run<LoadingValueObjects>(config);
// BenchmarkRunner.Run<SingleItemCollectionBenchmarks>(config);
// BenchmarkRunner.Run<SingleItemSetBenchmarks>(config);
BenchmarkRunner.Run<ItemSearch>(config);
