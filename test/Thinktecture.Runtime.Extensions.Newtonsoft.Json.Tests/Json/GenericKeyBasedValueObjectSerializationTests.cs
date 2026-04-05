using Newtonsoft.Json;
using Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable InconsistentNaming

namespace Thinktecture.Runtime.Tests.Json;

public class GenericKeyBasedValueObjectSerializationTests
{
   public class ReferenceValueObject_Unconstraint
   {
      [Fact]
      public void Should_roundtrip_int_based()
      {
         var original = GenericKeyBasedReferenceValueObjectUnconstraint<int>.Create(42);
         var json = JsonConvert.SerializeObject(original);

         json.Should().Be("42");

         var deserialized = JsonConvert.DeserializeObject<GenericKeyBasedReferenceValueObjectUnconstraint<int>>(json);
         deserialized.Should().Be(original);
      }
   }

   public class ReferenceValueObject_ClassConstraint
   {
      [Fact]
      public void Should_roundtrip_string_based()
      {
         var original = GenericKeyBasedReferenceValueObjectClassConstraint<string>.Create("test");
         var json = JsonConvert.SerializeObject(original);

         json.Should().Be("\"test\"");

         var deserialized = JsonConvert.DeserializeObject<GenericKeyBasedReferenceValueObjectClassConstraint<string>>(json);
         deserialized.Should().Be(original);
      }
   }

   public class ReferenceValueObject_StructConstraint
   {
      [Fact]
      public void Should_roundtrip_int_based()
      {
         var original = GenericKeyBasedReferenceValueObjectStructConstraint<int>.Create(42);
         var json = JsonConvert.SerializeObject(original);

         json.Should().Be("42");

         var deserialized = JsonConvert.DeserializeObject<GenericKeyBasedReferenceValueObjectStructConstraint<int>>(json);
         deserialized.Should().Be(original);
      }
   }

   public class StructValueObject_Unconstraint
   {
      [Fact]
      public void Should_roundtrip_int_based()
      {
         var original = GenericKeyBasedStructValueObject<int>.Create(42);
         var json = JsonConvert.SerializeObject(original);

         json.Should().Be("42");

         var deserialized = JsonConvert.DeserializeObject<GenericKeyBasedStructValueObject<int>>(json);
         deserialized.Should().Be(original);
      }
   }

   public class StructValueObject_ClassConstraint
   {
      [Fact]
      public void Should_roundtrip_string_based()
      {
         var original = GenericKeyBasedStructValueObjectClassConstraint<string>.Create("test");
         var json = JsonConvert.SerializeObject(original);

         json.Should().Be("\"test\"");

         var deserialized = JsonConvert.DeserializeObject<GenericKeyBasedStructValueObjectClassConstraint<string>>(json);
         deserialized.Should().Be(original);
      }
   }

   public class StructValueObject_StructConstraint
   {
      [Fact]
      public void Should_roundtrip_int_based()
      {
         var original = GenericKeyBasedStructValueObjectStructConstraint<int>.Create(42);
         var json = JsonConvert.SerializeObject(original);

         json.Should().Be("42");

         var deserialized = JsonConvert.DeserializeObject<GenericKeyBasedStructValueObjectStructConstraint<int>>(json);
         deserialized.Should().Be(original);
      }
   }
}
