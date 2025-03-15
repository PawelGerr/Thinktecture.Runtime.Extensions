using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class TypeParameterSymbolExtensions
{
   public static IReadOnlyList<GenericTypeParameterState> GetGenericTypeParameters(this ImmutableArray<ITypeParameterSymbol> generics)
   {
      if (generics.IsDefaultOrEmpty)
         return [];

      var genericTypeParameters = new List<GenericTypeParameterState>(generics.Length);

      for (var i = 0; i < generics.Length; i++)
      {
         var typeParam = generics[i];
         var constraints = typeParam.GetConstraints();

         genericTypeParameters.Add(new GenericTypeParameterState(
                                      typeParam.Name,
                                      constraints));
      }

      return genericTypeParameters;
   }
}
