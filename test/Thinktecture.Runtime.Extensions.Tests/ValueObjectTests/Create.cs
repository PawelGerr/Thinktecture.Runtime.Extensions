using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Thinktecture.Runtime.Tests.TestValueObjects;

namespace Thinktecture.Runtime.Tests.ValueObjectTests;

public class Create
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
   public void Simple_value_object_with_custom_factory_name()
   {
      IntBasedReferenceValueObjectWithCustomFactoryNames.Get(1)
                                                        .Should().BeEquivalentTo(new { Property = 1 });
   }

   [Fact]
   public void Complex_value_object_with_custom_factory_name()
   {
      BoundaryWithCustomFactoryNames.Get(1, 2)
                                    .Should().BeEquivalentTo(new { Lower = 1, Upper = 2 });
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

   [Fact]
   public void Properties_with_init_should_be_recognized_as_value_object_properties()
   {
      var obj = ValueObjectWithInitProperties.Create(initExpression: 1, initBody: 2, publicPropertyDefaultInit: 3, privatePropertyDefaultInit: 4);

      obj.Should().BeEquivalentTo(new
                                  {
                                     InitExpression = 1,
                                     InitBody = 2,
                                     PublicPropertyDefaultInit = 3
                                  });

      obj.GetType().GetProperty("PrivatePropertyDefaultInit", BindingFlags.Instance | BindingFlags.NonPublic)!
         .GetValue(obj).Should().BeOfType<int>()
         .Subject.Should().Be(4);
   }
}
