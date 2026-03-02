using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class TypeParameterSymbolExtensions
{
   public static ImmutableArray<GenericTypeParameterState> GetGenericTypeParameters(this ImmutableArray<ITypeParameterSymbol> generics)
   {
      if (generics.IsDefaultOrEmpty)
         return [];

      return ImmutableArray.CreateRange(generics, static typeParam =>
      {
         var constraints = typeParam.GetConstraints();

         return new GenericTypeParameterState(
            typeParam.Name,
            constraints);
      });
   }
}
