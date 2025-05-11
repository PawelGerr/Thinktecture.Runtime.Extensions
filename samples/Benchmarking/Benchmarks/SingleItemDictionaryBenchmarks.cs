using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Thinktecture.Benchmarks;

/*

29.04.2025

.NET 9.0.4

| Method                      | Mean     | Error     | StdDev    | Median   | Gen0   | Allocated |
|---------------------------- |---------:|----------:|----------:|---------:|-------:|----------:|
| ForeachSingleItemDictionary | 1.888 ns | 0.0387 ns | 0.0323 ns | 1.886 ns | 0.0017 |      32 B |
| ForeachDictionary           | 4.386 ns | 0.1154 ns | 0.2720 ns | 4.276 ns | 0.0025 |      48 B |

 */

public class SingleItemDictionaryBenchmarks
{
   private readonly IReadOnlyDictionary<int, int> _singleItemSet = SingleItem.Dictionary(42, 42);
   private readonly IReadOnlyDictionary<int, int> _hashset = new Dictionary<int, int> { { 42, 42 } };

   [Benchmark]
   public void ForeachSingleItemDictionary()
   {
      foreach (var _ in _singleItemSet)
      {
      }
   }

   [Benchmark]
   public void ForeachDictionary()
   {
      foreach (var _ in _hashset)
      {
      }
   }
}
