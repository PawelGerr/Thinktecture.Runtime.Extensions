using System.IO;
using ProtoBuf;
using Thinktecture.Runtime.Tests.TestEnums;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ProtoBuf.Serializers.ValueObjectSerializerTests;

public class RoundTrip
{
   [Fact]
   public void Should_round_trip_serialize_simple_value_object()
   {
      DoRoundTrip(IntBasedReferenceValueObject.Create(42));
      DoRoundTrip(IntBasedStructValueObject.Create(42));
      DoRoundTrip(StringBasedReferenceValueObject.Create("Test"));
      DoRoundTrip(StringBasedStructValueObject.Create("Test"));
   }

   [Fact]
   public void Should_round_trip_serialize_smart_enums()
   {
      DoRoundTrip(TestSmartEnum_Struct_IntBased.Value1);
      DoRoundTrip(TestSmartEnum_Struct_StringBased.Value1);
      DoRoundTrip(TestSmartEnum_Class_IntBased.Value1);
      DoRoundTrip(TestSmartEnum_Class_StringBased.Value1);
   }

   [Fact]
   public void Should_round_trip_complex_value_objects()
   {
      DoRoundTrip(ComplexValueObjectWithReservedIdentifiers.Create(1, 2));
      DoRoundTrip(ComplexValueObjectWithString.Create("Test"));
      DoRoundTrip(ValueObjectWithInitProperties.Create(1, 2, 4, 5));
      DoRoundTrip(Boundary.Create(1, 2));
      DoRoundTrip(TestValueObject_Complex_Class.Create("Test 1", "Test 2"));;
      DoRoundTrip(TestValueObject_Complex_Struct.Create("Test 1", "Test 2"));;
      DoRoundTrip(StructValueObjectWithoutMembers.Create());
      DoRoundTrip(NestedComplexValueObject.Create(
                     ChildComplexValueObject.Create(
                        (IntBasedStructValueObject)1,
                        (StringBasedReferenceValueObject)"Test 1"),
                     (IntBasedStructValueObject)2,
                     (StringBasedReferenceValueObject)"Test 2"));
   }

   private static void DoRoundTrip<T>(T value)
   {
      var memoryStream = new MemoryStream();
      Serializer.Serialize(memoryStream, value);

      memoryStream.Position = 0;
      var deserializedValue = Serializer.Deserialize<T>(memoryStream);

      deserializedValue.Should().Be(value);
   }
}
