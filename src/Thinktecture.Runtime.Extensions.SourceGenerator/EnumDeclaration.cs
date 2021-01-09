using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   public class EnumDeclaration
   {
      public TypeDeclarationSyntax TypeDeclarationSyntax { get; }
      public IReadOnlyList<GenericNameSyntax> EnumInterfaces { get; }

      public EnumDeclaration(
         TypeDeclarationSyntax cds,
         IReadOnlyList<GenericNameSyntax> enumInterfaces)
      {
         TypeDeclarationSyntax = cds ?? throw new ArgumentNullException(nameof(cds));
         EnumInterfaces = enumInterfaces ?? throw new ArgumentNullException(nameof(enumInterfaces));
      }
   }
}
