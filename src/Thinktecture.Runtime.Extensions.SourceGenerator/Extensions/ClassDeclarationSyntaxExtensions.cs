using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   internal static class ClassDeclarationSyntaxExtensions
   {
      public static bool IsAbstract(this ClassDeclarationSyntax cds)
      {
         if (cds is null)
            throw new ArgumentNullException(nameof(cds));

         return cds.Modifiers.Any(m => m.IsKind(SyntaxKind.AbstractKeyword));
      }

      public static bool IsPartial(this ClassDeclarationSyntax cds)
      {
         if (cds is null)
            throw new ArgumentNullException(nameof(cds));

         return cds.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
      }
   }
}
