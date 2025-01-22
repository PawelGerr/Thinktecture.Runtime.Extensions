using System;
using BenchmarkDotNet.Attributes;

namespace Thinktecture.Benchmarks;

/*

22.01.2025

.NET 9.0.1

| Method           | Mean     | Error    | StdDev   | Median   | Allocated |
|----------------- |---------:|---------:|---------:|---------:|----------:|
| Guid_NewGuid     | 56.12 ns | 2.936 ns | 8.658 ns | 59.00 ns |         - |
| CustomerId_NewId | 41.61 ns | 0.861 ns | 1.024 ns | 41.94 ns |         - |

 */

public class TypedIdBenchmarks
{
   [Benchmark]
   public Guid Guid_NewGuid()
   {
      return Guid.NewGuid();
   }

   [Benchmark]
   public CustomerId CustomerId_NewId()
   {
      return CustomerId.NewId();
   }
}

[ValueObject<Guid>]
public partial struct CustomerId
{
   public static CustomerId NewId() => new(Guid.NewGuid());
}
