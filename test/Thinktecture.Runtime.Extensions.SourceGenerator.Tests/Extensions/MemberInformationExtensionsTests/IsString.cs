using Microsoft.CodeAnalysis;
using NSubstitute;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.MemberInformationExtensionsTests;

public class IsString
{
   [Fact]
   public void Should_return_true_when_SpecialType_is_System_String()
   {
      var memberInfo = Substitute.For<IMemberInformation>();
      memberInfo.SpecialType.Returns(SpecialType.System_String);

      memberInfo.IsString().Should().BeTrue();
   }

   [Theory]
   [InlineData(SpecialType.None)]
   [InlineData(SpecialType.System_Object)]
   [InlineData(SpecialType.System_Void)]
   [InlineData(SpecialType.System_Boolean)]
   [InlineData(SpecialType.System_Char)]
   [InlineData(SpecialType.System_SByte)]
   [InlineData(SpecialType.System_Byte)]
   [InlineData(SpecialType.System_Int16)]
   [InlineData(SpecialType.System_UInt16)]
   [InlineData(SpecialType.System_Int32)]
   [InlineData(SpecialType.System_UInt32)]
   [InlineData(SpecialType.System_Int64)]
   [InlineData(SpecialType.System_UInt64)]
   [InlineData(SpecialType.System_Decimal)]
   [InlineData(SpecialType.System_Single)]
   [InlineData(SpecialType.System_Double)]
   [InlineData(SpecialType.System_DateTime)]
   [InlineData(SpecialType.System_IntPtr)]
   [InlineData(SpecialType.System_UIntPtr)]
   [InlineData(SpecialType.System_Array)]
   [InlineData(SpecialType.System_Collections_Generic_IEnumerable_T)]
   [InlineData(SpecialType.System_Collections_Generic_IList_T)]
   [InlineData(SpecialType.System_Collections_Generic_ICollection_T)]
   [InlineData(SpecialType.System_Nullable_T)]
   [InlineData(SpecialType.System_Enum)]
   [InlineData(SpecialType.System_MulticastDelegate)]
   [InlineData(SpecialType.System_Delegate)]
   [InlineData(SpecialType.System_ValueType)]
   public void Should_return_false_when_SpecialType_is_not_System_String(SpecialType specialType)
   {
      var memberInfo = Substitute.For<IMemberInformation>();
      memberInfo.SpecialType.Returns(specialType);

      memberInfo.IsString().Should().BeFalse();
   }
}
