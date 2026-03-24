using MessagePack;
using MessagePack.Resolvers;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Formatters.ThinktectureMessagePackFormatterTests;

/// <summary>
/// Regression tests for issue #19: verify ComplexValueObject with nullable SmartEnum properties
/// serializes and deserializes correctly with MessagePack.
/// </summary>
public class NullableSmartEnumPropertyTests
{
   private readonly MessagePackSerializerOptions _options;

   public NullableSmartEnumPropertyTests()
   {
      var resolver = CompositeResolver.Create(ThinktectureMessageFormatterResolver.Instance, StandardResolver.Instance);
      _options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_null_smart_enum_properties()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, null, 42);

      var bytes = MessagePackSerializer.Serialize(obj, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableSmartEnumProperty>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_null_string_based_and_non_null_int_based()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, SmartEnum_IntBased.Item1, 42);

      var bytes = MessagePackSerializer.Serialize(obj, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableSmartEnumProperty>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_non_null_string_based_and_null_int_based()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(SmartEnum_StringBased.Item1, null, 42);

      var bytes = MessagePackSerializer.Serialize(obj, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableSmartEnumProperty>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_non_null_smart_enum_properties()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(SmartEnum_StringBased.Item1, SmartEnum_IntBased.Item2, 42);

      var bytes = MessagePackSerializer.Serialize(obj, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableSmartEnumProperty>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_null_complex_value_object_as_null()
   {
      var bytes = MessagePackSerializer.Serialize<ComplexValueObjectWithNullableSmartEnumProperty>(null, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableSmartEnumProperty>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.Should().BeNull();
   }

   [Fact]
   public void Should_preserve_null_smart_enum_property_values_after_roundtrip()
   {
      var obj = ComplexValueObjectWithNullableSmartEnumProperty.Create(null, null, 42);

      var bytes = MessagePackSerializer.Serialize(obj, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableSmartEnumProperty>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.NullableStringBasedSmartEnum.Should().BeNull();
      deserialized.NullableIntBasedSmartEnum.Should().BeNull();
      deserialized.OtherProperty.Should().Be(42);
   }
}
