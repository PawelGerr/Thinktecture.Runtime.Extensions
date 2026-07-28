namespace Thinktecture;

public static partial class TypeSymbolExtensions
{
   public static bool IsSmartEnum(this ITypeSymbol? type)
   {
      if (type is null || type.SpecialType != SpecialType.None)
         return false;

      var datas = type.GetAttributes();

      for (var i = 0; i < datas.Length; i++)
      {
         if (datas[i].AttributeClass is
             {
                TypeKind: not TypeKind.Error,
                Name: CodeAnalysis.Constants.Attributes.SmartEnum.NAME,
                ContainingNamespace:
                {
                   Name: CodeAnalysis.Constants.Attributes.NAMESPACE,
                   ContainingNamespace.IsGlobalNamespace: true
                }
             })
         {
            return true;
         }
      }

      return false;
   }

   public static bool IsAnyUnionType(this ITypeSymbol? type)
   {
      if (type is null || type.SpecialType != SpecialType.None)
         return false;

      var datas = type.GetAttributes();

      for (var i = 0; i < datas.Length; i++)
      {
         if (datas[i].AttributeClass is
             {
                TypeKind: not TypeKind.Error,
                Name: CodeAnalysis.Constants.Attributes.Union.NAME or CodeAnalysis.Constants.Attributes.Union.NAME_AD_HOC,
                ContainingNamespace:
                {
                   Name: CodeAnalysis.Constants.Attributes.NAMESPACE,
                   ContainingNamespace.IsGlobalNamespace: true
                }
             })
         {
            return true;
         }
      }

      return false;
   }

   public static bool IsSystemAction(this INamedTypeSymbol type)
   {
      return type is
      {
         Name: "Action",
         ContainingNamespace:
         {
            Name: "System",
            ContainingNamespace.IsGlobalNamespace: true
         }
      };
   }

   public static bool IsSystemFunc(this INamedTypeSymbol type)
   {
      return type is
      {
         Name: "Func",
         ContainingNamespace:
         {
            Name: "System",
            ContainingNamespace.IsGlobalNamespace: true
         }
      };
   }

   public static bool IsThinktectureArgument(this INamedTypeSymbol type)
   {
      return type is
      {
         Name: "Argument",
         TypeArguments.Length: 1,
         ContainingNamespace:
         {
            Name: "Thinktecture",
            ContainingNamespace.IsGlobalNamespace: true
         }
      };
   }

   /// <summary>
   /// Computes the accessibility that is actually visible for the provided <paramref name="type"/>.
   /// A public type nested in an internal type is effectively internal, and a constructed generic type
   /// is at most as accessible as its least accessible type argument.
   /// </summary>
   public static Accessibility GetEffectiveAccessibility(this ITypeSymbol type)
   {
      if (type.TypeKind == TypeKind.Error)
         return Accessibility.Public;

      switch (type)
      {
         case ITypeParameterSymbol:
            return Accessibility.Public;

         case IArrayTypeSymbol arrayType:
            return arrayType.ElementType.GetEffectiveAccessibility();

         case IPointerTypeSymbol pointerType:
            return pointerType.PointedAtType.GetEffectiveAccessibility();

         case INamedTypeSymbol namedType:
         {
            var accessibility = Accessibility.Public;

            for (INamedTypeSymbol? containingType = namedType; containingType is not null; containingType = containingType.ContainingType)
            {
               if (containingType.DeclaredAccessibility < accessibility)
                  accessibility = containingType.DeclaredAccessibility;
            }

            var typeArguments = namedType.TypeArguments;

            for (var i = 0; i < typeArguments.Length; i++)
            {
               var typeArgumentAccessibility = typeArguments[i].GetEffectiveAccessibility();

               if (typeArgumentAccessibility < accessibility)
                  accessibility = typeArgumentAccessibility;
            }

            return accessibility;
         }

         default:
            return Accessibility.Public;
      }
   }
}
