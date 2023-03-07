using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture;

public static class EnumSettingsExtensions
{
   public static IMemberState CreateKeyProperty(this EnumSettings settings, ITypeSymbol keyType)
   {
      var keyPropertyName = settings.GetKeyPropertyName();
      return new DefaultMemberState(TypedMemberState.GetOrCreate(keyType), keyPropertyName, keyPropertyName.MakeArgumentName(), false);
   }

   private static string GetKeyPropertyName(this EnumSettings settings)
   {
      var name = settings.KeyPropertyName;

      if (name is not null && !StringComparer.OrdinalIgnoreCase.Equals(name, "item"))
         return name;

      return "Key";
   }
}
