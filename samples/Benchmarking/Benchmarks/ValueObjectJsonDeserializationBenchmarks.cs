using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Thinktecture.Database;

namespace Thinktecture.Benchmarks;

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
