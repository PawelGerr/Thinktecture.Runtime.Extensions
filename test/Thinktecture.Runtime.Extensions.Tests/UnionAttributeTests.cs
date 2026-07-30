using System;

namespace Thinktecture.Runtime.Tests;

public class UnionAttributeTests
{
   [Fact]
   public void Should_treat_stateless_interface_member_as_nullable_reference_type()
   {
      // The stateless-implies-nullable cascade must cover all reference types, including interfaces
      // (Type.IsClass is false for interfaces), matching AdHocUnionAttribute and the source generator.
      var attribute = new UnionAttribute<IFormattable, string>
      {
         T1IsStateless = true
      };

      attribute.T1IsNullableReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_not_treat_stateless_struct_member_as_nullable_reference_type()
   {
      var attribute = new UnionAttribute<int, string>
      {
         T1IsStateless = true
      };

      attribute.T1IsNullableReferenceType.Should().BeFalse();
   }

   [Fact]
   public void Should_return_explicit_value_when_set_on_struct_member()
   {
      // The property reports an explicitly assigned value even for structs; the generated code
      // ignores the setting for struct members (see the XML documentation of the property).
      var attribute = new UnionAttribute<int, string>
      {
         T1IsNullableReferenceType = true
      };

      attribute.T1IsNullableReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_apply_stateless_cascade_when_member_is_last_of_arity_2()
   {
      var attribute = new UnionAttribute<int, string>
      {
         T2IsStateless = true
      };

      attribute.T2IsNullableReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_apply_stateless_cascade_when_member_is_last_of_arity_3()
   {
      var attribute = new UnionAttribute<int, string, IFormattable>
      {
         T3IsStateless = true
      };

      attribute.T3IsNullableReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_apply_stateless_cascade_when_member_is_last_of_arity_4()
   {
      var attribute = new UnionAttribute<int, string, bool, IFormattable>
      {
         T4IsStateless = true
      };

      attribute.T4IsNullableReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_apply_stateless_cascade_when_member_is_last_of_arity_5()
   {
      var attribute = new UnionAttribute<int, string, bool, char, IFormattable>
      {
         T5IsStateless = true
      };

      attribute.T5IsNullableReferenceType.Should().BeTrue();
   }
}
