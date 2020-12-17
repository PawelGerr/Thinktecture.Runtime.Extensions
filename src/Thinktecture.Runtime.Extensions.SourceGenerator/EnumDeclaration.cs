using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   public class EnumDeclaration
   {
      public TypeDeclarationSyntax TypeDeclarationSyntax { get; }
      public GenericNameSyntax BaseType { get; }

      public EnumDeclaration(
         TypeDeclarationSyntax cds,
         GenericNameSyntax baseType)
      {
         TypeDeclarationSyntax = cds ?? throw new ArgumentNullException(nameof(cds));
         BaseType = baseType ?? throw new ArgumentNullException(nameof(baseType));
      }
   }
}
