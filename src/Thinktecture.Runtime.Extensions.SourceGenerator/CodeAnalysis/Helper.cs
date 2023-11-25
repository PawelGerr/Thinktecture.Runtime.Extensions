namespace Thinktecture.CodeAnalysis;

public class Helper
{
   public static string GetDefaultValueObjectKeyMemberName(ValueObjectAccessModifier accessModifier, ValueObjectMemberKind memberKind)
   {
      return accessModifier == ValueObjectAccessModifier.Private && memberKind == ValueObjectMemberKind.Field
                ? "_value"
                : "Value";
   }

   public static string GetDefaultSmartEnumKeyMemberName(ValueObjectAccessModifier accessModifier, ValueObjectMemberKind memberKind)
   {
      return accessModifier == ValueObjectAccessModifier.Private && memberKind == ValueObjectMemberKind.Field
                ? "_key"
                : "Key";
   }
}
