using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Thinktecture.Benchmarks;

/*

29.04.2025

.NET 9.0.4

| Method               | Mean     | Error     | StdDev    | Gen0   | Allocated |
|--------------------- |---------:|----------:|----------:|-------:|----------:|
| ForeachSingleItemSet | 1.440 ns | 0.0663 ns | 0.0992 ns | 0.0013 |      24 B |
| ForeachHashSet       | 3.935 ns | 0.1010 ns | 0.0992 ns | 0.0021 |      40 B |

 */

public class SingleItemSetBenchmarks
{
   private readonly IReadOnlySet<int> _singleItemSet = SingleItem.Set(42);
   private readonly IReadOnlySet<int> _hashset = new HashSet<int> { 42 };

   [Benchmark]
   public void ForeachSingleItemSet()
   {
      foreach (var _ in _singleItemSet)
      {
      }
   }

   [Benchmark]
   public void ForeachHashSet()
   {
      foreach (var _ in _hashset)
      {
      }
   }
}
