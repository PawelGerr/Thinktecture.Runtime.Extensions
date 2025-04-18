using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ThinktectureTypeConverterTests;

public class CanConvertFrom : TypeConverterTestsBase
{
   [Fact]
   public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_struct_key()
   {
      IntBasedReferenceValueObjectTypeConverter.CanConvertFrom(typeof(int)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_struct_key()
   {
      IntBasedStructValueObjectTypeConverter.CanConvertFrom(typeof(int)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_source_type_is_value_type_itself()
   {
      IntBasedStructValueObjectTypeConverter.CanConvertFrom(typeof(IntBasedStructValueObject)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_source_type_is_nullable_value_type_of_itself()
   {
      IntBasedStructValueObjectTypeConverter.CanConvertFrom(typeof(IntBasedStructValueObject?)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_nullable_struct_key()
   {
      IntBasedReferenceValueObjectTypeConverter.CanConvertFrom(typeof(int?)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_nullable_struct_key()
   {
      IntBasedStructValueObjectTypeConverter.CanConvertFrom(typeof(int?)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_reference_type_and_key_type_matches_the_reference_type_key()
   {
      StringBasedReferenceValueObjectTypeConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_if_value_type_is_struct_and_key_type_matches_the_reference_type_key()
   {
      StringBasedStructValueObjectTypeConverter.CanConvertFrom(typeof(string)).Should().BeTrue();
   }
}
