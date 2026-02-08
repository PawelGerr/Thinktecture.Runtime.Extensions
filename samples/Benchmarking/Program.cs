using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using Thinktecture.Benchmarks;

var config = ManualConfig.CreateMinimumViable()
                         .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()))
                         .AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0));

// BenchmarkRunner.Run<LoadingSmartEnums>(config);
// BenchmarkRunner.Run<LoadingValueObjects>(config);
// BenchmarkRunner.Run<SingleItemSetBenchmarks>(config);
// BenchmarkRunner.Run<SingleItemDictionaryBenchmarks>(config);
// BenchmarkRunner.Run<ItemSearch>(config);
// BenchmarkRunner.Run<TypedIdBenchmarks>(config);
// BenchmarkRunner.Run<SmartEnumJsonDeserializationBenchmarks>(config);
BenchmarkRunner.Run<ValueObjectJsonDeserializationBenchmarks>(config);
