using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectSourceGeneratorState : ITypeInformation, IEquatable<ValueObjectSourceGeneratorState>
{
   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated { get; }
   public string TypeMinimallyQualified { get; }

   public string? Namespace { get; }
   public string Name { get; }
   public bool IsReferenceType { get; }

   public string? FactoryValidationReturnType { get; }

   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public IReadOnlyList<EqualityInstanceMemberInfo> EqualityMembers { get; }

   [MemberNotNullWhen(true, nameof(KeyMember))]
   public bool HasKeyMember => EqualityMembers.Count == 1
                               && AssignableInstanceFieldsAndProperties.Count == 1
                               && EqualityMembers[0].Member.Equals(AssignableInstanceFieldsAndProperties[0]);

   public EqualityInstanceMemberInfo? KeyMember => HasKeyMember ? EqualityMembers[0] : null;

   public ValueObjectSettings Settings { get; }

   public ValueObjectSourceGeneratorState(
      TypedMemberStateFactory factory,
      INamedTypeSymbol type,
      ValueObjectSettings settings,
      CancellationToken cancellationToken)
   {
      Settings = settings;
      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullable = $"{TypeFullyQualified}?";
      TypeFullyQualifiedNullAnnotated = type.IsReferenceType ? TypeFullyQualifiedNullable : TypeFullyQualified;
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;
      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, true, true, cancellationToken).ToList();
      EqualityMembers = GetEqualityMembers();

      var factoryValidationReturnType = type.GetMembers()
                                            .OfType<IMethodSymbol>()
                                            .FirstOrDefault(m => m.IsStatic && m.ReturnType.SpecialType != SpecialType.System_Void && m.Name == "ValidateFactoryArguments")?
                                            .ReturnType;

      if (factoryValidationReturnType is not null)
      {
         FactoryValidationReturnType = factoryValidationReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

         if (factoryValidationReturnType.NullableAnnotation == NullableAnnotation.Annotated)
            FactoryValidationReturnType += "?";
      }
   }

   private IReadOnlyList<EqualityInstanceMemberInfo> GetEqualityMembers()
   {
      var members = AssignableInstanceFieldsAndProperties;

      if (members.Count == 0)
         return Array.Empty<EqualityInstanceMemberInfo>();

      List<EqualityInstanceMemberInfo>? equalityMembers = null;

      for (var i = 0; i < members.Count; i++)
      {
         var member = members[i];
         var settings = member.ValueObjectMemberSettings;

         if (!settings.IsExplicitlyDeclared)
            continue;

         var equalityComparer = settings.HasInvalidEqualityComparerType ? null : settings.EqualityComparerAccessor;
         var comparer = settings.HasInvalidComparerType ? null : settings.ComparerAccessor;
         var equalityMember = new EqualityInstanceMemberInfo(member, equalityComparer, comparer);

         (equalityMembers ??= new List<EqualityInstanceMemberInfo>(members.Count)).Add(equalityMember);
      }

      if (equalityMembers != null)
         return equalityMembers;

      var equalityMembersArray = new EqualityInstanceMemberInfo[members.Count];

      for (var i = 0; i < members.Count; i++)
      {
         var memberInfo = members[i];
         var comparerAccessor = memberInfo.SpecialType == SpecialType.System_String ? Constants.ComparerAccessor.ORDINAL_IGNORE_CASE : null;
         var equalityMemberInfo = new EqualityInstanceMemberInfo(memberInfo, comparerAccessor, comparerAccessor);

         equalityMembersArray[i] = equalityMemberInfo;
      }

      return equalityMembersArray;
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
             && FactoryValidationReturnType == other.FactoryValidationReturnType
             && Settings.Equals(other.Settings)
             && AssignableInstanceFieldsAndProperties.EqualsTo(other.AssignableInstanceFieldsAndProperties)
             && EqualityMembers.EqualsTo(other.EqualityMembers);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ FactoryValidationReturnType?.GetHashCode() ?? 0;
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ EqualityMembers.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();

         return hashCode;
      }
   }
}
