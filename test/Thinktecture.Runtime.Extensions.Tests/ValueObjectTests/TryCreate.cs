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
}
