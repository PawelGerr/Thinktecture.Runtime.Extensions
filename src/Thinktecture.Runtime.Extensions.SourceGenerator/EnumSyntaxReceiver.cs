using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   internal class EnumSyntaxReceiver : ISyntaxReceiver
   {
      public List<EnumDeclaration> Enums { get; }

      public EnumSyntaxReceiver()
      {
         Enums = new List<EnumDeclaration>();
      }

      /// <inheritdoc />
      public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
      {
         if (syntaxNode is TypeDeclarationSyntax tds
             && (tds is ClassDeclarationSyntax || tds is StructDeclarationSyntax))
         {
            if (tds.IsEnumCandidate(out var enumDeclaration) && tds.IsPartial())
               Enums.Add(enumDeclaration);
         }
      }
   }
}
