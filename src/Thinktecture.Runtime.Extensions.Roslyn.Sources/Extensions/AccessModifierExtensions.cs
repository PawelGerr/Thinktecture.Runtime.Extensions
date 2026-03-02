using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class AccessModifierExtensions
{
   public static string GetDefaultValueObjectKeyMemberName(this AccessModifier accessModifier, MemberKind memberKind)
   {
      return accessModifier == AccessModifier.Private && memberKind == MemberKind.Field
                ? "_value"
                : "Value";
   }

   public static string GetDefaultSmartEnumKeyMemberName(this AccessModifier accessModifier, MemberKind memberKind)
   {
      return accessModifier == AccessModifier.Private && memberKind == MemberKind.Field
                ? "_key"
                : "Key";
   }
}
