namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectSourceGeneratorState : ITypeInformation, IEquatable<ComplexValueObjectSourceGeneratorState>
{
   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullable { get; }
   public string TypeFullyQualifiedNullAnnotated => IsReferenceType ? TypeFullyQualifiedNullable : TypeFullyQualified;
   public string TypeMinimallyQualified { get; }
   public bool IsEqualWithReferenceEquality => false;

   public string? Namespace { get; }
   public string Name { get; }
   public bool IsReferenceType { get; }

   public string? FactoryValidationReturnType { get; }

   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public IReadOnlyList<EqualityInstanceMemberInfo> EqualityMembers { get; }

   public ValidationErrorState ValidationError { get; }
   public ValueObjectSettings Settings { get; }

   public ComplexValueObjectSourceGeneratorState(
      TypedMemberStateFactory factory,
      INamedTypeSymbol type,
      ValidationErrorState validationError,
      ValueObjectSettings settings,
      CancellationToken cancellationToken)
   {
      ValidationError = validationError;
      Settings = settings;
      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullable = $"{TypeFullyQualified}?";
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;

      var nonIgnoredMembers = type.GetNonIgnoredMembers();
      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(nonIgnoredMembers, factory, true, true, cancellationToken).ToList();
      EqualityMembers = GetEqualityMembers();

      var factoryValidationReturnType = nonIgnoredMembers.IsDefaultOrEmpty
                                           ? null
                                           : (nonIgnoredMembers.FirstOrDefault(m => m.IsStatic && m.Name == Constants.Methods.VALIDATE_FACTORY_ARGUMENTS && m is IMethodSymbol method && method.ReturnType.SpecialType != SpecialType.System_Void) as IMethodSymbol)?.ReturnType;

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
         var equalityMember = new EqualityInstanceMemberInfo(member, equalityComparer);

         (equalityMembers ??= new List<EqualityInstanceMemberInfo>(members.Count)).Add(equalityMember);
      }

      if (equalityMembers != null)
         return equalityMembers;

      var equalityMembersArray = new EqualityInstanceMemberInfo[members.Count];

      for (var i = 0; i < members.Count; i++)
      {
         var memberInfo = members[i];
         var equalityMemberInfo = new EqualityInstanceMemberInfo(memberInfo, null);

         equalityMembersArray[i] = equalityMemberInfo;
      }

      return equalityMembersArray;
   }

   public override bool Equals(object? obj)
   {
      return obj is ComplexValueObjectSourceGeneratorState other && Equals(other);
   }

   public bool Equals(ComplexValueObjectSourceGeneratorState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && FactoryValidationReturnType == other.FactoryValidationReturnType
             && ValidationError.Equals(other.ValidationError)
             && Settings.Equals(other.Settings)
             && AssignableInstanceFieldsAndProperties.SequenceEqual(other.AssignableInstanceFieldsAndProperties)
             && EqualityMembers.SequenceEqual(other.EqualityMembers);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ FactoryValidationReturnType?.GetHashCode() ?? 0;
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ EqualityMembers.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();

         return hashCode;
      }
   }
}
