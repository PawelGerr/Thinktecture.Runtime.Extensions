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

      public TypeDeclarationSyntax TypeDeclarationSyntax { get; }
      public GenericNameSyntax BaseType { get; }

      public string Namespace { get; }
      public INamedTypeSymbol EnumTypeInfo { get; }
      public ITypeSymbol KeyType { get; }
      public bool IsKeyARefType { get; }
      public bool IsEnumARefType { get; }
      public SyntaxToken EnumIdentifier { get; }

      public string KeyPropertyName { get; }
      public string KeyComparerMember { get; }
      public bool NeedsDefaultComparer { get; }
      public bool IsValidatable { get; }

      public string? NullableQuestionMarkEnum { get; }
      public string? NullableQuestionMarkKey { get; }

      public IReadOnlyList<FieldDeclarationSyntax> Items { get; }

      public AttributeSyntax? EnumSettings { get; }

      public EnumSourceGeneratorState(
         GeneratorExecutionContext context,
         SemanticModel model,
         EnumDeclaration enumDeclaration,
         INamedTypeSymbol enumType,
         EnumInterfaceInfo enumInterfaceInfo,
         IReadOnlyList<FieldDeclarationSyntax> items)
      {
         if (enumDeclaration is null)
            throw new ArgumentNullException(nameof(enumDeclaration));
         if (enumInterfaceInfo is null)
            throw new ArgumentNullException(nameof(enumInterfaceInfo));

         TypeDeclarationSyntax = enumDeclaration.TypeDeclarationSyntax;
         BaseType = enumInterfaceInfo.Syntax;

         EnumTypeInfo = enumType ?? throw new ArgumentNullException(nameof(enumType));
         Namespace = enumType.ContainingNamespace.ToString();
         Context = context;
         Model = model ?? throw new ArgumentNullException(nameof(model));
         KeyType = enumInterfaceInfo.KeyType;
         IsKeyARefType = KeyType.TypeKind != TypeKind.Struct;
         EnumIdentifier = enumDeclaration.TypeDeclarationSyntax.Identifier;
         IsEnumARefType = enumType.TypeKind != TypeKind.Struct;
         IsValidatable = enumInterfaceInfo.IsValidatable;

         NullableQuestionMarkEnum = IsEnumARefType ? "?" : null;
         NullableQuestionMarkKey = IsKeyARefType ? "?" : null;

         Items = items;
         EnumSettings = enumDeclaration.TypeDeclarationSyntax.AttributeLists.SelectMany(a => a.Attributes).FirstOrDefault(a => model.GetTypeInfo(a).Type?.ToString() == "Thinktecture.EnumGenerationAttribute");

         KeyComparerMember = GetKeyComparerMember(model, EnumSettings, out var needsDefaultComparer);
         KeyPropertyName = GetKeyPropertyName(model, EnumSettings);
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

      public IReadOnlyList<TypeDeclarationSyntax> FindDerivedTypes()
      {
         var types = new List<TypeDeclarationSyntax>();

         FindDerivedTypes(types, TypeDeclarationSyntax);

         return types;
      }

      private void FindDerivedTypes(
         List<TypeDeclarationSyntax> types,
         TypeDeclarationSyntax typeToAnalyse)
      {
         foreach (var member in typeToAnalyse.Members)
         {
            if (member is TypeDeclarationSyntax innerClass)
            {
               if (innerClass.BaseList?.Types.Count > 0)
               {
                  foreach (var baseType in innerClass.BaseList.Types)
                  {
                     var baseTypeInfo = Model.GetTypeInfo(baseType.Type).Type;

                     if (SymbolEqualityComparer.Default.Equals(baseTypeInfo, EnumTypeInfo))
                        types.Add(innerClass);
                  }
               }

               FindDerivedTypes(types, innerClass);
            }
         }
      }
   }
}
