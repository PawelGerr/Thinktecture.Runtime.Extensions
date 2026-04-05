using System.Text.Json;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Text.Json.Serialization;

public class GenericKeyBasedValueObjectSerializationTests
{
   public class ReferenceValueObjectUnconstraint
   {
      public class Serialization
      {
         [Fact]
         public void Should_serialize_int_based_to_int_value()
         {
            var obj = GenericKeyBasedReferenceValueObjectUnconstraint<int>.Create(42);
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be("42");
         }
      }

      public class Deserialization
      {
         [Fact]
         public void Should_deserialize_int_based_from_int_value()
         {
            var expected = GenericKeyBasedReferenceValueObjectUnconstraint<int>.Create(42);
            var obj = JsonSerializer.Deserialize<GenericKeyBasedReferenceValueObjectUnconstraint<int>>("42");

            obj.Should().Be(expected);
         }
      }

      public class RoundTrip
      {
         [Fact]
         public void Should_round_trip_int_based_successfully()
         {
            var original = GenericKeyBasedReferenceValueObjectUnconstraint<int>.Create(42);
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<GenericKeyBasedReferenceValueObjectUnconstraint<int>>(json);

            deserialized.Should().Be(original);
         }
      }
   }

   public class ReferenceValueObjectClassConstraint
   {
      public class Serialization
      {
         [Fact]
         public void Should_serialize_string_based_to_string_value()
         {
            var obj = GenericKeyBasedReferenceValueObjectClassConstraint<string>.Create("test");
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be("\"test\"");
         }
      }

      public class Deserialization
      {
         [Fact]
         public void Should_deserialize_string_based_from_string_value()
         {
            var expected = GenericKeyBasedReferenceValueObjectClassConstraint<string>.Create("test");
            var obj = JsonSerializer.Deserialize<GenericKeyBasedReferenceValueObjectClassConstraint<string>>("\"test\"");

            obj.Should().Be(expected);
         }
      }

      public class RoundTrip
      {
         [Fact]
         public void Should_round_trip_string_based_successfully()
         {
            var original = GenericKeyBasedReferenceValueObjectClassConstraint<string>.Create("test");
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<GenericKeyBasedReferenceValueObjectClassConstraint<string>>(json);

            deserialized.Should().Be(original);
         }
      }
   }

   public class ReferenceValueObjectStructConstraint
   {
      public class Serialization
      {
         [Fact]
         public void Should_serialize_int_based_to_int_value()
         {
            var obj = GenericKeyBasedReferenceValueObjectStructConstraint<int>.Create(42);
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be("42");
         }
      }

      public class Deserialization
      {
         [Fact]
         public void Should_deserialize_int_based_from_int_value()
         {
            var expected = GenericKeyBasedReferenceValueObjectStructConstraint<int>.Create(42);
            var obj = JsonSerializer.Deserialize<GenericKeyBasedReferenceValueObjectStructConstraint<int>>("42");

            obj.Should().Be(expected);
         }
      }

      public class RoundTrip
      {
         [Fact]
         public void Should_round_trip_int_based_successfully()
         {
            var original = GenericKeyBasedReferenceValueObjectStructConstraint<int>.Create(42);
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<GenericKeyBasedReferenceValueObjectStructConstraint<int>>(json);

            deserialized.Should().Be(original);
         }
      }
   }

   public class StructValueObject
   {
      public class Serialization
      {
         [Fact]
         public void Should_serialize_int_based_to_int_value()
         {
            var obj = GenericKeyBasedStructValueObject<int>.Create(42);
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be("42");
         }
      }

      public class Deserialization
      {
         [Fact]
         public void Should_deserialize_int_based_from_int_value()
         {
            var expected = GenericKeyBasedStructValueObject<int>.Create(42);
            var obj = JsonSerializer.Deserialize<GenericKeyBasedStructValueObject<int>>("42");

            obj.Should().Be(expected);
         }
      }

      public class RoundTrip
      {
         [Fact]
         public void Should_round_trip_int_based_successfully()
         {
            var original = GenericKeyBasedStructValueObject<int>.Create(42);
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<GenericKeyBasedStructValueObject<int>>(json);

            deserialized.Should().Be(original);
         }
      }
   }

   public class StructValueObjectClassConstraint
   {
      public class Serialization
      {
         [Fact]
         public void Should_serialize_string_based_to_string_value()
         {
            var obj = GenericKeyBasedStructValueObjectClassConstraint<string>.Create("test");
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be("\"test\"");
         }
      }

      public class Deserialization
      {
         [Fact]
         public void Should_deserialize_string_based_from_string_value()
         {
            var expected = GenericKeyBasedStructValueObjectClassConstraint<string>.Create("test");
            var obj = JsonSerializer.Deserialize<GenericKeyBasedStructValueObjectClassConstraint<string>>("\"test\"");

            obj.Should().Be(expected);
         }
      }

      public class RoundTrip
      {
         [Fact]
         public void Should_round_trip_string_based_successfully()
         {
            var original = GenericKeyBasedStructValueObjectClassConstraint<string>.Create("test");
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<GenericKeyBasedStructValueObjectClassConstraint<string>>(json);

            deserialized.Should().Be(original);
         }
      }
   }

   public class StructValueObjectStructConstraint
   {
      public class Serialization
      {
         [Fact]
         public void Should_serialize_int_based_to_int_value()
         {
            var obj = GenericKeyBasedStructValueObjectStructConstraint<int>.Create(42);
            var json = JsonSerializer.Serialize(obj);

            json.Should().Be("42");
         }
      }

      public class Deserialization
      {
         [Fact]
         public void Should_deserialize_int_based_from_int_value()
         {
            var expected = GenericKeyBasedStructValueObjectStructConstraint<int>.Create(42);
            var obj = JsonSerializer.Deserialize<GenericKeyBasedStructValueObjectStructConstraint<int>>("42");

            obj.Should().Be(expected);
         }
      }

      public class RoundTrip
      {
         [Fact]
         public void Should_round_trip_int_based_successfully()
         {
            var original = GenericKeyBasedStructValueObjectStructConstraint<int>.Create(42);
            var json = JsonSerializer.Serialize(original);
            var deserialized = JsonSerializer.Deserialize<GenericKeyBasedStructValueObjectStructConstraint<int>>(json);

            deserialized.Should().Be(original);
         }
      }
   }
}
