using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture;

public static class EnumSourceGeneratorStateExtensions
{
   public static bool IsExtensible(this EnumSourceGeneratorState state)
   {
      return !state.HasBaseEnum // derived enums must not be extensible
             && state.IsReferenceType
             && state.Settings.IsExtensible;
   }

   public static string GetRuntimeTypeName(this EnumSourceGeneratorState state)
   {
      return state.IsExtensible() ? "{GetType().Name}" : state.Name;
   }
}
