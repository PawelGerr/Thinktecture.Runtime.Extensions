using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Thinktecture.Internal;
using Thinktecture.Json;
using Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests.TestClasses;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestRegularUnions;
using Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable InconsistentNaming

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests;

public class RoundTrip : JsonTestsBase
{
   [Fact]
   public void Should_not_let_shared_converter_cache_bypass_skip_attribute_check_across_factory_instances()
   {
      // Regression: the converter cache is a static field shared by all factory instances, but the skip-attribute
      // policy is per-instance. A converter cached by a non-skipping instance must not let a skipping instance's
      // CanConvert short-circuit to true and thereby ignore the type's own foreign [JsonConverter] attribute.
      var type = typeof(ValueObjectWithForeignNewtonsoftConverter);

      var nonSkippingFactory = new ThinktectureNewtonsoftJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var skippingFactory = new ThinktectureNewtonsoftJsonConverterFactory(skipObjectsWithJsonConverterAttribute: true);

      // The non-skipping factory converts the type and, by serializing an instance, populates the shared static cache.
      nonSkippingFactory.CanConvert(type).Should().BeTrue();

      var instance = ValueObjectWithForeignNewtonsoftConverter.Create(1);
      var sb = new StringBuilder();
      using (var writer = new JsonTextWriter(new StringWriter(sb)))
      {
         nonSkippingFactory.WriteJson(writer, instance, JsonSerializer.CreateDefault());
      }

      // The skipping factory must still refuse the type because it carries a foreign [JsonConverter] attribute.
      skippingFactory.CanConvert(type).Should().BeFalse();
   }

   [Fact]
   public void Should_roundtrip_serialize_dictionary_with_string_based_enum_key()
   {
      var dictionary = new Dictionary<SmartEnum_StringBased, int>
                       {
                          { SmartEnum_StringBased.Item1, 1 },
                          { SmartEnum_StringBased.Item2, 2 }
                       };

      var json = JsonConvert.SerializeObject(dictionary);
      var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<SmartEnum_StringBased, int>>(json);

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

      var json = JsonConvert.SerializeObject(dictionary);
      var deserializedDictionary = JsonConvert.DeserializeObject<Dictionary<StringBasedStructValueObject, int>>(json);

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
      var options = new JsonSerializerSettings { Converters = { new ThinktectureNewtonsoftJsonConverterFactory() } };

      var json = JsonConvert.SerializeObject(obj, options);
      json.Should().Be(expectedJson);

      var deserializedObj = JsonConvert.DeserializeObject(json, obj.GetType(), options);
      obj.Should().BeEquivalentTo(deserializedObj);
   }

   [Theory]
   [InlineData("2025", 2025, null, null)]
   [InlineData("2025-06", 2025, 6, null)]
   [InlineData("2025-06-19", 2025, 6, 19)]
   public void Should_roundtrip_PartiallyKnownDateSerializable(string value, int year, int? month, int? day)
   {
      var obj = value.Split('-').Length switch
      {
         1 => (PartiallyKnownDateSerializable)new PartiallyKnownDateSerializable.YearOnly(year),
         2 => new PartiallyKnownDateSerializable.YearMonth(year, month!.Value),
         3 => new PartiallyKnownDateSerializable.Date(year, month!.Value, day!.Value),
         _ => throw new System.Exception("Invalid test data")
      };

      var json = JsonConvert.SerializeObject(obj);
      json.Should().Be($"\"{value}\"");

      var deserialized = JsonConvert.DeserializeObject<PartiallyKnownDateSerializable>(json);
      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_regular_union_with_factory()
   {
      var json = JsonConvert.SerializeObject((PartiallyKnownDateSerializable)null);
      json.Should().Be("null");

      var deserialized = JsonConvert.DeserializeObject<PartiallyKnownDateSerializable>(json);
      deserialized.Should().BeNull();
   }

   [Fact]
   public void Should_roundtrip_using_custom_factory_specified_by_ObjectFactoryAttribute()
   {
      var original = BoundaryWithFactories.Create(1, 2);
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("\"1:2\"");

      var deserialized = JsonConvert.DeserializeObject<BoundaryWithFactories>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_enum_with_ValidationErrorAttribute()
   {
      var original = TestSmartEnum_CustomError.Item1;
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("\"item1\"");

      var deserialized = JsonConvert.DeserializeObject<TestSmartEnum_CustomError>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_simple_value_object_with_ValidationErrorAttribute()
   {
      var original = StringBasedReferenceValueObjectWithCustomError.Create("value");
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("\"value\"");

      var deserialized = JsonConvert.DeserializeObject<StringBasedReferenceValueObjectWithCustomError>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_ValidationErrorAttribute()
   {
      var original = BoundaryWithCustomError.Create(1, 2);
      var json = JsonConvert.SerializeObject(original, new JsonSerializerSettings
                                                       {
                                                          ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() }
                                                       });

      json.Should().Be("{\"lower\":1.0,\"upper\":2.0}");

      var deserialized = JsonConvert.DeserializeObject<BoundaryWithCustomError>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_non_default_naming_strategy()
   {
      // Snake case inserts a separator between words (e.g. "StructProperty" => "struct_property"). Unlike
      // CamelCaseNamingStrategy, it changes more than the first letter, so an ordinal-ignore-case read that compares
      // against the raw argument name no longer matches. This exercises the read path's use of the contract resolver.
      var original = ValueObjectWithMultipleProperties.Create(1.5m, 3, "foo");
      var settings = new JsonSerializerSettings
                     {
                        ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
                     };

      var json = JsonConvert.SerializeObject(original, settings);

      // The leading quote is required: without it the assertion would already be satisfied by "nullable_struct_property".
      json.Should().Contain("\"struct_property\"");
      json.Should().NotContain("structProperty");

      var deserialized = JsonConvert.DeserializeObject<ValueObjectWithMultipleProperties>(json, settings);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_keyed_value_object_having_custom_factory()
   {
      var original = IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1);
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("1");

      var deserialized = JsonConvert.DeserializeObject<IntBasedReferenceValueObjectWithCustomFactoryNames>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_having_custom_factory()
   {
      var original = BoundaryWithCustomFactoryNames.Get(1, 2);
      var json = JsonConvert.SerializeObject(original);

      var deserialized = JsonConvert.DeserializeObject<BoundaryWithCustomFactoryNames>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_keyed_smart_enum_nested_in_generic_class()
   {
      var original = SmartEnums_NestedInGenericClass.GenericOuter<int>.KeyedSmartEnum.Item;
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("1");

      var deserialized = JsonConvert.DeserializeObject<SmartEnums_NestedInGenericClass.GenericOuter<int>.KeyedSmartEnum>(json);
      deserialized.Should().BeSameAs(original);
   }

   [Fact]
   public void Should_roundtrip_keyed_value_object_nested_in_generic_class()
   {
      var original = ValueObjects_NestedInGenericClass.GenericOuter<int>.KeyedValueObject.Create(42);
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("42");

      var deserialized = JsonConvert.DeserializeObject<ValueObjects_NestedInGenericClass.GenericOuter<int>.KeyedValueObject>(json);
      deserialized.Should().Be(original);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_nested_in_generic_class()
   {
      var original = ValueObjects_NestedInGenericClass.GenericOuter<int>.ComplexValueObject.Create(42);
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("{\"Value\":42}");

      var deserialized = JsonConvert.DeserializeObject<ValueObjects_NestedInGenericClass.GenericOuter<int>.ComplexValueObject>(json);
      deserialized.Should().Be(original);
   }

   [Fact]
   public void Should_roundtrip_serialize_timespan_reference_value_object()
   {
      var original = TimeSpanBasedReferenceValueObject.Create(TimeSpan.FromHours(1));
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("\"01:00:00\"");

      var deserialized = JsonConvert.DeserializeObject<TimeSpanBasedReferenceValueObject>(json);
      deserialized.Should().BeEquivalentTo(original);
   }

   [Fact]
   public void Should_roundtrip_serialize_timespan_struct_value_object()
   {
      var original = TimeSpanBasedStructValueObject.Create(TimeSpan.FromHours(1));
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("\"01:00:00\"");

      var deserialized = JsonConvert.DeserializeObject<TimeSpanBasedStructValueObject>(json);
      deserialized.Should().Be(original);
   }

   [Fact]
   public void Should_roundtrip_generic_key_based_unconstraint_smart_enum()
   {
      var original = SmartEnum_GenericKeyBasedUnconstraint<int>.Item1;
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("1");

      var deserialized = JsonConvert.DeserializeObject<SmartEnum_GenericKeyBasedUnconstraint<int>>(json);
      deserialized.Should().BeSameAs(original);
   }

   [Fact]
   public void Should_roundtrip_generic_key_based_struct_constraint_smart_enum()
   {
      var original = SmartEnum_GenericKeyBasedStructConstraint<int>.Item2;
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be("2");

      var deserialized = JsonConvert.DeserializeObject<SmartEnum_GenericKeyBasedStructConstraint<int>>(json);
      deserialized.Should().BeSameAs(original);
   }

   [Theory]
   [InlineData(1)]
   [InlineData(2)]
   [InlineData(3)]
   public void Should_roundtrip_enum_based_smart_enum(int key)
   {
      var original = SmartEnum_EnumBased.Get((SmartEnum_EnumKey)key);
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be(key.ToString());

      var deserialized = JsonConvert.DeserializeObject<SmartEnum_EnumBased>(json);
      deserialized.Should().BeSameAs(original);
   }

   [Theory]
   [InlineData(ValueObject_EnumKey.Item1, 1)]
   [InlineData(ValueObject_EnumKey.Item2, 2)]
   [InlineData(ValueObject_EnumKey.Item3, 3)]
   public void Should_roundtrip_enum_based_value_object(ValueObject_EnumKey key, int expectedKey)
   {
      var original = EnumBasedValueObject.Create(key);
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be(expectedKey.ToString());

      var deserialized = JsonConvert.DeserializeObject<EnumBasedValueObject>(json);
      deserialized.Should().Be(original);
   }

   [Theory]
   [InlineData(ValueObject_FlagsEnumKey.None, 0)]
   [InlineData(ValueObject_FlagsEnumKey.First, 1)]
   [InlineData(ValueObject_FlagsEnumKey.First | ValueObject_FlagsEnumKey.Second, 3)]
   [InlineData(ValueObject_FlagsEnumKey.First | ValueObject_FlagsEnumKey.Second | ValueObject_FlagsEnumKey.Third, 7)]
   public void Should_roundtrip_flags_enum_based_value_object(ValueObject_FlagsEnumKey key, int expectedKey)
   {
      var original = FlagsEnumBasedValueObject.Create(key);
      var json = JsonConvert.SerializeObject(original);

      json.Should().Be(expectedKey.ToString());

      var deserialized = JsonConvert.DeserializeObject<FlagsEnumBasedValueObject>(json);
      deserialized.Should().Be(original);
   }

   [Fact]
   public void Should_roundtrip_enum_based_smart_enum_using_StringEnumConverter()
   {
      var settings = new JsonSerializerSettings
                     {
                        Converters =
                        {
                           new ThinktectureNewtonsoftJsonConverterFactory(),
                           new Newtonsoft.Json.Converters.StringEnumConverter()
                        }
                     };

      var original = SmartEnum_EnumBased.Item2;
      var json = JsonConvert.SerializeObject(original, settings);

      json.Should().Be("\"Item2\"");

      var deserialized = JsonConvert.DeserializeObject<SmartEnum_EnumBased>(json, settings);
      deserialized.Should().BeSameAs(original);
   }

   [Fact]
   public void Should_roundtrip_enum_based_value_object_using_StringEnumConverter()
   {
      var settings = new JsonSerializerSettings
                     {
                        Converters =
                        {
                           new ThinktectureNewtonsoftJsonConverterFactory(),
                           new Newtonsoft.Json.Converters.StringEnumConverter()
                        }
                     };

      var original = EnumBasedValueObject.Create(ValueObject_EnumKey.Item2);
      var json = JsonConvert.SerializeObject(original, settings);

      json.Should().Be("\"Item2\"");

      var deserialized = JsonConvert.DeserializeObject<EnumBasedValueObject>(json, settings);
      deserialized.Should().Be(original);
   }

   [Fact]
   public void Should_roundtrip_item_of_derived_smart_enum_type_using_factory()
   {
      // Regression: FindMetadataForConversion returned null for a derived (nested) Smart Enum runtime type, so the
      // factory could not create a converter for such an item. The factory is exercised directly because, in
      // Newtonsoft.Json, the class-level [JsonConverter] attribute takes precedence over converters registered in the
      // settings, which would otherwise mask the defect at the JsonConvert level. skipObjectsWithJsonConverterAttribute
      // is false so that the factory handles the type itself (as when the domain assembly does not reference the
      // Newtonsoft integration package and thus has no generated attribute), which is the code path that failed.
      var factory = new ThinktectureNewtonsoftJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var item = SmartEnum_DerivedTypes.ItemOfDerivedType; // runtime type is a private nested derived class
      item.GetType().Should().NotBe(typeof(SmartEnum_DerivedTypes));

      factory.CanConvert(item.GetType()).Should().BeTrue();

      var sb = new StringBuilder();
      using (var writer = new JsonTextWriter(new StringWriter(sb)))
      {
         factory.WriteJson(writer, item, JsonSerializer.CreateDefault());
      }

      sb.ToString().Should().Be("2");

      using var reader = new JsonTextReader(new StringReader(sb.ToString()));
      reader.Read();
      var deserialized = factory.ReadJson(reader, typeof(SmartEnum_DerivedTypes), null, JsonSerializer.CreateDefault());

      deserialized.Should().BeSameAs(item);
   }

   [Fact]
   public void Should_not_convert_type_whose_only_object_factory_has_a_ref_struct_value_type()
   {
      // Regression: the object-factory filter of the factory excluded only ReadOnlySpan<char>, so a factory with any
      // other ref struct value type passed it. CreateConverter then called MakeGenericType with ReadOnlySpan<byte>,
      // which throws an ArgumentException because a ref struct cannot be a generic type argument. Every ref-struct
      // factory must be ignored instead, and because this type has no key-based metadata either, the factory must
      // refuse it entirely.
      var type = typeof(ClassWithByteSpanObjectFactoryMetadata);

      // Guard: the hand-written explicit interface member must really be discoverable, otherwise the assertion below
      // would hold even without the ref-struct exclusion (see the comment on ClassWithByteSpanObjectFactoryMetadata).
      MetadataLookup.FindMetadataForConversion(type, _ => true, _ => true).Should().NotBeNull();

      var factory = new ThinktectureNewtonsoftJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);

      factory.CanConvert(type).Should().BeFalse();
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_fall_back_to_key_based_conversion_for_span_object_factory_flagged_for_newtonsoft()
   {
      // Regression: a ReadOnlySpan<char> object factory flagged for Newtonsoft.Json must be ignored by the runtime
      // factory, which must fall back to the string key. Before the fix, CreateConverter built the converter with
      // ReadOnlySpan<char> as the generic key argument, throwing ArgumentException (a ref struct cannot be a generic
      // type argument). The factory is exercised directly (as when the domain assembly does not reference the
      // Newtonsoft integration package and thus has no generated attribute), which is the code path that failed.
      var factory = new ThinktectureNewtonsoftJsonConverterFactory(skipObjectsWithJsonConverterAttribute: false);
      var original = TestClasses.StringValueObjectWithSpanFactoryForNewtonsoft.Create("test-value");

      factory.CanConvert(original.GetType()).Should().BeTrue();

      var sb = new StringBuilder();
      using (var writer = new JsonTextWriter(new StringWriter(sb)))
      {
         factory.WriteJson(writer, original, JsonSerializer.CreateDefault());
      }

      sb.ToString().Should().Be("\"test-value\"");

      using var reader = new JsonTextReader(new StringReader(sb.ToString()));
      reader.Read();
      var deserialized = factory.ReadJson(reader, typeof(TestClasses.StringValueObjectWithSpanFactoryForNewtonsoft), null, JsonSerializer.CreateDefault());

      deserialized.Should().BeEquivalentTo(original);
   }
#endif

   private struct TestStruct<T>
   {
      public T Prop { get; set; }

      public TestStruct(T prop)
      {
         Prop = prop;
      }
   }
}
