using System;
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
   }
}
