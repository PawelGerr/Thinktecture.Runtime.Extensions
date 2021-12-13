using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTypeConverterTests;

public class CanConvertTo : TypeConverterTestsBase
{
   [Fact]
   public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_struct_key()
   {
      IntBasedReferenceValueObjectTypeConverter.CanConvertTo(typeof(int)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_struct_key()
   {
      IntBasedStructValueObjectTypeConverter.CanConvertTo(typeof(int)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_destination_type_is_value_type_itself()
   {
      IntBasedStructValueObjectTypeConverter.CanConvertTo(typeof(IntBasedStructValueObject)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_destination_type_is_nullable_value_type_of_itself()
   {
      IntBasedStructValueObjectTypeConverter.CanConvertTo(typeof(IntBasedStructValueObject?)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_nullable_struct_key()
   {
      IntBasedReferenceValueObjectTypeConverter.CanConvertTo(typeof(int?)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_nullable_struct_key()
   {
      IntBasedStructValueObjectTypeConverter.CanConvertTo(typeof(int?)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_reference_type_key()
   {
      StringBasedReferenceValueObjectTypeConverter.CanConvertTo(typeof(string)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_reference_type_key()
   {
      StringBasedStructValueObjectTypeConverter.CanConvertTo(typeof(string)).Should().BeTrue();
   }
}