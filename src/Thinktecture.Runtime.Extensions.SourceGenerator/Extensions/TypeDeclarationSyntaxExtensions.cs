using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   internal static class TypeDeclarationSyntaxExtensions
   {
      public static bool IsPartial(this TypeDeclarationSyntax tds)
      {
         if (tds is null)
            throw new ArgumentNullException(nameof(tds));

         return tds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
      }

      public static bool IsEnumCandidate(this TypeDeclarationSyntax tds)
      {
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

      private static bool CouldBeEnumInterface(BaseTypeSyntax? baseType)
      {
         var type = baseType?.Type;

         while (type is not null)
         {
            switch (type)
            {
               case IdentifierNameSyntax: // could be an alias
                  return true;

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
}
