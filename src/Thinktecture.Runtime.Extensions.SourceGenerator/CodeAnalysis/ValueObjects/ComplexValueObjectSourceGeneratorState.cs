namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectSourceGeneratorState : ITypeInformation, IEquatable<ComplexValueObjectSourceGeneratorState>
{
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public bool IsEqualWithReferenceEquality => false;
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }
   public IReadOnlyList<string> GenericsFullyQualified => [];

   public string? Namespace { get; }
   public string Name { get; }
   public bool IsReferenceType { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }

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
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      ContainingTypes = type.GetContainingTypes();
      IsReferenceType = type.IsReferenceType;
      NullableAnnotation = type.NullableAnnotation;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, true, true, cancellationToken).ToList();
      EqualityMembers = GetEqualityMembers();

      var factoryValidationReturnType = (type.GetMembers().FirstOrDefault(m => m.IsStatic && m.Name == Constants.Methods.VALIDATE_FACTORY_ARGUMENTS && m is IMethodSymbol method && method.ReturnType.SpecialType != SpecialType.System_Void) as IMethodSymbol)?.ReturnType;

      if (factoryValidationReturnType is not null)
         FactoryValidationReturnType = factoryValidationReturnType.ToFullyQualifiedDisplayString();
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
             && EqualityMembers.SequenceEqual(other.EqualityMembers)
             && ContainingTypes.SequenceEqual(other.ContainingTypes);
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
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}
