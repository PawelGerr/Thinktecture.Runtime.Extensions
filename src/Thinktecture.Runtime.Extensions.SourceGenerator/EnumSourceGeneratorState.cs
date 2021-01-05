using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture
{
   public class EnumSourceGeneratorState
   {
      private readonly TypeDeclarationSyntax _enumSyntax;

      public GeneratorExecutionContext Context { get; }
      public SemanticModel Model { get; }

      public string? Namespace { get; }
      public INamedTypeSymbol EnumType { get; }
      public ITypeSymbol KeyType { get; }
      public SyntaxToken EnumIdentifier => _enumSyntax.Identifier;

      public string KeyPropertyName { get; }
      public string KeyArgumentName { get; }
      public string KeyComparerMember { get; }
      public bool NeedsDefaultComparer { get; }
      public bool IsValidatable { get; }

      public string? NullableQuestionMarkEnum { get; }
      public string? NullableQuestionMarkKey { get; }

      private IReadOnlyList<IFieldSymbol>? _items;
      public IReadOnlyList<IFieldSymbol> Items => _items ??= EnumType.GetValidItems();

      private IReadOnlyList<EnumMemberInfo>? _assignableInstanceFieldsAndProperties;
      public IReadOnlyList<EnumMemberInfo> AssignableInstanceFieldsAndProperties => _assignableInstanceFieldsAndProperties ??= EnumType.GetAssignableInstanceFieldsAndProperties();

      public AttributeSyntax? EnumSettings { get; }

      public EnumSourceGeneratorState(
         GeneratorExecutionContext context,
         SemanticModel model,
         EnumDeclaration enumDeclaration,
         INamedTypeSymbol enumType,
         INamedTypeSymbol enumInterface)
      {
         if (enumDeclaration is null)
            throw new ArgumentNullException(nameof(enumDeclaration));
         if (enumInterface is null)
            throw new ArgumentNullException(nameof(enumInterface));

         Context = context;
         Model = model ?? throw new ArgumentNullException(nameof(model));

         _enumSyntax = enumDeclaration.TypeDeclarationSyntax;

         EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
         Namespace = enumType.ContainingNamespace.ToString();
         KeyType = enumInterface.TypeArguments[0];
         IsValidatable = enumInterface.IsValidatableEnumInterface();

         NullableQuestionMarkEnum = EnumType.IsReferenceType ? "?" : null;
         NullableQuestionMarkKey = KeyType.IsReferenceType ? "?" : null;

         EnumSettings = enumDeclaration.TypeDeclarationSyntax.AttributeLists.SelectMany(a => a.Attributes).FirstOrDefault(a => ModelExtensions.GetTypeInfo(model, a).Type?.ToString() == "Thinktecture.EnumGenerationAttribute");

         KeyComparerMember = GetKeyComparerMember(model, EnumSettings, out var needsDefaultComparer);
         KeyPropertyName = GetKeyPropertyName(model, EnumSettings);
         KeyArgumentName = KeyPropertyName.MakeArgumentName();
         NeedsDefaultComparer = needsDefaultComparer;
      }

      public bool HasAttribute(string fullAttributeType)
      {
         return _enumSyntax.AttributeLists.SelectMany(a => a.Attributes).Any(a => Model.GetTypeInfo(a).Type?.ToString() == fullAttributeType);
      }

      private static string GetKeyComparerMember(SemanticModel model, AttributeSyntax? enumSettingsAttribute, out bool needsDefaultComparer)
      {
         var comparerMemberName = GetParameterValue(model, enumSettingsAttribute, "KeyComparerProvidingMember");

         needsDefaultComparer = comparerMemberName is null;
         return comparerMemberName ?? "_defaultKeyComparerMember";
      }

      private string GetKeyPropertyName(SemanticModel model, AttributeSyntax? enumSettingsAttribute)
      {
         var name = GetParameterValue(model, enumSettingsAttribute, "KeyPropertyName");

         if (name is not null)
         {
            if (!StringComparer.OrdinalIgnoreCase.Equals(name, "item"))
               return name;

            Context.ReportDiagnostic(Diagnostic.Create(DiagnosticsDescriptors.KeyPropertyNameNotAllowed,
                                                       enumSettingsAttribute?.GetLocation() ?? _enumSyntax.GetLocation(),
                                                       name));
         }

         return "Key";
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

         FindDerivedTypes(types, _enumSyntax);

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

                     if (SymbolEqualityComparer.Default.Equals(baseTypeInfo, EnumType))
                        types.Add(innerClass);
                  }
               }

               FindDerivedTypes(types, innerClass);
            }
         }
      }
   }
}
