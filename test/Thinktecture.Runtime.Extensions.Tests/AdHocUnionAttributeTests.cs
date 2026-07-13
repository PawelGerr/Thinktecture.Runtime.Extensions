using System;

namespace Thinktecture.Runtime.Tests;

public class AdHocUnionAttributeTests
{
   [Fact]
   public void Should_treat_stateless_interface_member_as_nullable_reference_type()
   {
      // Regression: the stateless-implies-nullable cascade must cover all reference types,
      // including interfaces (Type.IsClass is false for interfaces), matching the XML remark
      // and the source generator (ITypeSymbol.IsReferenceType).
      var attribute = new AdHocUnionAttribute(typeof(IFormattable), typeof(string))
      {
         T1IsStateless = true
      };

      attribute.T1IsNullableReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_treat_stateless_class_member_as_nullable_reference_type()
   {
      var attribute = new AdHocUnionAttribute(typeof(string), typeof(int))
      {
         T1IsStateless = true
      };

      attribute.T1IsNullableReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_not_treat_stateless_struct_member_as_nullable_reference_type()
   {
      var attribute = new AdHocUnionAttribute(typeof(int), typeof(string))
      {
         T1IsStateless = true
      };

      attribute.T1IsNullableReferenceType.Should().BeFalse();
   }

   [Fact]
   public void Should_apply_cascade_to_stateless_interface_member_at_higher_positions()
   {
      var attribute = new AdHocUnionAttribute(typeof(string), typeof(int), typeof(IFormattable))
      {
         T3IsStateless = true
      };

      attribute.T3IsNullableReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_return_explicit_value_when_set_regardless_of_member_kind()
   {
      var attribute = new AdHocUnionAttribute(typeof(int), typeof(string))
      {
         T1IsNullableReferenceType = true
      };

      attribute.T1IsNullableReferenceType.Should().BeTrue();
   }
}
