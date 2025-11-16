using System;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class TryCreate
{
   [Fact]
   public void With_EmptyStringInFactoryMethodsYieldsNull_null_should_yield_null()
   {
      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.TryCreate(null, out var obj).Should().BeTrue();
      obj.Should().BeNull();

      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.TryCreate(null, out obj, out var error).Should().BeTrue();
      obj.Should().BeNull();
      error.Should().BeNull();
   }

   [Fact]
   public void With_EmptyStringInFactoryMethodsYieldsNull_empty_string_should_yield_null()
   {
      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.TryCreate(String.Empty, out var obj).Should().BeTrue();
      obj.Should().BeNull();

      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.TryCreate(String.Empty, out obj, out var error).Should().BeTrue();
      obj.Should().BeNull();
      error.Should().BeNull();
   }

   [Fact]
   public void With_EmptyStringInFactoryMethodsYieldsNull_whitespaces_should_yield_null()
   {
      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.TryCreate(" ", out var obj).Should().BeTrue();
      obj.Should().BeNull();

      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.TryCreate(" ", out obj, out var error).Should().BeTrue();
      obj.Should().BeNull();
      error.Should().BeNull();
   }

   [Fact]
   public void With_NullInFactoryMethodsYieldsNull_null_should_yield_null()
   {
      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.TryCreate(null, out var obj).Should().BeTrue();
      obj.Should().BeNull();

      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.TryCreate(null, out obj, out var error).Should().BeTrue();
      error.Should().BeNull();
   }

   [Fact]
   public void With_NullInFactoryMethodsYieldsNull_empty_string_should_yield_null()
   {
      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.TryCreate(String.Empty, out var obj).Should().BeFalse();
      obj.Should().BeNull();

      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.TryCreate(String.Empty, out obj, out var error).Should().BeFalse();
      obj.Should().BeNull();
      error.Should().NotBeNull();
   }

   [Fact]
   public void With_NullInFactoryMethodsYieldsNull_whitespaces_should_yield_null()
   {
      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.TryCreate(" ", out var obj).Should().BeFalse();
      obj.Should().BeNull();

      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.TryCreate(" ", out obj, out var error).Should().BeFalse();
      obj.Should().BeNull();
      error.Should().NotBeNull();
   }

   [Fact]
   public void With_custom_factory_name()
   {
      IntBasedReferenceValueObjectWithCustomFactoryNames.TryGet(1, out var obj).Should().BeTrue();
      obj.Should().BeEquivalentTo(new { Property = 1 });

      IntBasedReferenceValueObjectWithCustomFactoryNames.TryGet(1, out obj, out var error).Should().BeTrue();
      obj.Should().BeEquivalentTo(new { Property = 1 });
      error.Should().BeNull();
   }

   [Fact]
   public void Simple_value_object_wWith_custom_factory_name()
   {
      BoundaryWithCustomFactoryNames.TryGet(1, 2, out var obj).Should().BeTrue();
      obj.Should().BeEquivalentTo(new { Lower = 1, Upper = 2 });

      BoundaryWithCustomFactoryNames.TryGet(1, 2, out obj, out var error).Should().BeTrue();
      obj.Should().BeEquivalentTo(new { Lower = 1, Upper = 2 });
      error.Should().BeNull();
   }

   [Fact]
   public void Should_return_true_for_valid_generic_int_based_value()
   {
      var result = ValueObject_Generic_IntBased<string>.TryCreate(42, out var obj);

      result.Should().BeTrue();
      obj.Should().NotBeNull();
      obj!.Value.Should().Be(42);
   }

   [Fact]
   public void Should_return_true_for_valid_generic_string_based_value()
   {
      var result = ValueObject_Generic_StringBased<object>.TryCreate("test", out var obj);

      result.Should().BeTrue();
      obj.Should().NotBeNull();
      obj!.Value.Should().Be("test");
   }
}
