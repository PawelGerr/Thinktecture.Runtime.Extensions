using MessagePack;
using MessagePack.Resolvers;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Formatters.ThinktectureMessagePackFormatterTests;

/// <summary>
/// Regression tests for issue #19: verify ComplexValueObject with nullable keyed value object
/// properties serializes and deserializes correctly with MessagePack.
/// </summary>
public class NullableKeyedPropertyTests
{
   private readonly MessagePackSerializerOptions _options;

   public NullableKeyedPropertyTests()
   {
      var resolver = CompositeResolver.Create(ThinktectureMessageFormatterResolver.Instance, StandardResolver.Instance);
      _options = MessagePackSerializerOptions.Standard.WithResolver(resolver);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_null_keyed_value_object_properties()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(null, null, null, 42);

      var bytes = MessagePackSerializer.Serialize(obj, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableKeyedProperties>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_non_null_keyed_value_object_properties()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(
         StringBasedReferenceValueObject.Create("test"),
         IntBasedReferenceValueObject.Create(1),
         IntBasedStructValueObject.Create(2),
         42);

      var bytes = MessagePackSerializer.Serialize(obj, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableKeyedProperties>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_roundtrip_complex_value_object_with_partial_null_keyed_properties()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(
         StringBasedReferenceValueObject.Create("test"),
         null,
         null,
         42);

      var bytes = MessagePackSerializer.Serialize(obj, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableKeyedProperties>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.Should().Be(obj);
   }

   [Fact]
   public void Should_preserve_null_keyed_property_values_after_roundtrip()
   {
      var obj = ComplexValueObjectWithNullableKeyedProperties.Create(null, null, null, 42);

      var bytes = MessagePackSerializer.Serialize(obj, _options, TestContext.Current.CancellationToken);
      var deserialized = MessagePackSerializer.Deserialize<ComplexValueObjectWithNullableKeyedProperties>(bytes, _options, TestContext.Current.CancellationToken);

      deserialized.NullableStringBasedValueObject.Should().BeNull();
      deserialized.NullableIntBasedValueObject.Should().BeNull();
      deserialized.NullableIntBasedStructValueObject.Should().BeNull();
      deserialized.OtherProperty.Should().Be(42);
   }
}
