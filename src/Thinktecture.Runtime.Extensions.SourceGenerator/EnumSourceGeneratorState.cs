using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   public class EnumSourceGeneratorState
   {
      public GeneratorExecutionContext Context { get; }
      public SemanticModel Model { get; }

      public ClassDeclarationSyntax ClassDeclarationSyntax { get; }
      public GenericNameSyntax BaseType { get; }

      public string Namespace { get; }
      public INamedTypeSymbol ClassTypeInfo { get; }
      public ITypeSymbol KeyType { get; }
      public bool IsKeyARefType { get; }
      public SyntaxToken EnumType { get; }

      public string KeyComparerMember { get; }
      public bool NeedsDefaultComparer { get; }

      public string? NullableQuestionMark { get; }
      public string? NullableQuestionMarkKey { get; }

      public IReadOnlyList<FieldDeclarationSyntax> Items { get; }

      public AttributeSyntax? EnumSettings { get; }

      public EnumSourceGeneratorState(
         GeneratorExecutionContext context,
         SemanticModel model,
         EnumDeclaration enumDeclaration,
         INamedTypeSymbol classTypeInfo,
         ITypeSymbol keyType,
         bool isKeyARefType,
         IReadOnlyList<FieldDeclarationSyntax> items)
      {
         if (enumDeclaration is null)
            throw new ArgumentNullException(nameof(enumDeclaration));
         if (classTypeInfo is null)
            throw new ArgumentNullException(nameof(classTypeInfo));

         ClassDeclarationSyntax = enumDeclaration.ClassDeclarationSyntax;
         BaseType = enumDeclaration.BaseType;

         ClassTypeInfo = classTypeInfo;
         Namespace = classTypeInfo.ContainingNamespace.ToString();
         Context = context;
         Model = model;
         KeyType = keyType;
         IsKeyARefType = isKeyARefType;
         EnumType = enumDeclaration.ClassDeclarationSyntax.Identifier;

         NullableQuestionMark = context.Compilation.Options.NullableContextOptions == NullableContextOptions.Disable ? null : "?";
         NullableQuestionMarkKey = isKeyARefType ? NullableQuestionMark : null;

         Items = items;
         EnumSettings = enumDeclaration.ClassDeclarationSyntax.AttributeLists.SelectMany(a => a.Attributes).FirstOrDefault(a => model.GetTypeInfo(a).Type?.ToString() == "Thinktecture.EnumGenerationAttribute");

         KeyComparerMember = GetKeyComparerMember(EnumSettings, out var needsDefaultComparer);
         NeedsDefaultComparer = needsDefaultComparer;
      }

      private static string GetKeyComparerMember(AttributeSyntax? enumSettingsAttribute, out bool needsDefaultComparer)
      {
         var keyComparer = enumSettingsAttribute?.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.Identifier.ToString() == "KeyComparerProvidingMember");

         if (keyComparer is { Expression: LiteralExpressionSyntax les })
         {
            if (!String.IsNullOrWhiteSpace(les.Token.ValueText))
            {
               needsDefaultComparer = false;
               return les.Token.ValueText;
            }
         }

         needsDefaultComparer = true;
         return "_defaultKeyComparerMember";
      }
   }
}
