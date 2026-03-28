using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Thinktecture.Database;

namespace Thinktecture.Benchmarks;

/*

Runtime=.NET 10.0

| Method                              | Mean     | Error    | StdDev   | Ratio | Gen0   | Allocated | Alloc Ratio |
|------------------------------------ |---------:|---------:|---------:|------:|-------:|----------:|------------:|
| StringBased_Class_SpanParsable      | 29.44 ns | 0.228 ns | 0.213 ns |  1.02 | 0.0013 |      24 B |        0.75 |
| StringBased_Class_not_SpanParsable  | 31.97 ns | 0.364 ns | 0.341 ns |  1.10 | 0.0029 |      56 B |        1.75 |
| StringBased_Struct_SpanParsable     | 26.31 ns | 0.242 ns | 0.214 ns |  0.91 |      - |         - |        0.00 |
| StringBased_Struct_not_SpanParsable | 29.00 ns | 0.186 ns | 0.174 ns |  1.00 | 0.0017 |      32 B |        1.00 |

 */

public class ValueObjectJsonDeserializationBenchmarks
{
   private readonly byte[] _stringBasedJson = "\"Value\""u8.ToArray();
   private readonly byte[] _intBasedJson = "1"u8.ToArray();

   private readonly JsonSerializerOptions _options = new();

   [Benchmark]
   public NameSpanParsable? StringBased_Class_SpanParsable()
   {
      return JsonSerializer.Deserialize<NameSpanParsable>(_stringBasedJson, _options);
   }

   [Benchmark]
   public Name? StringBased_Class_not_SpanParsable()
   {
      return JsonSerializer.Deserialize<Name>(_stringBasedJson, _options);
   }

   [Benchmark]
   public NameStructSpanParsable StringBased_Struct_SpanParsable()
   {
      return JsonSerializer.Deserialize<NameStructSpanParsable>(_stringBasedJson, _options);
   }

   [Benchmark(Baseline = true)]
   public NameStruct StringBased_Struct_not_SpanParsable()
   {
      return JsonSerializer.Deserialize<NameStruct>(_stringBasedJson, _options);
   }

   // [Benchmark]
   // public Counter? IntBased_Class()
   // {
   //    return JsonSerializer.Deserialize<Counter>(_intBasedJson, _options);
   // }
   //
   // [Benchmark]
   // public CounterStruct IntBased_Struct()
   // {
   //    return JsonSerializer.Deserialize<CounterStruct>(_intBasedJson, _options);
   // }
}
