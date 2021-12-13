using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis;

internal class SyntaxReceiver : ISyntaxReceiver
{
   public List<TypeDeclarationSyntax> Enums { get; }
   public List<TypeDeclarationSyntax> ValueObjects { get; }

   public SyntaxReceiver()
   {
      Enums = new List<TypeDeclarationSyntax>();
      ValueObjects = new List<TypeDeclarationSyntax>();
   }

   public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
   {
      if (syntaxNode is not TypeDeclarationSyntax tds)
         return;

      if (tds is not ClassDeclarationSyntax && tds is not StructDeclarationSyntax
          || !tds.IsPartial())
         return;

      if (tds.IsEnumCandidate())
         Enums.Add(tds);

      if (tds.IsValueObjectCandidate())
         ValueObjects.Add(tds);
   }
}