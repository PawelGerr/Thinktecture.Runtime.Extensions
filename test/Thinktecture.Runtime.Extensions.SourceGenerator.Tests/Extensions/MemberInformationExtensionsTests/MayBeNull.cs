using NSubstitute;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.MemberInformationExtensionsTests;

public class MayBeNull
{
   [Fact]
   public void Should_return_true_for_reference_type()
   {
      var memberInfo = Substitute.For<IMemberInformation>();
      memberInfo.IsReferenceType.Returns(true);
      memberInfo.IsValueType.Returns(false);
      memberInfo.IsTypeParameter.Returns(false);
      memberInfo.IsNullableStruct.Returns(false);

      memberInfo.MayBeNull().Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_nullable_struct()
   {
      var memberInfo = Substitute.For<IMemberInformation>();
      memberInfo.IsReferenceType.Returns(false);
      memberInfo.IsValueType.Returns(true);
      memberInfo.IsTypeParameter.Returns(false);
      memberInfo.IsNullableStruct.Returns(true);

      memberInfo.MayBeNull().Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_unconstrained_type_parameter()
   {
      var memberInfo = Substitute.For<IMemberInformation>();
      memberInfo.IsReferenceType.Returns(false);
      memberInfo.IsValueType.Returns(false);
      memberInfo.IsTypeParameter.Returns(true);
      memberInfo.IsNullableStruct.Returns(false);

      memberInfo.MayBeNull().Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_value_type()
   {
      var memberInfo = Substitute.For<IMemberInformation>();
      memberInfo.IsReferenceType.Returns(false);
      memberInfo.IsValueType.Returns(true);
      memberInfo.IsTypeParameter.Returns(false);
      memberInfo.IsNullableStruct.Returns(false);

      memberInfo.MayBeNull().Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_struct_constrained_type_parameter()
   {
      var memberInfo = Substitute.For<IMemberInformation>();
      memberInfo.IsReferenceType.Returns(false);
      memberInfo.IsValueType.Returns(true);
      memberInfo.IsTypeParameter.Returns(true);
      memberInfo.IsNullableStruct.Returns(false);

      memberInfo.MayBeNull().Should().BeFalse();
   }
}
