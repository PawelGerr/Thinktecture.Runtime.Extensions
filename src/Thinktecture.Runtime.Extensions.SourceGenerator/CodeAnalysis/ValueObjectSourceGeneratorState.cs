using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class ValueObjectSourceGeneratorState : IEquatable<ValueObjectSourceGeneratorState>
{
   public INamedTypeSymbol Type { get; }
   public AttributeData ValueObjectAttribute { get; }

   public bool SkipFactoryMethods => ValueObjectAttribute.FindSkipFactoryMethods() ?? false;
   public bool NullInFactoryMethodsYieldsNull => ValueObjectAttribute.FindNullInFactoryMethodsYieldsNull() ?? false;
   public bool SkipCompareTo => ValueObjectAttribute.FindSkipCompareTo() ?? false;

   public string? Namespace { get; }
   public string? NullableQuestionMark => Type.IsReferenceType ? "?" : null;

   private IReadOnlyList<InstanceMemberInfo>? _assignableInstanceFieldsAndProperties;
   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties => _assignableInstanceFieldsAndProperties ??= Type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true);

   private IReadOnlyList<EqualityInstanceMemberInfo>? _equalityMembers;
   public IReadOnlyList<EqualityInstanceMemberInfo> EqualityMembers => _equalityMembers ??= GetEqualityMembers();

   [MemberNotNullWhen(true, nameof(KeyMember))]
   public bool HasKeyMember => EqualityMembers.Count == 1 &&
                               AssignableInstanceFieldsAndProperties.Count == 1 &&
                               SymbolEqualityComparer.Default.Equals(EqualityMembers[0].Member.Symbol, AssignableInstanceFieldsAndProperties[0].Symbol);

   public EqualityInstanceMemberInfo? KeyMember => HasKeyMember ? EqualityMembers[0] : null;

   public ValueObjectSourceGeneratorState(INamedTypeSymbol type, AttributeData valueObjectAttribute)
   {
      Type = type ?? throw new ArgumentNullException(nameof(type));
      ValueObjectAttribute = valueObjectAttribute;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
   }

   private IReadOnlyList<EqualityInstanceMemberInfo> GetEqualityMembers()
   {
      var members = AssignableInstanceFieldsAndProperties;

      if (members.Count == 0)
         return Array.Empty<EqualityInstanceMemberInfo>();

      List<EqualityInstanceMemberInfo>? equalityMembers = null;

      foreach (var member in members)
      {
         var attribute = member.Symbol.FindValueObjectEqualityMemberAttribute();

         if (attribute is not null)
         {
            var equalityComparer = attribute.FindEqualityComparer().TrimAndNullify();
            var comparer = attribute.FindComparer().TrimAndNullify();
            var equalityMember = new EqualityInstanceMemberInfo(member, equalityComparer, comparer);

            (equalityMembers ??= new List<EqualityInstanceMemberInfo>()).Add(equalityMember);
         }
      }

      return equalityMembers ?? members.Select(m => new EqualityInstanceMemberInfo(m, null, null)).ToList();
   }

   public bool Equals(ValueObjectSourceGeneratorState? other)
   {
      return SymbolEqualityComparer.Default.Equals(Type, other?.Type);
   }

   public override bool Equals(object? obj)
   {
      if (ReferenceEquals(null, obj))
         return false;
      if (ReferenceEquals(this, obj))
         return true;
      if (obj.GetType() != GetType())
         return false;
      return Equals((ValueObjectSourceGeneratorState)obj);
   }

   public override int GetHashCode()
   {
      return SymbolEqualityComparer.Default.GetHashCode(Type);
   }
}
