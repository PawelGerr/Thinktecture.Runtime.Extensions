namespace Thinktecture.CodeAnalysis;

public static class TypeSymbolExtensions
{
   extension(ITypeSymbol type)
   {
      /// <summary>
      /// Returns the 1-based index if the type is TypeParamRef1–5, or 0 if not.
      /// </summary>
      private int GetTypeParamRefIndex()
      {
         if (type is not INamedTypeSymbol { ContainingNamespace: { Name: Constants.TypeParamRef.NAMESPACE, ContainingNamespace.IsGlobalNamespace: true } } namedType)
            return 0;

         return namedType.Name switch
         {
            Constants.TypeParamRef.NAME_1 => 1,
            Constants.TypeParamRef.NAME_2 => 2,
            Constants.TypeParamRef.NAME_3 => 3,
            Constants.TypeParamRef.NAME_4 => 4,
            Constants.TypeParamRef.NAME_5 => 5,
            _ => 0,
         };
      }

      /// <summary>
      /// Returns the highest 1-based TypeParamRef index found in the type, or 0 if none.
      /// Walks constructed generic types and arrays recursively.
      /// </summary>
      public int GetMaxTypeParamRefIndex()
      {
         var index = GetTypeParamRefIndex(type);

         if (index != 0)
            return index;

         if (type is INamedTypeSymbol { IsGenericType: true } namedType)
         {
            var max = 0;

            for (var i = 0; i < namedType.TypeArguments.Length; i++)
            {
               var argMax = GetMaxTypeParamRefIndex(namedType.TypeArguments[i]);

               if (argMax > max)
                  max = argMax;
            }

            return max;
         }

         if (type is IArrayTypeSymbol arrayType)
            return GetMaxTypeParamRefIndex(arrayType.ElementType);

         return 0;
      }

      /// <summary>
      /// Resolves all TypeParamRefN placeholders in <paramref name="type"/> using the union's type parameters
      /// and returns the highest 1-based TypeParamRef index encountered during resolution.
      /// </summary>
      /// <param name="unionTypeParams">The union type's type parameters to substitute.</param>
      /// <param name="compilation">Optional compilation, needed for resolving array element types.</param>
      /// <returns>The resolved type and the highest TypeParamRef index found (0 if none).</returns>
      public (ITypeSymbol Resolved, int MaxTypeParamRefIndex) ResolveTypeParamRefs(
         ImmutableArray<ITypeParameterSymbol> unionTypeParams,
         Compilation compilation)
      {
         var index = GetTypeParamRefIndex(type);

         if (index != 0)
         {
            if (index > unionTypeParams.Length)
               return (type, index);

            ITypeSymbol resolved = unionTypeParams[index - 1];

            if (type.NullableAnnotation == NullableAnnotation.Annotated)
            {
               resolved = resolved.IsValueType
                             ? compilation.GetSpecialType(SpecialType.System_Nullable_T).Construct(unionTypeParams[index - 1])
                             : resolved.WithNullableAnnotation(NullableAnnotation.Annotated);
            }

            return (resolved, index);
         }

         switch (type)
         {
            case INamedTypeSymbol { IsGenericType: true } namedType:
            {
               var typeArgs = namedType.TypeArguments;
               ITypeSymbol[]? resolvedArgs = null;
               var maxIndex = 0;

               for (var i = 0; i < typeArgs.Length; i++)
               {
                  var (resolved, argMaxIndex) = typeArgs[i].ResolveTypeParamRefs(unionTypeParams, compilation);

                  if (argMaxIndex > maxIndex)
                     maxIndex = argMaxIndex;

                  if (!SymbolEqualityComparer.Default.Equals(resolved, typeArgs[i]))
                  {
                     if (resolvedArgs is null)
                     {
                        resolvedArgs = new ITypeSymbol[typeArgs.Length];

                        for (var j = 0; j < i; j++)
                        {
                           resolvedArgs[j] = typeArgs[j];
                        }
                     }

                     resolvedArgs[i] = resolved;
                  }
                  else if (resolvedArgs is not null)
                  {
                     resolvedArgs[i] = typeArgs[i];
                  }
               }

               return resolvedArgs is not null
                         ? (namedType.ConstructedFrom.Construct(resolvedArgs), maxIndex)
                         : (type, maxIndex);
            }
            case IArrayTypeSymbol arrayType:
            {
               var (resolvedElement, elementMaxIndex) = arrayType.ElementType.ResolveTypeParamRefs(unionTypeParams, compilation);

               return SymbolEqualityComparer.Default.Equals(resolvedElement, arrayType.ElementType)
                         ? (type, elementMaxIndex)
                         : (compilation.CreateArrayTypeSymbol(resolvedElement, arrayType.Rank), elementMaxIndex);
            }
            default:
            {
               return (type, 0);
            }
         }
      }

      public bool IsIEquatable(ITypeSymbol typeParam)
      {
         return type is INamedTypeSymbol { Name: "IEquatable", Arity: 1 } ieq
                && SymbolEqualityComparer.Default.Equals(ieq.TypeArguments[0], typeParam);
      }
   }
}
