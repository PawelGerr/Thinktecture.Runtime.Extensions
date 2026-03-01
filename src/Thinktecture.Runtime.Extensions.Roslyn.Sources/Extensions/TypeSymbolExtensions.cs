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
}
