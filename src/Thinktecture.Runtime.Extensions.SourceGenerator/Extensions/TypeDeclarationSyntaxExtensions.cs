using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture;

internal static class TypeDeclarationSyntaxExtensions
{
   public static bool IsPartial(this TypeDeclarationSyntax tds)
   {
      if (tds is null)
         throw new ArgumentNullException(nameof(tds));

      for (var i = 0; i < tds.Modifiers.Count; i++)
      {
         if (tds.Modifiers[i].IsKind(SyntaxKind.PartialKeyword))
            return true;
      }

      return false;
   }

   public static bool IsGeneric(this TypeDeclarationSyntax tds)
   {
      if (tds is null)
         throw new ArgumentNullException(nameof(tds));

      return tds.TypeParameterList is { Parameters.Count: > 0 };
   }

   private static string? ExtractTypeName(NameSyntax? name)
   {
      return name switch
      {
         SimpleNameSyntax ins => ins.Identifier.Text,
         QualifiedNameSyntax qns => qns.Right.Identifier.Text,
         _ => null
      };
   }

   public static bool IsEnumCandidate(this TypeDeclarationSyntax tds)
   {
      for (var i = 0; i < tds.AttributeLists.Count; i++)
      {
         var attributes = tds.AttributeLists[i].Attributes;

         for (var j = 0; j < attributes.Count; j++)
         {
            if (CouldBeEnumGenerationAttribute(attributes[j].Name))
               return true;
         }
      }

      if (tds.BaseList?.Types.Count > 0)
      {
         var baseTypes = tds.BaseList.Types;

         for (var i = 0; i < baseTypes.Count; i++)
         {
            if (CouldBeEnumInterface(baseTypes[i]))
               return true;
         }
      }

      return false;
   }

   private static bool CouldBeEnumGenerationAttribute(NameSyntax name)
   {
      var typeName = ExtractTypeName(name);
      return typeName is "EnumGeneration" or "EnumGenerationAttribute";
   }

   private static bool CouldBeEnumInterface(BaseTypeSyntax? baseType)
   {
      var type = baseType?.Type;

      return type switch
      {
         QualifiedNameSyntax { Right: GenericNameSyntax gns } => CouldBeEnumInterface(gns),
         GenericNameSyntax gns => CouldBeEnumInterface(gns),
         _ => false
      };
   }

   private static bool CouldBeEnumInterface(GenericNameSyntax genericNameSyntax)
   {
      return genericNameSyntax.Identifier.Text is "IEnum" or "IValidatableEnum"
             && genericNameSyntax.TypeArgumentList.Arguments.Count == 1;
   }
}
