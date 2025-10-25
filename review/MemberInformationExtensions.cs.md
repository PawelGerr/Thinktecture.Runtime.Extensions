Issues found in src/Thinktecture.Runtime.Extensions.SourceGenerator/Extensions/MemberInformationExtensions.cs

1) Unnecessarily public API surface (design)
- The extensions class and methods are declared public but appear to be used only internally by the generator.
- Public Roslyn-specific helpers expand the assembly&#39;s API surface without necessity.
- Recommendation: Make the class (and methods) internal.

Suggested change:
- public static class MemberInformationExtensions
+ internal static class MemberInformationExtensions

2) Missing char in arithmetic promotion set (correctness)
- In C#, arithmetic on char promotes to int, similar to byte/sbyte/short/ushort.
- The method DoesArithmeticOperationYieldDifferentType omits SpecialType.System_Char, which likely leads to incorrect false for char members.
- Recommendation: Include SpecialType.System_Char.

Suggested change:
-       or SpecialType.System_UInt16;
+       or SpecialType.System_UInt16
+       or SpecialType.System_Char;

3) Ambiguous method name/contract (API clarity)
- The name DoesArithmeticOperationYieldDifferentType is vague (which operation exactly? +, -, *, /, also bitwise?).
- Recommendation: Either rename to IsPromotedToIntOnArithmetic (or similar) or document explicitly that the check is for arithmetic promotion rules of small integral types.

Optional consolidated revision:
internal static class MemberInformationExtensions
{
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal static bool IsString(this IMemberInformation memberInformation)
   {
      return memberInformation.SpecialType == SpecialType.System_String;
   }

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   internal static bool IsPromotedToIntOnArithmetic(this IMemberInformation memberInformation)
   {
      return memberInformation.SpecialType
                is SpecialType.System_Byte
                or SpecialType.System_SByte
                or SpecialType.System_Int16
                or SpecialType.System_UInt16
                or SpecialType.System_Char;
   }
}
