using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture;

internal static class TypeDeclarationSyntaxExtensions
{
   public static bool IsPartial(this TypeDeclarationSyntax tds)
   {
      if (tds is null)
         throw new ArgumentNullException(nameof(tds));

      return tds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
   }

   public static bool IsValueObjectCandidate(this TypeDeclarationSyntax tds)
   {
      return tds.AttributeLists.Any(l => l.Attributes.Any(a => CouldBeValueObject(a.Name)));
   }

   private static bool CouldBeValueObject(TypeSyntax? type)
   {
      var typeName = ExtractTypeName(type);
      return typeName == "ValueObjectAttribute" || typeName == "ValueObject";
   }

   private static string? ExtractTypeName(TypeSyntax? type)
   {
      while (type is not null)
      {
         switch (type)
         {
            case IdentifierNameSyntax ins:
               return ins.Identifier.Text;

            case QualifiedNameSyntax qns:
               type = qns.Right;
               break;

            default:
               return null;
         }
      }

      return null;
   }

   public static bool IsEnumCandidate(this TypeDeclarationSyntax tds)
   {
      if (tds.AttributeLists.Any(l => l.Attributes.Any(a => CouldBeEnumType(a.Name))))
         return true;

      if (tds.BaseList?.Types.Count > 0)
      {
         foreach (var baseType in tds.BaseList.Types)
         {
            if (CouldBeEnumInterface(baseType))
               return true;
         }
      }

      return false;
   }

   private static bool CouldBeEnumType(TypeSyntax? type)
   {
      var typeName = ExtractTypeName(type);
      return typeName == "EnumGeneration" || typeName == "EnumGeneration";
   }

   private static bool CouldBeEnumInterface(BaseTypeSyntax? baseType)
   {
      var type = baseType?.Type;

      while (type is not null)
      {
         switch (type)
         {
            case QualifiedNameSyntax qns:
               type = qns.Right;
               break;

            case GenericNameSyntax genericNameSyntax:
               return (genericNameSyntax.Identifier.Text == "IEnum" || genericNameSyntax.Identifier.Text == "IValidatableEnum") &&
                      genericNameSyntax.TypeArgumentList.Arguments.Count == 1;

            default:
               return false;
         }
      }

      return false;
   }
}