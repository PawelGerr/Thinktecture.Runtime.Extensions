namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectSourceGeneratorState
   : ITypeInformation,
     IParsableTypeInformation,
     IKeyedSerializerGeneratorTypeInformation,
     IHasGenerics,
     IEquatable<ComplexValueObjectSourceGeneratorState>
{
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public bool IsEqualWithReferenceEquality => false;
   public bool DisallowsDefaultValue => !IsReferenceType && !Settings.AllowDefaultStructs;
   public ImmutableArray<ContainingTypeState> ContainingTypes { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }
   public int NumberOfGenerics => GenericParameters.Length;

   public string? Namespace { get; }
   public string Name { get; }
   public bool IsReferenceType { get; }
   public bool IsValueType { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }

   public bool IsRecord => false;
   public bool IsTypeParameter => false;

   public string? FactoryValidationReturnType { get; }

   public ImmutableArray<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public ImmutableArray<EqualityInstanceMemberInfo> EqualityMembers { get; }

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
      GenericParameters = type.GetGenericTypeParameters();
      IsReferenceType = type.IsReferenceType;
      IsValueType = type.IsValueType;
      NullableAnnotation = type.NullableAnnotation;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

      AssignableInstanceFieldsAndProperties = [..type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, true, true, cancellationToken)];
      EqualityMembers = GetEqualityMembers();

      var factoryValidationReturnType = (type.GetMembers().FirstOrDefault(m => m.IsStatic && m.Name == Constants.Methods.VALIDATE_FACTORY_ARGUMENTS && m is IMethodSymbol method && method.ReturnType.SpecialType != SpecialType.System_Void) as IMethodSymbol)?.ReturnType;

      if (factoryValidationReturnType is not null)
         FactoryValidationReturnType = factoryValidationReturnType.ToFullyQualifiedDisplayString();
   }

   private ImmutableArray<EqualityInstanceMemberInfo> GetEqualityMembers()
   {
      var members = AssignableInstanceFieldsAndProperties;

      if (members.Length == 0)
         return [];

      ImmutableArray<EqualityInstanceMemberInfo>.Builder? equalityMembers = null;

      for (var i = 0; i < members.Length; i++)
      {
         var member = members[i];
         var settings = member.ValueObjectMemberSettings;

         if (!settings.IsExplicitlyDeclared)
            continue;

         var equalityComparer = settings.HasInvalidEqualityComparerType ? null : settings.EqualityComparerAccessor;
         var equalityMember = new EqualityInstanceMemberInfo(member, equalityComparer);

         (equalityMembers ??= ImmutableArray.CreateBuilder<EqualityInstanceMemberInfo>(members.Length)).Add(equalityMember);
      }

      if (equalityMembers != null)
         return equalityMembers.DrainToImmutable();

      var equalityMembersArray = ImmutableArray.CreateBuilder<EqualityInstanceMemberInfo>(members.Length);

      for (var i = 0; i < members.Length; i++)
      {
         var memberInfo = members[i];
         var equalityMemberInfo = new EqualityInstanceMemberInfo(memberInfo, null);

         equalityMembersArray.Add(equalityMemberInfo);
      }

      return equalityMembersArray.DrainToImmutable();
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
             && IsValueType == other.IsValueType
             && FactoryValidationReturnType == other.FactoryValidationReturnType
             && ValidationError.Equals(other.ValidationError)
             && Settings.Equals(other.Settings)
             && AssignableInstanceFieldsAndProperties.SequenceEqual(other.AssignableInstanceFieldsAndProperties)
             && EqualityMembers.SequenceEqual(other.EqualityMembers)
             && GenericParameters.SequenceEqual(other.GenericParameters)
             && ContainingTypes.SequenceEqual(other.ContainingTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValueType.GetHashCode();
         hashCode = (hashCode * 397) ^ (FactoryValidationReturnType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ EqualityMembers.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}
