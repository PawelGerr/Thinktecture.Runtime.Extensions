using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Thinktecture.Benchmarks;

[MemoryDiagnoser]
public class SingleItemSetBenchmarks
{
   private readonly IReadOnlySet<int> _singleItemSet = SingleItem.Set(42);
   private readonly IReadOnlySet<int> _hashset = new HashSet<int> { 42 };

   [Benchmark]
   public void ForeachSingleItemSet()
   {
      foreach (var item in _singleItemSet)
      {
      }
   }

   [Benchmark]
   public void ForeachHashSet()
   {
      foreach (var item in _hashset)
      {
      }
   }
}
