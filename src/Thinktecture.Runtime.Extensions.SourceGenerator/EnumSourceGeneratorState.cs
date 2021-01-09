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
      public IReadOnlyList<IFieldSymbol> Items => _items ??= EnumType.GetEnumItems();

      private IReadOnlyList<EnumMemberInfo>? _assignableInstanceFieldsAndProperties;
      public IReadOnlyList<EnumMemberInfo> AssignableInstanceFieldsAndProperties => _assignableInstanceFieldsAndProperties ??= EnumType.GetAssignableInstanceFieldsAndProperties();

      public EnumSourceGeneratorState(
         SemanticModel model,
         TypeDeclarationSyntax enumDeclaration,
         INamedTypeSymbol enumType,
         INamedTypeSymbol enumInterface)
      {
         if (enumInterface is null)
            throw new ArgumentNullException(nameof(enumInterface));

         Model = model ?? throw new ArgumentNullException(nameof(model));

         _enumSyntax = enumDeclaration ?? throw new ArgumentNullException(nameof(enumDeclaration));

         EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
         Namespace = enumType.ContainingNamespace.ToString();
         KeyType = enumInterface.TypeArguments[0];
         IsValidatable = enumInterface.IsValidatableEnumInterface();

         NullableQuestionMarkEnum = EnumType.IsReferenceType ? "?" : null;
         NullableQuestionMarkKey = KeyType.IsReferenceType ? "?" : null;

         var enumSettings = enumType.FindEnumGenerationAttribute();

         KeyComparerMember = GetKeyComparerMember(enumSettings, out var needsDefaultComparer);
         KeyPropertyName = GetKeyPropertyName(enumSettings);
         KeyArgumentName = KeyPropertyName.MakeArgumentName();
         NeedsDefaultComparer = needsDefaultComparer;
      }

      public bool HasAttribute(string fullAttributeType)
      {
         return _enumSyntax.AttributeLists.SelectMany(a => a.Attributes).Any(a => Model.GetTypeInfo(a).Type?.ToString() == fullAttributeType);
      }

      private static string GetKeyComparerMember(AttributeData? enumSettingsAttribute, out bool needsDefaultComparer)
      {
         var comparerMemberName = enumSettingsAttribute?.FindKeyComparerProvidingMember();

         needsDefaultComparer = comparerMemberName is null;
         return comparerMemberName ?? "_defaultKeyComparerMember";
      }

      private static string GetKeyPropertyName(AttributeData? enumSettingsAttribute)
      {
         var name = enumSettingsAttribute?.FindKeyPropertyName();

         if (name is not null)
         {
            if (!StringComparer.OrdinalIgnoreCase.Equals(name, "item"))
               return name;
         }

         return "Key";
      }
   }
}
