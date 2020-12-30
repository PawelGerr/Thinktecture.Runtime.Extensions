using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   internal static class MemberDeclarationSyntaxExtensions
   {
      public static bool IsStatic(this MemberDeclarationSyntax mds)
      {
         if (mds is null)
            throw new ArgumentNullException(nameof(mds));

         return mds.Modifiers.Any(SyntaxKind.StaticKeyword);
      }

      public static bool IsPublic(this MemberDeclarationSyntax mds)
      {
         if (mds is null)
            throw new ArgumentNullException(nameof(mds));

         return mds.Modifiers.Any(SyntaxKind.PublicKeyword);
      }

      public static bool IsReadOnly(this MemberDeclarationSyntax mds)
      {
         if (mds is null)
            throw new ArgumentNullException(nameof(mds));

         return mds.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
      }
   }
}
