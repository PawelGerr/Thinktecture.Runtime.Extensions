namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class KeyedValueObjectSourceGeneratorState
   : ITypeInformation,
     IKeyedSerializerGeneratorTypeInformation,
     IParsableTypeInformation,
     IEquatable<KeyedValueObjectSourceGeneratorState>,
     IHasGenerics
{
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public bool DisallowsDefaultValue => IsValueType && (!Settings.AllowDefaultStructs || KeyMember.IsReferenceType);
   public ImmutableArray<ContainingTypeState> ContainingTypes { get; }

   public string? Namespace { get; }
   public string Name { get; }
   public bool IsReferenceType { get; }
   public bool IsValueType { get; }
   public ImmutableArray<GenericTypeParameterState> GenericParameters { get; }
   public int NumberOfGenerics => GenericParameters.Length;

   public NullableAnnotation NullableAnnotation => NullableAnnotation.NotAnnotated;
   public bool IsNullableStruct => false;
   public bool IsRecord => false;
   public bool IsTypeParameter => false;
   public bool IsEqualWithReferenceEquality => false;

   public string? FactoryValidationReturnType { get; }

   public KeyMemberState KeyMember { get; }
   public ValidationErrorState ValidationError { get; }
   public ValueObjectSettings Settings { get; }

   public KeyedValueObjectSourceGeneratorState(
      INamedTypeSymbol type,
      KeyMemberState keyMember,
      ValidationErrorState validationError,
      ValueObjectSettings settings)
   {
      KeyMember = keyMember;
      ValidationError = validationError;
      Settings = settings;
      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeMinimallyQualified = type.ToMinimallyQualifiedDisplayString();
      ContainingTypes = type.GetContainingTypes();
      IsReferenceType = type.IsReferenceType;
      IsValueType = type.IsValueType;
      FactoryValidationReturnType = type.GetValidateFactoryArgumentsReturnType();
      GenericParameters = type.GetGenericTypeParameters();
   }

   public override bool Equals(object? obj)
   {
      return obj is KeyedValueObjectSourceGeneratorState other && Equals(other);
   }

   public bool Equals(KeyedValueObjectSourceGeneratorState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsReferenceType == other.IsReferenceType
             && IsValueType == other.IsValueType
             && FactoryValidationReturnType == other.FactoryValidationReturnType
             && KeyMember.Equals(other.KeyMember)
             && ValidationError.Equals(other.ValidationError)
             && Settings.Equals(other.Settings)
             && ContainingTypes.SequenceEqual(other.ContainingTypes)
             && GenericParameters.SequenceEqual(other.GenericParameters);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValueType.GetHashCode();
         hashCode = (hashCode * 397) ^ (FactoryValidationReturnType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ KeyMember.GetHashCode();
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ GenericParameters.ComputeHashCode();

         return hashCode;
      }
   }
}
