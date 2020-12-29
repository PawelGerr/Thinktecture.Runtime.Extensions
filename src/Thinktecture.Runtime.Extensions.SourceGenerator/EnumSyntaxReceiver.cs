using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            if (IsEnum(tds, out var enumDeclaration))
               Enums.Add(enumDeclaration);
         }
      }

      private static bool IsEnum(
         TypeDeclarationSyntax tds,
         [MaybeNullWhen(false)] out EnumDeclaration enumDeclaration)
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

         enumDeclaration = null;
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
