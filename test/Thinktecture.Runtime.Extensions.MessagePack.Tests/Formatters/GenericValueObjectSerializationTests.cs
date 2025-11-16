using System;
using MessagePack;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Formatters;

public class GenericValueObjectSerializationTests
{
   public class IntBased
   {
      public class Serialization
      {
         [Theory]
         [InlineData(1)]
         [InlineData(42)]
         [InlineData(int.MaxValue)]
         [InlineData(int.MinValue)]
         [InlineData(0)]
         public void Should_serialize_to_int_value(int value)
         {
            var obj = ValueObject_Generic_IntBased<string>.Create(value);
            var bytes = MessagePackSerializer.Serialize(obj);
            var expected = MessagePackSerializer.Serialize(value);

            bytes.Should().Equal(expected);
         }

         [Fact]
         public void Should_serialize_with_different_type_arguments()
         {
            var stringInstance = ValueObject_Generic_IntBased<string>.Create(42);
            var intInstance = ValueObject_Generic_IntBased<int>.Create(42);

            var stringBytes = MessagePackSerializer.Serialize(stringInstance);
            var intBytes = MessagePackSerializer.Serialize(intInstance);
            var expected = MessagePackSerializer.Serialize(42);

            stringBytes.Should().Equal(expected);
            intBytes.Should().Equal(expected);
         }

         [Fact]
         public void Should_serialize_struct_instance()
         {
            var obj = StructValueObject_Generic_IntBased<string>.Create(42);
            var bytes = MessagePackSerializer.Serialize(obj);
            var expected = MessagePackSerializer.Serialize(42);

            bytes.Should().Equal(expected);
         }
      }

      public class Deserialization
      {
         [Theory]
         [InlineData(1)]
         [InlineData(42)]
         [InlineData(int.MaxValue)]
         [InlineData(int.MinValue)]
         [InlineData(0)]
         public void Should_deserialize_from_int_value(int value)
         {
            var bytes = MessagePackSerializer.Serialize(value);
            var obj = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(bytes);

            obj.Should().NotBeNull();
            obj.Value.Should().Be(value);
         }

         [Fact]
         public void Should_deserialize_with_different_type_arguments()
         {
            var bytes = MessagePackSerializer.Serialize(42);

            var stringInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(bytes);
            var intInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<int>>(bytes);

            stringInstance.Should().NotBeNull();
            intInstance.Should().NotBeNull();
            stringInstance.Value.Should().Be(42);
            intInstance.Value.Should().Be(42);
         }

         [Fact]
         public void Should_deserialize_struct_instance()
         {
            var bytes = MessagePackSerializer.Serialize(42);
            var obj = MessagePackSerializer.Deserialize<StructValueObject_Generic_IntBased<string>>(bytes);

            obj.Value.Should().Be(42);
         }
      }

      public class RoundTrip
      {
         [Theory]
         [InlineData(1)]
         [InlineData(42)]
         [InlineData(int.MaxValue)]
         [InlineData(int.MinValue)]
         public void Should_round_trip_successfully(int value)
         {
            var original = ValueObject_Generic_IntBased<string>.Create(value);
            var bytes = MessagePackSerializer.Serialize(original);
            var deserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(bytes);

            deserialized.Should().Be(original);
         }

         [Fact]
         public void Should_round_trip_with_different_type_arguments()
         {
            var stringOriginal = ValueObject_Generic_IntBased<string>.Create(42);
            var intOriginal = ValueObject_Generic_IntBased<int>.Create(42);

            var stringBytes = MessagePackSerializer.Serialize(stringOriginal);
            var intBytes = MessagePackSerializer.Serialize(intOriginal);

            var stringDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(stringBytes);
            var intDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<int>>(intBytes);

            stringDeserialized.Should().Be(stringOriginal);
            intDeserialized.Should().Be(intOriginal);
         }

         [Fact]
         public void Should_round_trip_struct_successfully()
         {
            var original = StructValueObject_Generic_IntBased<string>.Create(42);
            var bytes = MessagePackSerializer.Serialize(original);
            var deserialized = MessagePackSerializer.Deserialize<StructValueObject_Generic_IntBased<string>>(bytes);

            deserialized.Should().Be(original);
         }
      }
   }

   public class StringBased
   {
      public class Serialization
      {
         [Theory]
         [InlineData("test")]
         [InlineData("")]
         [InlineData("value with spaces")]
         public void Should_serialize_to_string_value(string value)
         {
            var obj = ValueObject_Generic_StringBased<object>.Create(value);
            var bytes = MessagePackSerializer.Serialize(obj);
            var expected = MessagePackSerializer.Serialize(value);

            bytes.Should().Equal(expected);
         }

         [Fact]
         public void Should_serialize_with_different_type_arguments()
         {
            var stringInstance = ValueObject_Generic_StringBased<string>.Create("test");
            var objectInstance = ValueObject_Generic_StringBased<object>.Create("test");

            var stringBytes = MessagePackSerializer.Serialize(stringInstance);
            var objectBytes = MessagePackSerializer.Serialize(objectInstance);
            var expected = MessagePackSerializer.Serialize("test");

            stringBytes.Should().Equal(expected);
            objectBytes.Should().Equal(expected);
         }
      }

      public class Deserialization
      {
         [Theory]
         [InlineData("test")]
         [InlineData("")]
         [InlineData("value with spaces")]
         public void Should_deserialize_from_string_value(string value)
         {
            var bytes = MessagePackSerializer.Serialize(value);
            var obj = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(bytes);

            obj.Should().NotBeNull();
            obj!.Value.Should().Be(value);
         }

         [Fact]
         public void Should_deserialize_with_different_type_arguments()
         {
            var bytes = MessagePackSerializer.Serialize("test");

            var stringInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<string>>(bytes);
            var objectInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(bytes);

            stringInstance.Should().NotBeNull();
            objectInstance.Should().NotBeNull();
            stringInstance.Value.Should().Be("test");
            objectInstance.Value.Should().Be("test");
         }
      }

      public class RoundTrip
      {
         [Theory]
         [InlineData("test")]
         [InlineData("")]
         [InlineData("value with spaces")]
         public void Should_round_trip_successfully(string value)
         {
            var original = ValueObject_Generic_StringBased<object>.Create(value);
            var bytes = MessagePackSerializer.Serialize(original);
            var deserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(bytes);

            deserialized.Should().Be(original);
         }

         [Fact]
         public void Should_round_trip_with_different_type_arguments()
         {
            var stringOriginal = ValueObject_Generic_StringBased<string>.Create("test");
            var objectOriginal = ValueObject_Generic_StringBased<object>.Create("test");

            var stringBytes = MessagePackSerializer.Serialize(stringOriginal);
            var objectBytes = MessagePackSerializer.Serialize(objectOriginal);

            var stringDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<string>>(stringBytes);
            var objectDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(objectBytes);

            stringDeserialized.Should().Be(stringOriginal);
            objectDeserialized.Should().Be(objectOriginal);
         }
      }
   }

   public class GuidBased
   {
      public class Serialization
      {
         [Fact]
         public void Should_serialize_to_guid_value()
         {
            var guid = Guid.NewGuid();
            var obj = ValueObject_Generic_GuidBased<string>.Create(guid);
            var bytes = MessagePackSerializer.Serialize(obj);
            var expected = MessagePackSerializer.Serialize(guid);

            bytes.Should().Equal(expected);
         }

         [Fact]
         public void Should_serialize_with_different_type_arguments()
         {
            var guid = Guid.NewGuid();
            var stringInstance = ValueObject_Generic_GuidBased<string>.Create(guid);
            var intInstance = ValueObject_Generic_GuidBased<int>.Create(guid);

            var stringBytes = MessagePackSerializer.Serialize(stringInstance);
            var intBytes = MessagePackSerializer.Serialize(intInstance);
            var expected = MessagePackSerializer.Serialize(guid);

            stringBytes.Should().Equal(expected);
            intBytes.Should().Equal(expected);
         }
      }

      public class Deserialization
      {
         [Fact]
         public void Should_deserialize_from_guid_value()
         {
            var guid = Guid.NewGuid();
            var bytes = MessagePackSerializer.Serialize(guid);
            var obj = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(bytes);

            obj.Should().NotBeNull();
            obj.Value.Should().Be(guid);
         }

         [Fact]
         public void Should_deserialize_with_different_type_arguments()
         {
            var guid = Guid.NewGuid();
            var bytes = MessagePackSerializer.Serialize(guid);

            var stringInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(bytes);
            var intInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<int>>(bytes);

            stringInstance.Should().NotBeNull();
            intInstance.Should().NotBeNull();
            stringInstance.Value.Should().Be(guid);
            intInstance.Value.Should().Be(guid);
         }
      }

      public class RoundTrip
      {
         [Fact]
         public void Should_round_trip_successfully()
         {
            var guid = Guid.NewGuid();
            var original = ValueObject_Generic_GuidBased<string>.Create(guid);
            var bytes = MessagePackSerializer.Serialize(original);
            var deserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(bytes);

            deserialized.Should().Be(original);
         }

         [Fact]
         public void Should_round_trip_with_different_type_arguments()
         {
            var guid = Guid.NewGuid();
            var stringOriginal = ValueObject_Generic_GuidBased<string>.Create(guid);
            var intOriginal = ValueObject_Generic_GuidBased<int>.Create(guid);

            var stringBytes = MessagePackSerializer.Serialize(stringOriginal);
            var intBytes = MessagePackSerializer.Serialize(intOriginal);

            var stringDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(stringBytes);
            var intDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<int>>(intBytes);

            stringDeserialized.Should().Be(stringOriginal);
            intDeserialized.Should().Be(intOriginal);
         }
      }
   }
}
