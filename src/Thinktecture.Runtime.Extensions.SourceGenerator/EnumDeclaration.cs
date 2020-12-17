using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   public class EnumDeclaration
   {
      public ClassDeclarationSyntax ClassDeclarationSyntax { get; }
      public GenericNameSyntax BaseType { get; }

      public EnumDeclaration(
         ClassDeclarationSyntax cds,
         GenericNameSyntax baseType)
      {
         ClassDeclarationSyntax = cds ?? throw new ArgumentNullException(nameof(cds));
         BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
      }
   }
}
