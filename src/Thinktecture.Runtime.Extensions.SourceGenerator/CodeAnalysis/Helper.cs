namespace Thinktecture.CodeAnalysis;

public class Helper
{
   public static string GetDefaultValueObjectKeyMemberName(AccessModifier accessModifier, MemberKind memberKind)
   {
      return accessModifier == AccessModifier.Private && memberKind == MemberKind.Field
                ? "_value"
                : "Value";
   }

   public static string GetDefaultSmartEnumKeyMemberName(AccessModifier accessModifier, MemberKind memberKind)
   {
      return accessModifier == AccessModifier.Private && memberKind == MemberKind.Field
                ? "_key"
                : "Key";
   }
}
