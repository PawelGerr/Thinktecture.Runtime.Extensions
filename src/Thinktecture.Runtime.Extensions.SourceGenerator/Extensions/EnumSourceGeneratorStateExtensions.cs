using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class EnumSourceGeneratorStateExtensions
{
   public static bool IsExtensible<TBaseEnumExtension>(this EnumSourceGeneratorStateBase<TBaseEnumExtension> state)
      where TBaseEnumExtension : IEquatable<TBaseEnumExtension>
   {
      return !state.HasBaseEnum // derived enums must not be extensible
             && state.IsReferenceType
             && state.Settings.IsExtensible;
   }

   public static string GetRuntimeTypeName<TBaseEnumExtension>(this EnumSourceGeneratorStateBase<TBaseEnumExtension> state)
      where TBaseEnumExtension : IEquatable<TBaseEnumExtension>
   {
      return state.IsExtensible() ? "{GetType().Name}" : state.Name;
   }
}
