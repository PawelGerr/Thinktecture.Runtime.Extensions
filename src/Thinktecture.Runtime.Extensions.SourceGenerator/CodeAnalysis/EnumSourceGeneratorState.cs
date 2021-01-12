using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis
{
   public class EnumSourceGeneratorState
   {
      private readonly TypeDeclarationSyntax _declaration;

      public SemanticModel Model { get; }

      public string? Namespace { get; }
      public INamedTypeSymbol EnumType { get; }
      public ITypeSymbol KeyType { get; }
      public SyntaxToken EnumIdentifier => _declaration.Identifier;

      public string KeyPropertyName { get; }
      public string KeyArgumentName { get; }
      public string KeyComparerMember { get; }
      public bool NeedsDefaultComparer { get; }
      public bool IsValidatable { get; }

      public string? NullableQuestionMarkEnum { get; }
      public string? NullableQuestionMarkKey { get; }

      private IReadOnlyList<IFieldSymbol>? _items;
      public IReadOnlyList<IFieldSymbol> Items => _items ??= EnumType.GetEnumItems();

      private IReadOnlyList<InstanceMemberInfo>? _assignableInstanceFieldsAndProperties;
      public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties => _assignableInstanceFieldsAndProperties ??= EnumType.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true);

      public EnumSourceGeneratorState(
         SemanticModel model,
         TypeDeclarationSyntax enumDeclaration,
         INamedTypeSymbol enumType,
         INamedTypeSymbol enumInterface)
      {
         if (enumInterface is null)
            throw new ArgumentNullException(nameof(enumInterface));

         Model = model ?? throw new ArgumentNullException(nameof(model));

         _declaration = enumDeclaration ?? throw new ArgumentNullException(nameof(enumDeclaration));

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

      private static string GetKeyComparerMember(AttributeData? enumSettingsAttribute, out bool needsDefaultComparer)
      {
         var comparerMemberName = enumSettingsAttribute?.FindKeyComparer();

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
