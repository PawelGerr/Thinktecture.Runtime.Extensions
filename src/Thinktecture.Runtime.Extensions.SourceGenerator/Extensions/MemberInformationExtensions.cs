using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class MemberInformationExtensions
{
   public static bool IsString(this IMemberInformation memberInformation)
   {
      return memberInformation.SpecialType == SpecialType.System_String;
   }

   public static bool DoesArithmeticOperationYieldDifferentType(this IMemberInformation memberInformation)
   {
      return memberInformation.SpecialType
                is SpecialType.System_Byte
                or SpecialType.System_SByte
                or SpecialType.System_Int16
                or SpecialType.System_UInt16;
   }
}
