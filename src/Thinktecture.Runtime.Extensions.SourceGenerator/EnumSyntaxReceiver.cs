using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   internal class EnumSyntaxReceiver : ISyntaxReceiver
   {
      public List<TypeDeclarationSyntax> Enums { get; }

      public EnumSyntaxReceiver()
      {
         Enums = new List<TypeDeclarationSyntax>();
      }

      /// <inheritdoc />
      public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
      {
         if (syntaxNode is TypeDeclarationSyntax tds
             && (tds is ClassDeclarationSyntax || tds is StructDeclarationSyntax))
         {
            if (tds.IsEnumCandidate() && tds.IsPartial())
               Enums.Add(tds);
         }
      }
   }
}
