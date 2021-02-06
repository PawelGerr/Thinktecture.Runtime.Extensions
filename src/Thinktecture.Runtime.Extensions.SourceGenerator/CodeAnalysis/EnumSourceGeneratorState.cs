using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis
{
   public class EnumSourceGeneratorState
   {
      public TypeDeclarationSyntax Declaration { get; }
      public SemanticModel Model { get; }

      public string? Namespace { get; }
      public INamedTypeSymbol EnumType { get; }
      public ITypeSymbol KeyType { get; }
      public SyntaxToken EnumIdentifier => Declaration.Identifier;

      public string RuntimeTypeName { get; }

      public string KeyPropertyName { get; private set; }
      public string KeyArgumentName { get; private set; }
      public string KeyComparerMember { get; private set; }
      public bool NeedsDefaultComparer { get; private set; }
      public bool IsExtensible { get; private set; }

      public bool IsValidatable { get; }

      private bool _isBaseItemDetermined;
      private IBaseEnumState? _baseEnum;

      public IBaseEnumState? BaseEnum
      {
         get
         {
            if (_isBaseItemDetermined)
               return _baseEnum;

            DetermineBaseEnum();

            return _baseEnum;
         }
      }

      [MemberNotNullWhen(true, nameof(BaseEnum))]
      public bool HasBaseEnum => BaseEnum is not null;

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

         Declaration = enumDeclaration ?? throw new ArgumentNullException(nameof(enumDeclaration));

         EnumType = enumType ?? throw new ArgumentNullException(nameof(enumType));
         Namespace = enumType.ContainingNamespace.ToString();
         KeyType = enumInterface.TypeArguments[0];
         IsValidatable = enumInterface.IsValidatableEnumInterface();

         NullableQuestionMarkEnum = EnumType.IsReferenceType ? "?" : null;
         NullableQuestionMarkKey = KeyType.IsReferenceType ? "?" : null;

         var enumSettings = enumType.FindEnumGenerationAttribute();

         InitializeFromSettings(enumSettings, false);
         IsExtensible = enumType.IsReferenceType && (enumSettings?.IsExtensible() ?? false);

         RuntimeTypeName = IsExtensible ? "{GetType().Name}" : Declaration.Identifier.ToString();
      }

      [MemberNotNull(nameof(KeyComparerMember), nameof(KeyPropertyName), nameof(KeyArgumentName))]
      private void InitializeFromSettings(AttributeData? enumSettings, bool isFromBaseEnum)
      {
         KeyComparerMember = GetKeyComparerMember(enumSettings, out var needsDefaultComparer);
         KeyPropertyName = GetKeyPropertyName(enumSettings);
         KeyArgumentName = KeyPropertyName.MakeArgumentName();
         NeedsDefaultComparer = !isFromBaseEnum && needsDefaultComparer;
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

      public void SetBaseType(EnumSourceGeneratorState other)
      {
         if (_baseEnum is SameAssemblyBaseEnumState)
            return;

         SetBaseTypeInternal(new SameAssemblyBaseEnumState(other));
      }

      private void DetermineBaseEnum()
      {
         _isBaseItemDetermined = true;

         if (EnumType.BaseType is null)
            return;

         if (!EnumType.BaseType.IsEnum(out var enumInterfaces))
            return;

         var baseInterface = enumInterfaces.GetValidEnumInterface(EnumType.BaseType);

         if (baseInterface is null)
            return;

         SetBaseTypeInternal(new BaseEnumState(EnumType.BaseType));
      }

      private void SetBaseTypeInternal(IBaseEnumState other)
      {
         _baseEnum = other;
         _isBaseItemDetermined = true;

         var enumSettings = other.Type.FindEnumGenerationAttribute();
         InitializeFromSettings(enumSettings, true);

         IsExtensible = false;
      }
   }
}
