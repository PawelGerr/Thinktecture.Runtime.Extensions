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
         TypeDeclarationSyntax tds,
         [MaybeNullWhen(false)] out EnumDeclaration enumDeclaration)
      {
         if (tds.BaseList?.Types.Count > 0)
         {
            foreach (var baseType in tds.BaseList.Types)
            {
               if (IsEnumInterface(baseType, out var genericNameSyntax))
               {
                  enumDeclaration = new EnumDeclaration(tds, genericNameSyntax);
                  return true;
               }
            }
         }

         enumDeclaration = null;
         return false;
      }

      private static bool IsEnumInterface(BaseTypeSyntax? type, [MaybeNullWhen(false)] out GenericNameSyntax genNameSyntax)
      {
         if (type?.Type is GenericNameSyntax genericNameSyntax)
         {
            if (genericNameSyntax.Identifier.Text == "IEnum" &&
                genericNameSyntax.TypeArgumentList.Arguments.Count == 1)
            {
               genNameSyntax = genericNameSyntax;
               return true;
            }
         }

         genNameSyntax = null;
         return false;
      }
   }
}
