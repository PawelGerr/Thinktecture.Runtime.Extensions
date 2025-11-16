using System;
using System.Text.Json;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization;

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
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be(value.ToString());
         }

         [Fact]
         public void Should_serialize_with_different_type_arguments()
         {
            var stringInstance = ValueObject_Generic_IntBased<string>.Create(42);
            var intInstance = ValueObject_Generic_IntBased<int>.Create(42);

            var stringJson = JsonSerializer.Serialize(stringInstance);
            var intJson = JsonSerializer.Serialize(intInstance);

            stringJson.Should().Be("42");
            intJson.Should().Be("42");
         }

         [Fact]
         public void Should_serialize_struct_instance()
         {
            var obj = StructValueObject_Generic_IntBased<string>.Create(42);
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be("42");
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
            var json = value.ToString();
            var obj = JsonSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(json);

            obj.Should().NotBeNull();
            obj!.Value.Should().Be(value);
         }

         [Fact]
         public void Should_deserialize_with_different_type_arguments()
         {
            var json = "42";

            var stringInstance = JsonSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(json);
            var intInstance = JsonSerializer.Deserialize<ValueObject_Generic_IntBased<int>>(json);

            stringInstance.Should().NotBeNull();
            intInstance.Should().NotBeNull();
            stringInstance!.Value.Should().Be(42);
            intInstance!.Value.Should().Be(42);
         }

         [Fact]
         public void Should_deserialize_struct_instance()
         {
            var json = "42";
            var obj = JsonSerializer.Deserialize<StructValueObject_Generic_IntBased<string>>(json);

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
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(json);

            deserialized.Should().Be(original);
         }

         [Fact]
         public void Should_round_trip_with_different_type_arguments()
         {
            var stringOriginal = ValueObject_Generic_IntBased<string>.Create(42);
            var intOriginal = ValueObject_Generic_IntBased<int>.Create(42);

            var stringJson = JsonSerializer.Serialize(stringOriginal);
            var intJson = JsonSerializer.Serialize(intOriginal);

            var stringDeserialized = JsonSerializer.Deserialize<ValueObject_Generic_IntBased<string>>(stringJson);
            var intDeserialized = JsonSerializer.Deserialize<ValueObject_Generic_IntBased<int>>(intJson);

            stringDeserialized.Should().Be(stringOriginal);
            intDeserialized.Should().Be(intOriginal);
         }

         [Fact]
         public void Should_round_trip_struct_successfully()
         {
            var original = StructValueObject_Generic_IntBased<string>.Create(42);
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<StructValueObject_Generic_IntBased<string>>(json);

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
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be(JsonSerializer.Serialize(value));
         }

         [Fact]
         public void Should_serialize_with_different_type_arguments()
         {
            var stringInstance = ValueObject_Generic_StringBased<string>.Create("test");
            var objectInstance = ValueObject_Generic_StringBased<object>.Create("test");

            var stringJson = JsonSerializer.Serialize(stringInstance);
            var objectJson = JsonSerializer.Serialize(objectInstance);

            stringJson.Should().Be("\"test\"");
            objectJson.Should().Be("\"test\"");
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
            var json = JsonSerializer.Serialize(value);
            var obj = JsonSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(json);

            obj.Should().NotBeNull();
            obj!.Value.Should().Be(value);
         }

         [Fact]
         public void Should_deserialize_with_different_type_arguments()
         {
            var json = "\"test\"";

            var stringInstance = JsonSerializer.Deserialize<ValueObject_Generic_StringBased<string>>(json);
            var objectInstance = JsonSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(json);

            stringInstance.Should().NotBeNull();
            objectInstance.Should().NotBeNull();
            stringInstance!.Value.Should().Be("test");
            objectInstance!.Value.Should().Be("test");
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
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(json);

            deserialized.Should().Be(original);
         }

         [Fact]
         public void Should_round_trip_with_different_type_arguments()
         {
            var stringOriginal = ValueObject_Generic_StringBased<string>.Create("test");
            var objectOriginal = ValueObject_Generic_StringBased<object>.Create("test");

            var stringJson = JsonSerializer.Serialize(stringOriginal);
            var objectJson = JsonSerializer.Serialize(objectOriginal);

            var stringDeserialized = JsonSerializer.Deserialize<ValueObject_Generic_StringBased<string>>(stringJson);
            var objectDeserialized = JsonSerializer.Deserialize<ValueObject_Generic_StringBased<object>>(objectJson);

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
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be(JsonSerializer.Serialize(guid));
         }

         [Fact]
         public void Should_serialize_with_different_type_arguments()
         {
            var guid = Guid.NewGuid();
            var stringInstance = ValueObject_Generic_GuidBased<string>.Create(guid);
            var intInstance = ValueObject_Generic_GuidBased<int>.Create(guid);

            var stringJson = JsonSerializer.Serialize(stringInstance);
            var intJson = JsonSerializer.Serialize(intInstance);

            var expectedJson = JsonSerializer.Serialize(guid);
            stringJson.Should().Be(expectedJson);
            intJson.Should().Be(expectedJson);
         }
      }

      public class Deserialization
      {
         [Fact]
         public void Should_deserialize_from_guid_value()
         {
            var guid = Guid.NewGuid();
            var json = JsonSerializer.Serialize(guid);
            var obj = JsonSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(json);

            obj.Should().NotBeNull();
            obj!.Value.Should().Be(guid);
         }

         [Fact]
         public void Should_deserialize_with_different_type_arguments()
         {
            var guid = Guid.NewGuid();
            var json = JsonSerializer.Serialize(guid);

            var stringInstance = JsonSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(json);
            var intInstance = JsonSerializer.Deserialize<ValueObject_Generic_GuidBased<int>>(json);

            stringInstance.Should().NotBeNull();
            intInstance.Should().NotBeNull();
            stringInstance!.Value.Should().Be(guid);
            intInstance!.Value.Should().Be(guid);
         }
      }

      public class RoundTrip
      {
         [Fact]
         public void Should_round_trip_successfully()
         {
            var guid = Guid.NewGuid();
            var original = ValueObject_Generic_GuidBased<string>.Create(guid);
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(json);

            deserialized.Should().Be(original);
         }

         [Fact]
         public void Should_round_trip_with_different_type_arguments()
         {
            var guid = Guid.NewGuid();
            var stringOriginal = ValueObject_Generic_GuidBased<string>.Create(guid);
            var intOriginal = ValueObject_Generic_GuidBased<int>.Create(guid);

            var stringJson = JsonSerializer.Serialize(stringOriginal);
            var intJson = JsonSerializer.Serialize(intOriginal);

            var stringDeserialized = JsonSerializer.Deserialize<ValueObject_Generic_GuidBased<string>>(stringJson);
            var intDeserialized = JsonSerializer.Deserialize<ValueObject_Generic_GuidBased<int>>(intJson);

            stringDeserialized.Should().Be(stringOriginal);
            intDeserialized.Should().Be(intOriginal);
         }
      }
   }
}
