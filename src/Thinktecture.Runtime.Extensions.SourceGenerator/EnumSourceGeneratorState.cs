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

      public string KeyPropetyName { get; }
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

         ClassDeclarationSyntax = enumDeclaration.ClassDeclarationSyntax;
         BaseType = enumDeclaration.BaseType;

         ClassTypeInfo = classTypeInfo ?? throw new ArgumentNullException(nameof(classTypeInfo));
         Namespace = classTypeInfo.ContainingNamespace.ToString();
         Context = context;
         Model = model ?? throw new ArgumentNullException(nameof(model));
         KeyType = keyType;
         IsKeyARefType = isKeyARefType;
         EnumType = enumDeclaration.ClassDeclarationSyntax.Identifier;

         NullableQuestionMark = context.Compilation.Options.NullableContextOptions == NullableContextOptions.Disable ? null : "?";
         NullableQuestionMarkKey = isKeyARefType ? NullableQuestionMark : null;

         Items = items;
         EnumSettings = enumDeclaration.ClassDeclarationSyntax.AttributeLists.SelectMany(a => a.Attributes).FirstOrDefault(a => model.GetTypeInfo(a).Type?.ToString() == "Thinktecture.EnumGenerationAttribute");

         KeyComparerMember = GetKeyComparerMember(model, EnumSettings, out var needsDefaultComparer);
         KeyPropetyName = GetKeyPropertyName(model, EnumSettings);
         NeedsDefaultComparer = needsDefaultComparer;
      }

      private static string GetKeyComparerMember(SemanticModel model, AttributeSyntax? enumSettingsAttribute, out bool needsDefaultComparer)
      {
         var comparerMemberName = GetParameterValue(model, enumSettingsAttribute, "KeyComparerProvidingMember");

         needsDefaultComparer = comparerMemberName is null;
         return comparerMemberName ?? "_defaultKeyComparerMember";
      }

      private static string GetKeyPropertyName(SemanticModel model, AttributeSyntax? enumSettingsAttribute)
      {
         return GetParameterValue(model, enumSettingsAttribute, "KeyPropertyName") ?? "Key";
      }

      private static string? GetParameterValue(SemanticModel model, AttributeSyntax? enumSettingsAttribute, string parameterName)
      {
         var keyName = enumSettingsAttribute?.ArgumentList?.Arguments.FirstOrDefault(a => a.NameEquals?.Name.Identifier.ToString() == parameterName);

         if (keyName is not null)
         {
            var value = model.GetConstantValue(keyName.Expression);

            if (value.HasValue && value.Value is string name && !String.IsNullOrWhiteSpace(name))
               return name.Trim();
         }

         return null;
      }
   }
}
