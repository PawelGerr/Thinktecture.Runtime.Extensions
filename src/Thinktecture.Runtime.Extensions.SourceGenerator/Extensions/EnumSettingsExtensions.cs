using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture;

public static class EnumSettingsExtensions
{
   public static IMemberState CreateKeyProperty(this AllEnumSettings settings, ITypedMemberState keyMemberState)
   {
      var keyPropertyName = settings.GetKeyPropertyName();
      return new DefaultMemberState(keyMemberState, keyPropertyName, keyPropertyName.MakeArgumentName());
   }

   private static string GetKeyPropertyName(this AllEnumSettings settings)
   {
      var name = settings.KeyPropertyName;

      if (name is not null && !StringComparer.OrdinalIgnoreCase.Equals(name, "item"))
         return name;

      return "Key";
   }
}
