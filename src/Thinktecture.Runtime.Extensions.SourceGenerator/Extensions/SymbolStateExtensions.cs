using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class SymbolStateExtensions
{
   public static bool IsString(this IMemberState memberState)
   {
      return memberState.SpecialType == SpecialType.System_String;
   }
}
