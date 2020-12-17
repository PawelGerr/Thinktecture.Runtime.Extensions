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
         if (syntaxNode is TypeDeclarationSyntax tds)
         {
            if (tds.IsAbstract())
               return;

            if (IsEnum(tds, out var enumDeclaration))
               Enums.Add(enumDeclaration);
         }
      }

      private static bool IsEnum(
         TypeDeclarationSyntax cds,
         [MaybeNullWhen(false)] out EnumDeclaration enumDeclaration)
      {
         if (cds.BaseList?.Types.Count > 0)
         {
            foreach (var type in cds.BaseList.Types)
            {
               if (type.Type is GenericNameSyntax genNameSyntax)
               {
                  if (genNameSyntax.Identifier.Text == "IEnum" &&
                      genNameSyntax.TypeArgumentList.Arguments.Count == 1)
                  {
                     enumDeclaration = new EnumDeclaration(cds, genNameSyntax);
                     return true;
                  }
               }
            }
         }

         enumDeclaration = null;
         return false;
      }
   }
}
