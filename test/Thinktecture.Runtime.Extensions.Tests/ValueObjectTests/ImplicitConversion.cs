using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class ImplicitConversion
{
   [Fact]
   public void Should_return_nullable_key_if_value_object_is_reference_type_and_key_is_struct()
   {
      int? value = IntBasedReferenceValueObject.Create(42);
      value.Should().Be(42);
   }

   [Fact]
   public void Should_return_null_if_value_object_is_reference_type_and_null_and_key_is_struct()
   {
      IntBasedReferenceValueObject obj = null;
      int? value = obj;

      value.Should().BeNull();
   }

   [Fact]
   public void Should_return_key_if_value_object_is_reference_type_and_key_is_reference_type()
   {
      string value = StringBasedReferenceValueObject.Create("value");
      value.Should().Be("value");
   }

   [Fact]
   public void Should_return_null_if_value_object_is_reference_type_and_null_and_key_is_reference_type()
   {
      StringBasedReferenceValueObject obj = null;
      string value = obj;
      value.Should().BeNull();
   }

   [Fact]
   public void Should_return_key_if_value_object_is_struct_and_key_is_struct()
   {
      int value = IntBasedStructValueObject.Create(42);
      value.Should().Be(42);
   }

   [Fact]
   public void Should_return_nullable_key_if_value_object_is_nullable_struct_and_key_is_struct()
   {
      int? value = (IntBasedStructValueObject?)IntBasedStructValueObject.Create(42);
      value.Should().Be(42);
   }

   [Fact]
   public void Should_return_null_if_value_object_is_nullable_struct_and_null_and_key_is_struct()
   {
      int? value = (IntBasedStructValueObject?)null;
      value.Should().BeNull();
   }
}
