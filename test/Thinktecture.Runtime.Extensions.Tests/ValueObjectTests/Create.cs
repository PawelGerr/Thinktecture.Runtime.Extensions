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

   [Fact]
   public void Should_create_generic_int_based_instance_with_different_type_arguments()
   {
      var stringInstance = ValueObject_Generic_IntBased<string>.Create(42);
      var intInstance = ValueObject_Generic_IntBased<int>.Create(42);

      stringInstance.Should().NotBeNull();
      intInstance.Should().NotBeNull();
      stringInstance.Value.Should().Be(42);
      intInstance.Value.Should().Be(42);
   }

   [Fact]
   public void Should_maintain_separate_instances_for_different_type_arguments()
   {
      var stringInstance1 = ValueObject_Generic_IntBased<string>.Create(42);
      var stringInstance2 = ValueObject_Generic_IntBased<string>.Create(42);
      var intInstance = ValueObject_Generic_IntBased<int>.Create(42);

      stringInstance1.Should().Be(stringInstance2);
      stringInstance1.Should().NotBeSameAs(stringInstance2);
   }

   [Theory]
   [InlineData(1)]
   [InlineData(42)]
   [InlineData(int.MaxValue)]
   [InlineData(int.MinValue)]
   public void Should_create_generic_int_based_instance_with_valid_value(int value)
   {
      var obj = ValueObject_Generic_IntBased<string>.Create(value);

      obj.Should().NotBeNull();
      obj.Value.Should().Be(value);
   }

   [Theory]
   [InlineData("test")]
   [InlineData("")]
   [InlineData("value")]
   public void Should_create_generic_string_based_instance_with_valid_value(string value)
   {
      var obj = ValueObject_Generic_StringBased<object>.Create(value);

      obj.Should().NotBeNull();
      obj.Value.Should().Be(value);
   }

   [Fact]
   public void Should_create_generic_string_based_instance_with_different_type_arguments()
   {
      var stringInstance = ValueObject_Generic_StringBased<string>.Create("test");
      var objectInstance = ValueObject_Generic_StringBased<object>.Create("test");

      stringInstance.Should().NotBeNull();
      objectInstance.Should().NotBeNull();
      stringInstance.Value.Should().Be("test");
      objectInstance.Value.Should().Be("test");
   }

   [Fact]
   public void Should_create_generic_guid_based_instance_with_valid_guid()
   {
      var guid = Guid.NewGuid();
      var obj = ValueObject_Generic_GuidBased<string>.Create(guid);

      obj.Should().NotBeNull();
      obj.Value.Should().Be(guid);
   }

   [Fact]
   public void Should_create_generic_guid_based_instance_with_different_type_arguments()
   {
      var guid = Guid.NewGuid();
      var stringInstance = ValueObject_Generic_GuidBased<string>.Create(guid);
      var intInstance = ValueObject_Generic_GuidBased<int>.Create(guid);

      stringInstance.Should().NotBeNull();
      intInstance.Should().NotBeNull();
      stringInstance.Value.Should().Be(guid);
      intInstance.Value.Should().Be(guid);
   }

   [Fact]
   public void Should_create_generic_struct_based_instance_with_valid_value()
   {
      var obj = StructValueObject_Generic_IntBased<string>.Create(42);

      obj.Value.Should().Be(42);
   }

   [Fact]
   public void Should_maintain_separate_instances_for_different_type_arguments_for_struct()
   {
      var stringInstance1 = StructValueObject_Generic_IntBased<string>.Create(42);
      var stringInstance2 = StructValueObject_Generic_IntBased<string>.Create(42);

      stringInstance1.Should().Be(stringInstance2);
   }
}
