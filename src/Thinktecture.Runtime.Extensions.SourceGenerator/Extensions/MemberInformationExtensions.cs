using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class MemberInformationExtensions
{
   public static bool IsString(this IMemberInformation memberInformation)
   {
      return memberInformation.SpecialType == SpecialType.System_String;
   }
}
