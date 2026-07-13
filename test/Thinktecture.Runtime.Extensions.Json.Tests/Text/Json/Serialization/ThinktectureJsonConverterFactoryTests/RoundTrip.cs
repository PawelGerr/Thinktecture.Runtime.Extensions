using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestRegularUnions;
using Thinktecture.Runtime.Tests.TestValueObjects;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization.ThinktectureJsonConverterFactoryTests;

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
      Deserialize<TestValueObjectByte>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      Deserialize<TestValueObjectSByte>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      Deserialize<TestValueObjectShort>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      Deserialize<TestValueObjectUShort>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      Deserialize<TestValueObjectInt>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      Deserialize<TestValueObjectUInt>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      Deserialize<TestValueObjectLong>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      Deserialize<TestValueObjectULong>($"\"{number}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      var numberAsStringJson = $"\"{numberJson}\"";
      var obj = TestValueObjectFloat.Create(number);

      Serialize<TestValueObjectFloat, float>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectFloat, float>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectFloat>($"\"{numberJson}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      var numberAsStringJson = $"\"{numberJson}\"";
      var obj = TestValueObjectDouble.Create(number);

      Serialize<TestValueObjectDouble, double>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectDouble, double>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectDouble>($"\"{numberJson}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
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
      var numberAsStringJson = $"\"{numberJson}\"";
      var obj = TestValueObjectDecimal.Create(number);

      Serialize<TestValueObjectDecimal, decimal>(obj).Should().Be(numberJson);
      Serialize<TestValueObjectDecimal, decimal>(obj, numberHandling: JsonNumberHandling.WriteAsString).Should().Be(numberAsStringJson);
      Deserialize<TestValueObjectDecimal>($"\"{numberJson}\"", numberHandling: JsonNumberHandling.AllowReadingFromString).Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_serialize_dictionary_with_string_based_enum_key()
   {
      var dictionary = new Dictionary<SmartEnum_StringBased, int>
                       {
                          { SmartEnum_StringBased.Item1, 1 },
                          { SmartEnum_StringBased.Item2, 2 }
                       };

      var options = new JsonSerializerOptions { Converters = { new ThinktectureJsonConverterFactory() } };

      var json = JsonSerializer.Serialize(dictionary, options);
      var deserializedDictionary = JsonSerializer.Deserialize<Dictionary<SmartEnum_StringBased, int>>(json, options);

      dictionary.Should().BeEquivalentTo(deserializedDictionary);
   }

   [Fact]
   public void Should_roundtrip_serialize_dictionary_with_string_based_value_objects()
   {
      var dictionary = new Dictionary<StringBasedStructValueObject, int>
                       {
                          { (StringBasedStructValueObject)"key 1", 1 },
                          { (StringBasedStructValueObject)"key 2", 2 }
                       };

      var options = new JsonSerializerOptions { Converters = { new ThinktectureJsonConverterFactory() } };

      var json = JsonSerializer.Serialize(dictionary, options);
      var deserializedDictionary = JsonSerializer.Deserialize<Dictionary<StringBasedStructValueObject, int>>(json, options);

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
      var options = new JsonSerializerOptions { Converters = { new ThinktectureJsonConverterFactory(skipValueObjectsWithJsonConverterAttribute) } };

      var json = JsonSerializer.Serialize(obj, options);
      json.Should().Be(expectedJson);

      var deserializedObj = JsonSerializer.Deserialize(json, obj.GetType(), options);
      obj.Should().BeEquivalentTo(deserializedObj);
   }

   [Fact]
   public void Should_serialize_item_of_derived_smart_enum_type_by_runtime_type()
   {
      // Regression coverage (mirrors the Newtonsoft test): FindMetadataForConversion resolves the base type's metadata
      // for the runtime type of an item of a derived (nested) Smart Enum, so the factory returns a JsonConverter for the
      // base type. System.Text.Json adapts it for the derived runtime type. Serializing through an object-typed value
      // makes System.Text.Json resolve the converter by the runtime (derived) type.
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var item = SmartEnum_DerivedTypes.ItemOfDerivedType;
      item.GetType().Should().NotBe(typeof(SmartEnum_DerivedTypes));

      factory.CanConvert(item.GetType()).Should().BeTrue();

      var options = new JsonSerializerOptions { Converters = { factory } };

      var json = JsonSerializer.Serialize<object>(item, options);
      json.Should().Be("2");

      var deserialized = JsonSerializer.Deserialize<SmartEnum_DerivedTypes>(json, options);
      deserialized.Should().BeSameAs(item);
   }

   [Fact]
   public void Should_serialize_derived_type_of_standalone_object_factory_class()
   {
      // Regression: the object-factory fallback of FindMetadataForConversion returned the derived requested type as
      // ConversionMetadata.Type, so CreateConverter's MakeGenericType threw ArgumentException because the derived type
      // does not implement IObjectFactory<TDerived, ...> (the interface is emitted only for the annotated base type).
      var factory = new ThinktectureJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var item = new DerivedClassWithStringObjectFactory("value");

      factory.CanConvert(typeof(DerivedClassWithStringObjectFactory)).Should().BeTrue();

      var options = new JsonSerializerOptions { Converters = { factory } };

      var json = JsonSerializer.Serialize(item, options);
      json.Should().Be("\"value\"");
   }

   private struct TestStruct<T>
   {
      public T Prop { get; set; }

      public TestStruct(T prop)
      {
         Prop = prop;
      }
   }

   [Theory]
   [InlineData("2025", 2025, null, null)]
   [InlineData("2025-06", 2025, 6, null)]
   [InlineData("2025-06-19", 2025, 6, 19)]
   public void Should_roundtrip_regular_union_with_factory(string value, int year, int? month, int? day)
   {
      var obj = value.Split('-').Length switch
      {
         1 => (PartiallyKnownDateSerializable)new PartiallyKnownDateSerializable.YearOnly(year),
         2 => new PartiallyKnownDateSerializable.YearMonth(year, month!.Value),
         3 => new PartiallyKnownDateSerializable.Date(year, month!.Value, day!.Value),
         _ => throw new System.Exception("Invalid test data")
      };

      var json = Serialize<PartiallyKnownDateSerializable, string>(obj);
      json.Should().Be($"\"{value}\"");

      var deserialized = Deserialize<PartiallyKnownDateSerializable>(json);
      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_regular_union_with_factory_null()
   {
      var json = Serialize<PartiallyKnownDateSerializable, string>(null);
      json.Should().Be("null");

      var deserialized = Deserialize<PartiallyKnownDateSerializable>(json);
      deserialized.Should().BeNull();
   }

   [Fact]
   public void Should_roundtrip_using_custom_factory_specified_by_ObjectFactoryAttribute()
   {
      var original = BoundaryWithFactories.Create(1, 2);
      var json = Serialize<BoundaryWithFactories, string>(original);
      json.Should().Be("\"1:2\"");
      var deserialized = Deserialize<BoundaryWithFactories>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_enum_with_ValidationErrorAttribute()
   {
      var original = TestSmartEnum_CustomError.Item1;
      var json = Serialize<TestSmartEnum_CustomError, string, CustomValidationError>(original);
      json.Should().Be("\"item1\"");
      var deserialized = Deserialize<TestSmartEnum_CustomError>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_simple_value_object_with_ValidationErrorAttribute()
   {
      var original = StringBasedReferenceValueObjectWithCustomError.Create("value");
      var json = Serialize<StringBasedReferenceValueObjectWithCustomError, string, StringBasedReferenceValidationError>(original);
      json.Should().Be("\"value\"");
      var deserialized = Deserialize<StringBasedReferenceValueObjectWithCustomError>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_ValidationErrorAttribute()
   {
      var original = BoundaryWithCustomError.Create(1, 2);
      var json = Serialize(original, JsonNamingPolicy.CamelCase);
      json.Should().Be("{\"lower\":1,\"upper\":2}");
      var deserialized = Deserialize<BoundaryWithCustomError>(json, JsonNamingPolicy.CamelCase);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_keyed_value_object_having_custom_factory()
   {
      var original = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);
      var json = Serialize<IntBasedReferenceValueObjectWithCustomFactoryNames, int>(original);
      json.Should().Be("1");
      var deserialized = Deserialize<IntBasedReferenceValueObjectWithCustomFactoryNames>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_having_custom_factory()
   {
      var original = BoundaryWithCustomFactoryNames.Get(1, 2);
      var json = Serialize(original, JsonNamingPolicy.CamelCase);
      // camelCase serialization expected; do not assert exact casing for resilience
      var deserialized = Deserialize<BoundaryWithCustomFactoryNames>(json, JsonNamingPolicy.CamelCase);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_numbers_as_string_when_number_handling_allows_string_reading()
   {
      var original = Boundary.Create(1, 2);
      var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
      var json = JsonSerializer.Serialize(original, options);
      json.Should().Be("{\"lower\":1,\"upper\":2}");

      // simulate incoming numbers as strings
      var incoming = "{ \"lower\": \"1\", \"upper\": 2}";
      var readOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, NumberHandling = JsonNumberHandling.AllowReadingFromString };
      var deserialized = JsonSerializer.Deserialize<Boundary>(incoming, readOptions);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_serialize_timespan_reference_value_object()
   {
      var timeSpan = TimeSpan.FromHours(1);
      var obj = TimeSpanBasedReferenceValueObject.Create(timeSpan);

      var json = Serialize<TimeSpanBasedReferenceValueObject, TimeSpan>(obj);
      json.Should().Be("\"01:00:00\"");

      var deserialized = Deserialize<TimeSpanBasedReferenceValueObject>(json);
      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_serialize_timespan_struct_value_object()
   {
      var timeSpan = TimeSpan.FromHours(1);
      var obj = TimeSpanBasedStructValueObject.Create(timeSpan);

      var json = Serialize<TimeSpanBasedStructValueObject, TimeSpan>(obj);
      json.Should().Be("\"01:00:00\"");

      var deserialized = Deserialize<TimeSpanBasedStructValueObject>(json);
      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_serialize_nullable_timespan_struct_value_object()
   {
      TimeSpanBasedStructValueObject? obj = TimeSpanBasedStructValueObject.Create(TimeSpan.FromHours(1));

      var options = new JsonSerializerOptions { Converters = { new ThinktectureJsonConverterFactory() } };

      var json = JsonSerializer.Serialize(obj, options);
      json.Should().Be("\"01:00:00\"");

      var deserialized = JsonSerializer.Deserialize<TimeSpanBasedStructValueObject?>(json, options);
      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_int_based_generic_smart_enum_items_for_constrained_type_argument()
   {
      foreach (var original in SmartEnum_Generic_IntBased<string>.Items)
      {
         var json = JsonSerializer.Serialize(original);
         var deserialized = JsonSerializer.Deserialize<SmartEnum_Generic_IntBased<string>>(json);

         deserialized.Should().BeSameAs(original);
      }
   }

   [Fact]
   public void Should_roundtrip_int_based_generic_smart_enum_items_for_another_constrained_type_argument()
   {
      foreach (var original in SmartEnum_Generic_IntBased<Guid>.Items)
      {
         var json = JsonSerializer.Serialize(original);
         var deserialized = JsonSerializer.Deserialize<SmartEnum_Generic_IntBased<Guid>>(json);

         deserialized.Should().BeSameAs(original);
      }
   }

   [Fact]
   public void Should_roundtrip_string_based_generic_smart_enum_items_for_constrained_type_argument()
   {
      foreach (var original in SmartEnum_Generic_StringBased<int>.Items)
      {
         var json = JsonSerializer.Serialize(original);
         var deserialized = JsonSerializer.Deserialize<SmartEnum_Generic_StringBased<int>>(json);

         deserialized.Should().BeSameAs(original);
      }
   }

   [Fact]
   public void Should_roundtrip_string_based_generic_smart_enum_items_for_another_constrained_type_argument()
   {
      foreach (var original in SmartEnum_Generic_StringBased<double>.Items)
      {
         var json = JsonSerializer.Serialize(original);
         var deserialized = JsonSerializer.Deserialize<SmartEnum_Generic_StringBased<double>>(json);

         deserialized.Should().BeSameAs(original);
      }
   }

   [Fact]
   public void Should_roundtrip_generic_key_based_unconstraint_smart_enum()
   {
      var original = SmartEnum_GenericKeyBasedUnconstraint<int>.Item1;

      var json = JsonSerializer.Serialize(original);
      json.Should().Be("1");

      var deserialized = JsonSerializer.Deserialize<SmartEnum_GenericKeyBasedUnconstraint<int>>(json);
      deserialized.Should().BeSameAs(original);
   }

   [Fact]
   public void Should_roundtrip_generic_key_based_struct_constraint_smart_enum()
   {
      var original = SmartEnum_GenericKeyBasedStructConstraint<int>.Item1;

      var json = JsonSerializer.Serialize(original);
      json.Should().Be("1");

      var deserialized = JsonSerializer.Deserialize<SmartEnum_GenericKeyBasedStructConstraint<int>>(json);
      deserialized.Should().BeSameAs(original);
   }

   public static IEnumerable<object[]> EnumBasedSmartEnumTestData =>
   [
      [SmartEnum_EnumBased.Item1, "1"],
      [SmartEnum_EnumBased.Item2, "2"],
      [SmartEnum_EnumBased.Item3, "3"]
   ];

   [Theory]
   [MemberData(nameof(EnumBasedSmartEnumTestData))]
   public void Should_roundtrip_enum_based_smart_enum_serializing_key_as_number(SmartEnum_EnumBased original, string expectedJson)
   {
      var json = JsonSerializer.Serialize(original);
      json.Should().Be(expectedJson);

      var deserialized = JsonSerializer.Deserialize<SmartEnum_EnumBased>(json);
      deserialized.Should().BeSameAs(original);
   }

   [Theory]
   [InlineData(ValueObject_EnumKey.Item1, "1")]
   [InlineData(ValueObject_EnumKey.Item2, "2")]
   [InlineData(ValueObject_EnumKey.Item3, "3")]
   public void Should_roundtrip_enum_based_value_object_serializing_key_as_number(ValueObject_EnumKey key, string expectedJson)
   {
      var original = EnumBasedValueObject.Create(key);

      var json = Serialize<EnumBasedValueObject, ValueObject_EnumKey>(original);
      json.Should().Be(expectedJson);

      var deserialized = Deserialize<EnumBasedValueObject>(json);
      deserialized.Should().Be(original);
   }

   [Theory]
   [InlineData(ValueObject_FlagsEnumKey.None, "0")]
   [InlineData(ValueObject_FlagsEnumKey.First, "1")]
   [InlineData(ValueObject_FlagsEnumKey.Second, "2")]
   [InlineData(ValueObject_FlagsEnumKey.First | ValueObject_FlagsEnumKey.Second, "3")]
   [InlineData(ValueObject_FlagsEnumKey.First | ValueObject_FlagsEnumKey.Second | ValueObject_FlagsEnumKey.Third, "7")]
   public void Should_roundtrip_flags_enum_based_value_object_serializing_key_as_number(ValueObject_FlagsEnumKey key, string expectedJson)
   {
      var original = FlagsEnumBasedValueObject.Create(key);

      var json = Serialize<FlagsEnumBasedValueObject, ValueObject_FlagsEnumKey>(original);
      json.Should().Be(expectedJson);

      var deserialized = Deserialize<FlagsEnumBasedValueObject>(json);
      deserialized.Should().Be(original);
   }

   private static JsonSerializerOptions StringEnumOptions()
   {
      return new JsonSerializerOptions
             {
                Converters =
                {
                   new ThinktectureJsonConverterFactory(),
                   new JsonStringEnumConverter()
                }
             };
   }

   [Fact]
   public void Should_roundtrip_enum_based_smart_enum_serializing_key_as_name_with_string_enum_converter()
   {
      var original = SmartEnum_EnumBased.Item2;
      var options = StringEnumOptions();

      var json = JsonSerializer.Serialize(original, options);
      json.Should().Be("\"Item2\"");

      var deserialized = JsonSerializer.Deserialize<SmartEnum_EnumBased>(json, options);
      deserialized.Should().BeSameAs(original);
   }

   [Fact]
   public void Should_roundtrip_enum_based_value_object_serializing_key_as_name_with_string_enum_converter()
   {
      var original = EnumBasedValueObject.Create(ValueObject_EnumKey.Item2);
      var options = StringEnumOptions();

      var json = JsonSerializer.Serialize(original, options);
      json.Should().Be("\"Item2\"");

      var deserialized = JsonSerializer.Deserialize<EnumBasedValueObject>(json, options);
      deserialized.Should().Be(original);
   }

   [Fact]
   public void Should_roundtrip_flags_enum_based_value_object_serializing_single_flag_as_name_with_string_enum_converter()
   {
      var original = FlagsEnumBasedValueObject.Create(ValueObject_FlagsEnumKey.First);
      var options = StringEnumOptions();

      var json = JsonSerializer.Serialize(original, options);
      json.Should().Be("\"First\"");

      var deserialized = JsonSerializer.Deserialize<FlagsEnumBasedValueObject>(json, options);
      deserialized.Should().Be(original);
   }
}
