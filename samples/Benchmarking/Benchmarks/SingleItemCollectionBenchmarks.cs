using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace Thinktecture.Benchmarks;

public class SingleItemCollectionBenchmarks
{
   private readonly IReadOnlyList<int> _singleItemCollection = SingleItem.Collection(42);
   private readonly IReadOnlyList<int> _list = new List<int> { 42 };
   private readonly IReadOnlyList<int> _array = [42];

   [Benchmark]
   public void ForeachSingleItemCollection()
   {
      foreach (var _ in _singleItemCollection)
      {
      }
   }

   [Benchmark]
   public void ForeachList()
   {
      foreach (var _ in _list)
      {
      }
   }

   [Benchmark]
   public void ForeachArray()
   {
      foreach (var _ in _array)
      {
      }
   }
}
