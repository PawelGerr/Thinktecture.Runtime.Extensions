using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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

      public bool SkipFactoryMethods => ValueTypeAttribute.FindSkipFactoryMethods() ?? false;
      public bool SkipCompareTo => ValueTypeAttribute.FindSkipCompareTo() ?? false;

      public string? Namespace { get; }
      public string? NullableQuestionMark => Type.IsReferenceType ? "?" : null;

      private IReadOnlyList<InstanceMemberInfo>? _assignableInstanceFieldsAndProperties;
      public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties => _assignableInstanceFieldsAndProperties ??= Type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true);

      private IReadOnlyList<EqualityInstanceMemberInfo>? _equalityMembers;
      public IReadOnlyList<EqualityInstanceMemberInfo> EqualityMembers => _equalityMembers ??= GetEqualityMembers();

#pragma warning disable CS8775
      [MemberNotNullWhen(true, nameof(KeyMember))]
      public bool HasKeyMember => EqualityMembers.Count == 1 &&
                                  AssignableInstanceFieldsAndProperties.Count == 1 &&
                                  SymbolEqualityComparer.Default.Equals(EqualityMembers[0].Member.Symbol, AssignableInstanceFieldsAndProperties[0].Symbol);
#pragma warning restore CS8775

      public EqualityInstanceMemberInfo? KeyMember => HasKeyMember ? EqualityMembers[0] : null;

      public ValueTypeSourceGeneratorState(SemanticModel model, TypeDeclarationSyntax declaration, INamedTypeSymbol type, AttributeData valueTypeAttribute)
      {
         Model = model ?? throw new ArgumentNullException(nameof(model));
         _declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));
         Type = type ?? throw new ArgumentNullException(nameof(type));
         ValueTypeAttribute = valueTypeAttribute;
         Namespace = type.ContainingNamespace.ToString();
      }

      private IReadOnlyList<EqualityInstanceMemberInfo> GetEqualityMembers()
      {
         var members = AssignableInstanceFieldsAndProperties;

         if (members.Count == 0)
            return Array.Empty<EqualityInstanceMemberInfo>();

         List<EqualityInstanceMemberInfo>? equalityMembers = null;

         foreach (var member in members)
         {
            var attribute = member.Symbol.FindValueTypeEqualityMemberAttribute();

            if (attribute is not null)
            {
               var equalityComparer = attribute.FindEqualityComparer().TrimmAndNullify();
               var comparer = attribute.FindComparer().TrimmAndNullify();
               var equalityMember = new EqualityInstanceMemberInfo(member, equalityComparer, comparer);

               (equalityMembers ??= new List<EqualityInstanceMemberInfo>()).Add(equalityMember);
            }
         }

         return equalityMembers ?? members.Select(m => new EqualityInstanceMemberInfo(m, null, null)).ToList();
      }
   }
}
