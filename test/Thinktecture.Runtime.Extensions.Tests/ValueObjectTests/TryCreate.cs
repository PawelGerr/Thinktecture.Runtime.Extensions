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
   }

   [Fact]
   public void With_EmptyStringInFactoryMethodsYieldsNull_empty_string_should_yield_null()
   {
      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.TryCreate(String.Empty, out var obj).Should().BeTrue();
      obj.Should().BeNull();
   }

   [Fact]
   public void With_EmptyStringInFactoryMethodsYieldsNull_whitespaces_should_yield_null()
   {
      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.TryCreate(" ", out var obj).Should().BeTrue();
      obj.Should().BeNull();
   }

   [Fact]
   public void With_NullInFactoryMethodsYieldsNull_null_should_yield_null()
   {
      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.TryCreate(null, out var obj).Should().BeTrue();
      obj.Should().BeNull();
   }

   [Fact]
   public void With_NullInFactoryMethodsYieldsNull_empty_string_should_yield_null()
   {
      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.TryCreate(String.Empty, out var obj).Should().BeFalse();
      obj.Should().BeNull();
   }

   [Fact]
   public void With_NullInFactoryMethodsYieldsNull_whitespaces_should_yield_null()
   {
      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.TryCreate(" ", out var obj).Should().BeFalse();
      obj.Should().BeNull();
   }

   [Fact]
   public void With_custom_factory_name()
   {
      IntBasedReferenceValueObjectWithCustomFactoryNames.TryGet(1, out var obj).Should().BeTrue();
      obj.Should().BeEquivalentTo(new { Property = 1 });
   }

   [Fact]
   public void Simple_value_object_wWith_custom_factory_name()
   {
      BoundaryWithCustomFactoryNames.TryGet(1, 2, out var obj).Should().BeTrue();
      obj.Should().BeEquivalentTo(new { Lower = 1, Upper = 2 });
   }
}
