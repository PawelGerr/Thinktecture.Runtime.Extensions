using System;
using System.ComponentModel.DataAnnotations;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class FactroryTests
{
   [Fact]
   public void With_EmptyStringInFactoryMethodsYieldsNull_null_should_yield_null()
   {
      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.Create(null)
                                                                              .Should().BeNull();
   }

   [Fact]
   public void With_EmptyStringInFactoryMethodsYieldsNull_empty_string_should_yield_null()
   {
      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.Create(String.Empty)
                                                                              .Should().BeNull();
   }

   [Fact]
   public void With_EmptyStringInFactoryMethodsYieldsNull_whitespaces_should_yield_null()
   {
      StringBasedReferenceValueObjectWithEmptyStringInFactoryMethodsYieldsNull.Create(" ")
                                                                              .Should().BeNull();
   }

   [Fact]
   public void With_NullInFactoryMethodsYieldsNull_null_should_yield_null()
   {
      StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.Create(null)
                                                                       .Should().BeNull();
   }

   [Fact]
   public void With_NullInFactoryMethodsYieldsNull_empty_string_should_yield_null()
   {
      FluentActions.Invoking(() => StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.Create(String.Empty))
                   .Should().Throw<ValidationException>().WithMessage("Property cannot be empty.");
   }

   [Fact]
   public void With_NullInFactoryMethodsYieldsNull_whitespaces_should_yield_null()
   {
      FluentActions.Invoking(() => StringBasedReferenceValueObjectWithNullInFactoryMethodsYieldsNull.Create(" "))
                   .Should().Throw<ValidationException>().WithMessage("Property cannot be empty.");
   }
}
