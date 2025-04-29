using System;
using BenchmarkDotNet.Attributes;

namespace Thinktecture.Benchmarks;

/*

29.04.2025

.NET 9.0.4

| Method              | Mean     | Error    | StdDev   | Allocated |
|-------------------- |---------:|---------:|---------:|----------:|
| Guid_CreateVersion7 | 54.24 ns | 0.398 ns | 0.372 ns |         - |
| CustomerId_NewId    | 57.59 ns | 0.188 ns | 0.176 ns |         - |

 */

public class TypedIdBenchmarks
{
   [Benchmark]
   public Guid Guid_CreateVersion7()
   {
      return Guid.CreateVersion7();
   }

   [Benchmark]
   public CustomerId CustomerId_NewId()
   {
      return CustomerId.NewId();
   }
}

[ValueObject<Guid>(ConversionToKeyMemberType = ConversionOperatorsGeneration.Explicit)]
public partial struct CustomerId
{
   public static CustomerId NewId() => new(Guid.CreateVersion7());
}
