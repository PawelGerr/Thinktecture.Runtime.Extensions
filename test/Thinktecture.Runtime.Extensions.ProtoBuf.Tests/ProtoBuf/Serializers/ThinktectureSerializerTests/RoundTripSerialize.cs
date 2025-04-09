using System.IO;
using ProtoBuf;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ProtoBuf.Serializers.ThinktectureSerializerTests;

public class RoundTripSerialize
{
   [Fact]
   public void Should_round_trip_serialize_simple_value_object()
   {
      RoundTrip(IntBasedReferenceValueObject.Create(42));
      RoundTrip(IntBasedStructValueObject.Create(42));
      RoundTrip(StringBasedReferenceValueObject.Create("Test"));
      RoundTrip(StringBasedStructValueObject.Create("Test"));
   }

   [Fact]
   public void Should_round_trip_serialize_smart_enums()
   {
      RoundTrip(SmartEnum_IntBased.Item1);
      RoundTrip(SmartEnum_StringBased.Item1);
   }

   [Fact]
   public void Should_round_trip_complex_value_objects()
   {
      RoundTrip(ComplexValueObjectWithReservedIdentifiers.Create(1, 2));
      RoundTrip(ComplexValueObjectWithString.Create("Test"));
      RoundTrip(ValueObjectWithInitProperties.Create(1, 2, 4, 5));
      RoundTrip(Boundary.Create(1, 2));
      RoundTrip(TestValueObject_Complex_Class.Create("Test 1", "Test 2"));
      RoundTrip(TestValueObject_Complex_Struct.Create("Test 1", "Test 2"));
      RoundTrip(StructValueObjectWithoutMembers.Create());
      RoundTrip(NestedComplexValueObject.Create(
                   ChildComplexValueObject.Create(
                      (IntBasedStructValueObject)1,
                      (StringBasedReferenceValueObject)"Test 1"!),
                   (IntBasedStructValueObject)2,
                   (StringBasedReferenceValueObject)"Test 2"));
   }

   private static void RoundTrip<T>(T value)
   {
      var memoryStream = new MemoryStream();
      Serializer.Serialize(memoryStream, value);

      memoryStream.Position = 0;
      var deserializedValue = Serializer.Deserialize<T>(memoryStream);

      deserializedValue.Should().Be(value);
   }

   private static void RoundTrip<TIn, TOut>(TIn serialize, TOut expected)
   {
      var memoryStream = new MemoryStream();
      Serializer.Serialize(memoryStream, serialize);

      memoryStream.Position = 0;
      var deserializedValue = Serializer.Deserialize<TOut>(memoryStream);

      deserializedValue.Should().Be(expected);
   }
}
