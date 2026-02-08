using System.Text.Json;
using BenchmarkDotNet.Attributes;
using Thinktecture.Database;

namespace Thinktecture.Benchmarks;

public class SmartEnumJsonDeserializationBenchmarks
{
   private readonly byte[] _stringBasedJson = "\"Value1\""u8.ToArray();
   private readonly byte[] _intBasedJson = "1"u8.ToArray();

   private readonly JsonSerializerOptions _options = new();

   [Benchmark]
   public TestSmartEnum_Class_StringBased? StringBased_with_ISpanParsable()
   {
      return JsonSerializer.Deserialize<TestSmartEnum_Class_StringBased>(_stringBasedJson, _options);
   }

   [Benchmark(Baseline = true)]
   public TestSmartEnum_Class_StringBased_Without_SpanParsableConverter? StringBased_without_ISpanParsable()
   {
      return JsonSerializer.Deserialize<TestSmartEnum_Class_StringBased_Without_SpanParsableConverter>(_stringBasedJson, _options);
   }

   // [Benchmark]
   // public TestSmartEnum_Class_IntBased? IntBased()
   // {
   //    return JsonSerializer.Deserialize<TestSmartEnum_Class_IntBased>(_intBasedJson, _options);
   // }
}
