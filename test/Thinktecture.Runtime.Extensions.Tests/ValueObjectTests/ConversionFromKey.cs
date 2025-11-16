using System;
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

   [Fact]
   public void Should_support_explicit_conversion_from_int_for_generic_int_based_value_objects()
   {
      var obj = (ValueObject_Generic_IntBased<string>)42;

      obj.Value.Should().Be(42);
   }

   [Fact]
   public void Should_support_explicit_conversion_from_string_for_generic_string_based_value_objects()
   {
      var obj = (ValueObject_Generic_StringBased<object>)"test";

      obj.Value.Should().Be("test");
   }

   [Fact]
   public void Should_support_explicit_conversion_from_guid_for_generic_guid_based_value_objects()
   {
      var guid = Guid.NewGuid();
      var obj = (ValueObject_Generic_GuidBased<string>)guid;

      obj.Value.Should().Be(guid);
   }
}
