using Thinktecture.Runtime.Tests.TestValueObjects;

#nullable enable

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class ConversionFromKey
{
   [Fact]
   public void Should_return_value_object_for_key()
   {
      var value = (IntBasedReferenceValueObject)42;
      value.Should().Be(IntBasedReferenceValueObject.Create(42));
   }

   [Fact]
   public void Should_return_nullable_value_object_for_nullable_struct_key()
   {
      int? key = 42;
      var value = (IntBasedReferenceValueObject?)key;
      value.Should().NotBeNull();
      value.Should().Be(IntBasedReferenceValueObject.Create(42));
   }

   [Fact]
   public void Should_return_null_for_nullable_value_object_for_null_nullable_struct_key()
   {
      // ReSharper disable ExpressionIsAlwaysNull

      int? key = null;
      var value = (IntBasedReferenceValueObject?)key;
      value.Should().BeNull();
   }

   [Fact]
   public void Should_return_nullable_struct_value_object_for_nullable_struct_key()
   {
      int? key = 42;
      var value = (IntBasedStructValueObject?)key;
      value.Should().NotBeNull();
      value.Value.Should().Be(IntBasedStructValueObject.Create(42));
   }

   [Fact]
   public void Should_return_null_for_nullable_struct_value_object_for_null_nullable_struct_key()
   {
      // ReSharper disable ExpressionIsAlwaysNull

      int? key = null;
      var value = (IntBasedStructValueObject?)key;
      value.Should().BeNull();
   }
}
