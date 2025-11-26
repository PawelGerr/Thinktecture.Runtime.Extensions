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
            var bytes = MessagePackSerializer.Serialize(obj, cancellationToken: TestContext.Current.CancellationToken);
            var expected = MessagePackSerializer.Serialize(value, cancellationToken: TestContext.Current.CancellationToken);

            bytes.Should().Equal(expected);
         }

         [Fact]
         public void Should_serialize_with_different_type_arguments()
         {
            var stringInstance = ValueObject_Generic_IntBased<string>.Create(42);
            var intInstance = ValueObject_Generic_IntBased<int>.Create(42);

            var stringBytes = MessagePackSerializer.Serialize(stringInstance, cancellationToken: TestContext.Current.CancellationToken);
            var intBytes = MessagePackSerializer.Serialize(intInstance, cancellationToken: TestContext.Current.CancellationToken);
            var expected = MessagePackSerializer.Serialize(42, cancellationToken: TestContext.Current.CancellationToken);

            stringBytes.Should().Equal(expected);
            intBytes.Should().Equal(expected);
         }

         [Fact]
         public void Should_serialize_struct_instance()
         {
            var obj = StructValueObject_Generic_IntBased<string>.Create(42);
            var bytes = MessagePackSerializer.Serialize(obj, cancellationToken: TestContext.Current.CancellationToken);
            var expected = MessagePackSerializer.Serialize(42, cancellationToken: TestContext.Current.CancellationToken);

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
            var bytes = MessagePackSerializer.Serialize(value, cancellationToken: TestContext.Current.CancellationToken);
            var obj = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

            obj.Should().NotBeNull();
            obj.Value.Should().Be(value);
         }

         [Fact]
         public void Should_deserialize_with_different_type_arguments()
         {
            var bytes = MessagePackSerializer.Serialize(42, cancellationToken: TestContext.Current.CancellationToken);

            var stringInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);
            var intInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

            stringInstance.Should().NotBeNull();
            intInstance.Should().NotBeNull();
            stringInstance.Value.Should().Be(42);
            intInstance.Value.Should().Be(42);
         }

         [Fact]
         public void Should_deserialize_struct_instance()
         {
            var bytes = MessagePackSerializer.Serialize(42, cancellationToken: TestContext.Current.CancellationToken);
            var obj = MessagePackSerializer.Deserialize<StructValueObject_Generic_IntBased<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

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
            var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
            var deserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

            deserialized.Should().Be(original);
         }

         [Fact]
         public void Should_round_trip_with_different_type_arguments()
         {
            var stringOriginal = ValueObject_Generic_IntBased<string>.Create(42);
            var intOriginal = ValueObject_Generic_IntBased<int>.Create(42);

            var stringBytes = MessagePackSerializer.Serialize(stringOriginal, cancellationToken: TestContext.Current.CancellationToken);
            var intBytes = MessagePackSerializer.Serialize(intOriginal, cancellationToken: TestContext.Current.CancellationToken);

            var stringDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(stringBytes, cancellationToken: TestContext.Current.CancellationToken);
            var intDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_IntBased<int>>(intBytes, cancellationToken: TestContext.Current.CancellationToken);

            stringDeserialized.Should().Be(stringOriginal);
            intDeserialized.Should().Be(intOriginal);
         }

         [Fact]
         public void Should_round_trip_struct_successfully()
         {
            var original = StructValueObject_Generic_IntBased<string>.Create(42);
            var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
            var deserialized = MessagePackSerializer.Deserialize<StructValueObject_Generic_IntBased<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

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
            var bytes = MessagePackSerializer.Serialize(obj, cancellationToken: TestContext.Current.CancellationToken);
            var expected = MessagePackSerializer.Serialize(value, cancellationToken: TestContext.Current.CancellationToken);

            bytes.Should().Equal(expected);
         }

         [Fact]
         public void Should_serialize_with_different_type_arguments()
         {
            var stringInstance = ValueObject_Generic_StringBased<string>.Create("test");
            var objectInstance = ValueObject_Generic_StringBased<object>.Create("test");

            var stringBytes = MessagePackSerializer.Serialize(stringInstance, cancellationToken: TestContext.Current.CancellationToken);
            var objectBytes = MessagePackSerializer.Serialize(objectInstance, cancellationToken: TestContext.Current.CancellationToken);
            var expected = MessagePackSerializer.Serialize("test", cancellationToken: TestContext.Current.CancellationToken);

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
            var bytes = MessagePackSerializer.Serialize(value, cancellationToken: TestContext.Current.CancellationToken);
            var obj = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

            obj.Should().NotBeNull();
            obj!.Value.Should().Be(value);
         }

         [Fact]
         public void Should_deserialize_with_different_type_arguments()
         {
            var bytes = MessagePackSerializer.Serialize("test", cancellationToken: TestContext.Current.CancellationToken);

            var stringInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);
            var objectInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

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
            var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
            var deserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

            deserialized.Should().Be(original);
         }

         [Fact]
         public void Should_round_trip_with_different_type_arguments()
         {
            var stringOriginal = ValueObject_Generic_StringBased<string>.Create("test");
            var objectOriginal = ValueObject_Generic_StringBased<object>.Create("test");

            var stringBytes = MessagePackSerializer.Serialize(stringOriginal, cancellationToken: TestContext.Current.CancellationToken);
            var objectBytes = MessagePackSerializer.Serialize(objectOriginal, cancellationToken: TestContext.Current.CancellationToken);

            var stringDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<string>>(stringBytes, cancellationToken: TestContext.Current.CancellationToken);
            var objectDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(objectBytes, cancellationToken: TestContext.Current.CancellationToken);

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
            var bytes = MessagePackSerializer.Serialize(obj, cancellationToken: TestContext.Current.CancellationToken);
            var expected = MessagePackSerializer.Serialize(guid, cancellationToken: TestContext.Current.CancellationToken);

            bytes.Should().Equal(expected);
         }

         [Fact]
         public void Should_serialize_with_different_type_arguments()
         {
            var guid = Guid.NewGuid();
            var stringInstance = ValueObject_Generic_GuidBased<string>.Create(guid);
            var intInstance = ValueObject_Generic_GuidBased<int>.Create(guid);

            var stringBytes = MessagePackSerializer.Serialize(stringInstance, cancellationToken: TestContext.Current.CancellationToken);
            var intBytes = MessagePackSerializer.Serialize(intInstance, cancellationToken: TestContext.Current.CancellationToken);
            var expected = MessagePackSerializer.Serialize(guid, cancellationToken: TestContext.Current.CancellationToken);

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
            var bytes = MessagePackSerializer.Serialize(guid, cancellationToken: TestContext.Current.CancellationToken);
            var obj = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

            obj.Should().NotBeNull();
            obj.Value.Should().Be(guid);
         }

         [Fact]
         public void Should_deserialize_with_different_type_arguments()
         {
            var guid = Guid.NewGuid();
            var bytes = MessagePackSerializer.Serialize(guid, cancellationToken: TestContext.Current.CancellationToken);

            var stringInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);
            var intInstance = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

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
            var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
            var deserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

            deserialized.Should().Be(original);
         }

         [Fact]
         public void Should_round_trip_with_different_type_arguments()
         {
            var guid = Guid.NewGuid();
            var stringOriginal = ValueObject_Generic_GuidBased<string>.Create(guid);
            var intOriginal = ValueObject_Generic_GuidBased<int>.Create(guid);

            var stringBytes = MessagePackSerializer.Serialize(stringOriginal, cancellationToken: TestContext.Current.CancellationToken);
            var intBytes = MessagePackSerializer.Serialize(intOriginal, cancellationToken: TestContext.Current.CancellationToken);

            var stringDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(stringBytes, cancellationToken: TestContext.Current.CancellationToken);
            var intDeserialized = MessagePackSerializer.Deserialize<ValueObject_Generic_GuidBased<int>>(intBytes, cancellationToken: TestContext.Current.CancellationToken);

            stringDeserialized.Should().Be(stringOriginal);
            intDeserialized.Should().Be(intOriginal);
         }
      }
   }
}
