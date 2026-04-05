using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class StateExtensions
{
   public static int? FindGenericParameterIndex<T>(this T state, string keyType)
      where T : INamespaceAndName, IHasGenerics
   {
      var index = 0;

      for (var i = 0; i < state.ContainingTypes.Length; i++)
      {
         var containingType = state.ContainingTypes[i];

         if (containingType.GenericParameters.IsDefaultOrEmpty)
            continue;

         for (var j = 0; j < containingType.GenericParameters.Length; j++)
         {
            if (containingType.GenericParameters[j].Name == keyType)
               return index;

            index++;
         }
      }

      if (state.GenericParameters.IsDefaultOrEmpty)
         return null;

      for (var i = 0; i < state.GenericParameters.Length; i++)
      {
         if (state.GenericParameters[i].Name == keyType)
            return index;

         index++;
      }

      return null;
   }
}
