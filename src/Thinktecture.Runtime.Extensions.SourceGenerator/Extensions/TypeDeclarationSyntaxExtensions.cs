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
      public static bool IsAbstract(this TypeDeclarationSyntax tds)
      {
         if (tds is null)
            throw new ArgumentNullException(nameof(tds));

         return tds.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword));
      }

      public static bool IsPartial(this TypeDeclarationSyntax tds)
      {
         if (tds is null)
            throw new ArgumentNullException(nameof(tds));

         return tds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
      }

      public static bool IsEnum(
         this TypeDeclarationSyntax tds)
      {
         return IsEnum(tds, out _);
      }

      public static bool IsEnum(
         this TypeDeclarationSyntax tds,
         out EnumDeclaration enumDeclaration)
      {
         if (tds.BaseList?.Types.Count > 0)
         {
            foreach (var baseType in tds.BaseList.Types)
            {
               var enumInterfaces = GetEnumInterfaces(baseType);

               if (enumInterfaces.Count > 0)
               {
                  enumDeclaration = new EnumDeclaration(tds, enumInterfaces);
                  return true;
               }
            }
         }

         enumDeclaration = null!;
         return false;
      }

      private static IReadOnlyList<GenericNameSyntax> GetEnumInterfaces(BaseTypeSyntax? type)
      {
         List<GenericNameSyntax>? enumInterfaces = null;

         if (type?.Type is GenericNameSyntax genericNameSyntax)
         {
            if ((genericNameSyntax.Identifier.Text == "IEnum" || genericNameSyntax.Identifier.Text == "IValidatableEnum") &&
                genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
            {
               (enumInterfaces ??= new List<GenericNameSyntax>()).Add(genericNameSyntax);
            }
         }

         return enumInterfaces ?? (IReadOnlyList<GenericNameSyntax>)Array.Empty<GenericNameSyntax>();
      }
   }
}
