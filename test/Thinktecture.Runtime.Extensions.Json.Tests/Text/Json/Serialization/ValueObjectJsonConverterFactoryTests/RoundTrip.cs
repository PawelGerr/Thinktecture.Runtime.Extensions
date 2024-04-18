using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ValueObjectJsonConverterFactoryTests;

public class RoundTrip : JsonTestsBase
{
   [Theory]
   [InlineData(byte.MinValue)]
   [InlineData(1)]
   [InlineData(byte.MaxValue)]
   public void Should_deserialize_byte_from_string_with_corresponding_NumberHandling(byte number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectByte.Create(number);

      Serialize<TestValueObjectByte, byte>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectByte, byte>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectByte, byte>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(sbyte.MinValue)]
   [InlineData(-1)]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(sbyte.MaxValue)]
   public void Should_deserialize_sbyte_from_string_with_corresponding_NumberHandling(sbyte number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectSByte.Create(number);

      Serialize<TestValueObjectSByte, sbyte>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectSByte, sbyte>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectSByte, sbyte>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(short.MinValue)]
   [InlineData(-1)]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(short.MaxValue)]
   public void Should_deserialize_short_from_string_with_corresponding_NumberHandling(short number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectShort.Create(number);

      Serialize<TestValueObjectShort, short>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectShort, short>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectShort, short>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(ushort.MinValue)]
   [InlineData(1)]
   [InlineData(ushort.MaxValue)]
   public void Should_deserialize_ushort_from_string_with_corresponding_NumberHandling(ushort number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectUShort.Create(number);

      Serialize<TestValueObjectUShort, ushort>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectUShort, ushort>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectUShort, ushort>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(int.MinValue)]
   [InlineData(-1)]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(int.MaxValue)]
   public void Should_deserialize_int_from_string_with_corresponding_NumberHandling(int number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectInt.Create(number);

      Serialize<TestValueObjectInt, int>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectInt, int>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectInt, int>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(uint.MinValue)]
   [InlineData(1)]
   [InlineData(uint.MaxValue)]
   public void Should_deserialize_uint_from_string_with_corresponding_NumberHandling(uint number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectUInt.Create(number);

      Serialize<TestValueObjectUInt, uint>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectUInt, uint>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectUInt, uint>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(long.MinValue)]
   [InlineData(-1)]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(long.MaxValue)]
   public void Should_deserialize_long_from_string_with_corresponding_NumberHandling(long number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectLong.Create(number);

      Serialize<TestValueObjectLong, long>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectLong, long>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectLong, long>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(ulong.MinValue)]
   [InlineData(1)]
   [InlineData(ulong.MaxValue)]
   public void Should_deserialize_ulong_from_string_with_corresponding_NumberHandling(ulong number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectULong.Create(number);

      Serialize<TestValueObjectULong, ulong>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectULong, ulong>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectULong, ulong>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(float.MinValue)]
   [InlineData(-1)]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(float.MaxValue)]
   public void Should_deserialize_float_from_string_with_corresponding_NumberHandling(float number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectFloat.Create(number);

      Serialize<TestValueObjectFloat, float>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectFloat, float>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectFloat, float>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(double.MinValue)]
   [InlineData(-1.234)]
   [InlineData(-1)]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(1.234)]
   [InlineData(double.MaxValue)]
   public void Should_deserialize_double_from_string_with_corresponding_NumberHandling(double number)
   {
      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectDouble.Create(number);

      Serialize<TestValueObjectDouble, double>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectDouble, double>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectDouble, double>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(2)]
   [InlineData(3)]
   [InlineData(4)]
   [InlineData(5)]
   [InlineData(6)]
   public void Should_deserialize_decimal_from_string_with_corresponding_NumberHandling(int index)
   {
      var decimals = new[] { decimal.MinValue, -1.234m, -1m, 0m, 1m, 1.234m, decimal.MaxValue };
      var number = decimals[index];

      var numberJson = number.ToString(CultureInfo.InvariantCulture);
      var numberAsStringJson = $"\"{number}\"";
      var obj = TestValueObjectDecimal.Create(number);

      Serialize<TestValueObjectDecimal, decimal>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectDecimal, decimal>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectDecimal, decimal>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_serialize_dictionary_with_string_based_enum_key()
   {
      var dictionary = new Dictionary<TestSmartEnum_Class_StringBased, int>
      {
         { TestSmartEnum_Class_StringBased.Value1, 1 },
         { TestSmartEnum_Class_StringBased.Value2, 2 }
      };

      var options = new JsonSerializerOptions { Converters = { new ValueObjectJsonConverterFactory() } };

      var json = JsonSerializer.Serialize(dictionary, options);
      var deserializedDictionary = JsonSerializer.Deserialize<Dictionary<TestSmartEnum_Class_StringBased, int>>(json, options);

      dictionary.Should().BeEquivalentTo(deserializedDictionary);
   }

   public static IEnumerable<object[]> ObjectWithStructTestData =
   [
      [new { Prop = IntBasedStructValueObject.Create(42) }, """{"Prop":42}"""],
      [new { Prop = (IntBasedStructValueObject?)IntBasedStructValueObject.Create(42) }, """{"Prop":42}"""],
      [new { Prop = IntBasedReferenceValueObject.Create(42) }, """{"Prop":42}"""],
      [new TestStruct<IntBasedStructValueObject>(IntBasedStructValueObject.Create(42)), """{"Prop":42}"""],
      [new TestStruct<IntBasedStructValueObject?>(IntBasedStructValueObject.Create(42)), """{"Prop":42}"""],
      [new TestStruct<IntBasedReferenceValueObject>(IntBasedReferenceValueObject.Create(42)), """{"Prop":42}"""],
   ];

   [Theory]
   [MemberData(nameof(ObjectWithStructTestData))]
   public void Should_roundtrip_serialize_types_with_struct_properties_using_non_generic_factory(
      object obj,
      string expectedJson)
   {
      Roundtrip_serialize_types_with_struct_properties_using_non_generic_factory(true, obj, expectedJson);
      Roundtrip_serialize_types_with_struct_properties_using_non_generic_factory(false, obj, expectedJson);
   }

   private static void Roundtrip_serialize_types_with_struct_properties_using_non_generic_factory(
      bool skipValueObjectsWithJsonConverterAttribute,
      object obj,
      string expectedJson)
   {
      var options = new JsonSerializerOptions { Converters = { new ValueObjectJsonConverterFactory(skipValueObjectsWithJsonConverterAttribute) } };

      var json = JsonSerializer.Serialize(obj, options);
      json.Should().Be(expectedJson);

      var deserializedObj = JsonSerializer.Deserialize(json, obj.GetType(), options);
      obj.Should().BeEquivalentTo(deserializedObj);
   }

   private struct TestStruct<T>
   {
      public T Prop { get; set; }

      public TestStruct(T prop)
      {
         Prop = prop;
      }
   }
}
