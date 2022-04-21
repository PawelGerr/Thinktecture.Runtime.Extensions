using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class ValueObjectSourceGeneratorState : ISourceGeneratorState, IEquatable<ValueObjectSourceGeneratorState>
{
   private readonly INamedTypeSymbol _type;

   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }

   public string? Namespace { get; }
   public string? NullableQuestionMark => _type.IsReferenceType ? "?" : null;
   public string Name => _type.Name;
   public bool IsReferenceType => _type.IsReferenceType;

   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public IReadOnlyList<EqualityInstanceMemberInfo> EqualityMembers { get; }

   [MemberNotNullWhen(true, nameof(KeyMember))]
   public bool HasKeyMember => EqualityMembers.Count == 1
                               && AssignableInstanceFieldsAndProperties.Count == 1
                               && EqualityMembers[0].Member.Equals(AssignableInstanceFieldsAndProperties[0]);

   public EqualityInstanceMemberInfo? KeyMember => HasKeyMember ? EqualityMembers[0] : null;

   public ValueObjectSettings Settings { get; }

   public ValueObjectSourceGeneratorState(INamedTypeSymbol type, AttributeData valueObjectAttribute)
   {
      _type = type ?? throw new ArgumentNullException(nameof(type));

      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      AssignableInstanceFieldsAndProperties = _type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true);
      EqualityMembers = GetEqualityMembers();

      Settings = new ValueObjectSettings(valueObjectAttribute);
   }

   public Location GetFirstLocation()
   {
      return _type.DeclaringSyntaxReferences.First().GetSyntax().GetLocation();
   }

   private IReadOnlyList<EqualityInstanceMemberInfo> GetEqualityMembers()
   {
      var members = AssignableInstanceFieldsAndProperties;

      if (members.Count == 0)
         return Array.Empty<EqualityInstanceMemberInfo>();

      List<EqualityInstanceMemberInfo>? equalityMembers = null;

      foreach (var member in members)
      {
         var settings = member.ValueObjectMemberSettings;

         if (settings.IsExplicitlyDeclared)
         {
            var equalityComparer = settings.EqualityComparer;
            var comparer = settings.Comparer;
            var equalityMember = new EqualityInstanceMemberInfo(member, equalityComparer, comparer);

            (equalityMembers ??= new List<EqualityInstanceMemberInfo>()).Add(equalityMember);
         }
      }

      return equalityMembers ?? members.Select(m => new EqualityInstanceMemberInfo(m, null, null)).ToList();
   }

   public override bool Equals(object? obj)
   {
      return obj is ValueObjectSourceGeneratorState other && Equals(other);
   }

   public bool Equals(ValueObjectSourceGeneratorState? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && AssignableInstanceFieldsAndProperties.EqualsTo(other.AssignableInstanceFieldsAndProperties)
             && EqualityMembers.EqualsTo(other.EqualityMembers)
             && Settings.Equals(other.Settings);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ EqualityMembers.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();

         return hashCode;
      }
   }
}
