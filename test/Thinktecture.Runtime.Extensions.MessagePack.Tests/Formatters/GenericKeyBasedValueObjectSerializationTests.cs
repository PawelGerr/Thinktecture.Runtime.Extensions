using MessagePack;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.Formatters;

public class GenericKeyBasedValueObjectSerializationTests
{
   public class ReferenceValueObjectUnconstraint
   {
      [Fact]
      public void Should_roundtrip_with_int_key()
      {
         var original = GenericKeyBasedReferenceValueObjectUnconstraint<int>.Create(42);
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedReferenceValueObjectUnconstraint<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().Be(original);
      }

      [Fact]
      public void Should_serialize_to_key_value()
      {
         var original = GenericKeyBasedReferenceValueObjectUnconstraint<int>.Create(42);
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var key = MessagePackSerializer.Deserialize<int>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         key.Should().Be(42);
      }

      [Fact]
      public void Should_deserialize_from_key_value()
      {
         var bytes = MessagePackSerializer.Serialize(42, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedReferenceValueObjectUnconstraint<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().NotBeNull();
         deserialized.Should().Be(GenericKeyBasedReferenceValueObjectUnconstraint<int>.Create(42));
      }
   }

   public class ReferenceValueObjectClassConstraint
   {
      [Fact]
      public void Should_roundtrip_with_string_key()
      {
         var original = GenericKeyBasedReferenceValueObjectClassConstraint<string>.Create("test");
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedReferenceValueObjectClassConstraint<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().Be(original);
      }

      [Fact]
      public void Should_serialize_to_key_value()
      {
         var original = GenericKeyBasedReferenceValueObjectClassConstraint<string>.Create("test");
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var key = MessagePackSerializer.Deserialize<string>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         key.Should().Be("test");
      }

      [Fact]
      public void Should_deserialize_from_key_value()
      {
         var bytes = MessagePackSerializer.Serialize("test", cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedReferenceValueObjectClassConstraint<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().NotBeNull();
         deserialized.Should().Be(GenericKeyBasedReferenceValueObjectClassConstraint<string>.Create("test"));
      }
   }

   public class ReferenceValueObjectStructConstraint
   {
      [Fact]
      public void Should_roundtrip_with_int_key()
      {
         var original = GenericKeyBasedReferenceValueObjectStructConstraint<int>.Create(42);
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedReferenceValueObjectStructConstraint<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().Be(original);
      }

      [Fact]
      public void Should_serialize_to_key_value()
      {
         var original = GenericKeyBasedReferenceValueObjectStructConstraint<int>.Create(42);
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var key = MessagePackSerializer.Deserialize<int>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         key.Should().Be(42);
      }

      [Fact]
      public void Should_deserialize_from_key_value()
      {
         var bytes = MessagePackSerializer.Serialize(42, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedReferenceValueObjectStructConstraint<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().NotBeNull();
         deserialized.Should().Be(GenericKeyBasedReferenceValueObjectStructConstraint<int>.Create(42));
      }
   }

   public class StructValueObject
   {
      [Fact]
      public void Should_roundtrip_with_int_key()
      {
         var original = GenericKeyBasedStructValueObject<int>.Create(42);
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedStructValueObject<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().Be(original);
      }

      [Fact]
      public void Should_serialize_to_key_value()
      {
         var original = GenericKeyBasedStructValueObject<int>.Create(42);
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var key = MessagePackSerializer.Deserialize<int>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         key.Should().Be(42);
      }

      [Fact]
      public void Should_deserialize_from_key_value()
      {
         var bytes = MessagePackSerializer.Serialize(42, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedStructValueObject<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().Be(GenericKeyBasedStructValueObject<int>.Create(42));
      }
   }

   public class StructValueObjectClassConstraint
   {
      [Fact]
      public void Should_roundtrip_with_string_key()
      {
         var original = GenericKeyBasedStructValueObjectClassConstraint<string>.Create("test");
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedStructValueObjectClassConstraint<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().Be(original);
      }

      [Fact]
      public void Should_serialize_to_key_value()
      {
         var original = GenericKeyBasedStructValueObjectClassConstraint<string>.Create("test");
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var key = MessagePackSerializer.Deserialize<string>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         key.Should().Be("test");
      }

      [Fact]
      public void Should_deserialize_from_key_value()
      {
         var bytes = MessagePackSerializer.Serialize("test", cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedStructValueObjectClassConstraint<string>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().Be(GenericKeyBasedStructValueObjectClassConstraint<string>.Create("test"));
      }
   }

   public class StructValueObjectStructConstraint
   {
      [Fact]
      public void Should_roundtrip_with_int_key()
      {
         var original = GenericKeyBasedStructValueObjectStructConstraint<int>.Create(42);
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedStructValueObjectStructConstraint<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().Be(original);
      }

      [Fact]
      public void Should_serialize_to_key_value()
      {
         var original = GenericKeyBasedStructValueObjectStructConstraint<int>.Create(42);
         var bytes = MessagePackSerializer.Serialize(original, cancellationToken: TestContext.Current.CancellationToken);
         var key = MessagePackSerializer.Deserialize<int>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         key.Should().Be(42);
      }

      [Fact]
      public void Should_deserialize_from_key_value()
      {
         var bytes = MessagePackSerializer.Serialize(42, cancellationToken: TestContext.Current.CancellationToken);
         var deserialized = MessagePackSerializer.Deserialize<GenericKeyBasedStructValueObjectStructConstraint<int>>(bytes, cancellationToken: TestContext.Current.CancellationToken);

         deserialized.Should().Be(GenericKeyBasedStructValueObjectStructConstraint<int>.Create(42));
      }
   }
}
