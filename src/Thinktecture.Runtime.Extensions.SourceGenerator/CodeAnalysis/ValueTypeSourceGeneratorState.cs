using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Thinktecture.CodeAnalysis
{
   public class ValueTypeSourceGeneratorState
   {
      private readonly TypeDeclarationSyntax _declaration;

      public SemanticModel Model { get; }
      public INamedTypeSymbol Type { get; }
      public AttributeData ValueTypeAttribute { get; }
      public SyntaxToken TypeIdentifier => _declaration.Identifier;

      public string? Namespace { get; }
      public string? NullableQuestionMark => Type.IsReferenceType ? "?" : null;

      private IReadOnlyList<InstanceMemberInfo>? _assignableInstanceFieldsAndProperties;
      public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties => _assignableInstanceFieldsAndProperties ??= Type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true);

      private IReadOnlyList<InstanceMemberInfo>? _equalityMembers;
      public IReadOnlyList<InstanceMemberInfo> EqualityMembers => _equalityMembers ??= GetEqualityMembers();

      [MemberNotNullWhen(true, nameof(KeyMember))]
      public bool HasKeyMember => AssignableInstanceFieldsAndProperties.Count == 1;

      public InstanceMemberInfo? KeyMember => HasKeyMember ? AssignableInstanceFieldsAndProperties[0] : null;

      public ValueTypeSourceGeneratorState(SemanticModel model, TypeDeclarationSyntax declaration, INamedTypeSymbol type, AttributeData valueTypeAttribute)
      {
         Model = model ?? throw new ArgumentNullException(nameof(model));
         _declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
         Type = type ?? throw new ArgumentNullException(nameof(type));
         ValueTypeAttribute = valueTypeAttribute;
         Namespace = type.ContainingNamespace.ToString();
      }

      private IReadOnlyList<InstanceMemberInfo> GetEqualityMembers()
      {
         return AssignableInstanceFieldsAndProperties;
      }
   }
}
