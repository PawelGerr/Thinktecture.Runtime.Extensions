namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SmartEnumSourceGeneratorState
   : ITypeInformation,
     IKeyedSerializerGeneratorTypeInformation,
     IParsableTypeInformation,
     IEquatable<SmartEnumSourceGeneratorState>
{
   public string? Namespace { get; }
   public string Name { get; }
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public ImmutableArray<ContainingTypeState> ContainingTypes { get; }

   public KeyMemberState? KeyMember { get; }
   public ValidationErrorState ValidationError { get; }
   public SmartEnumSettings Settings { get; }
   public BaseTypeState? BaseType { get; }
   public bool HasDerivedTypes { get; }

   public NullableAnnotation NullableAnnotation => NullableAnnotation.NotAnnotated; // Key members cannot be nullable
   public bool IsNullableStruct => false;                                           // Key members cannot be structs
   public bool IsReferenceType => true;                                             // Smart Enums cannot be structs
   public bool IsValueType => false;                                                // Smart Enums cannot be structs
   public bool DisallowsDefaultValue => false;                                      // null is allowed
   public bool IsEqualWithReferenceEquality => true;
   public bool IsRecord => false;
   public bool IsTypeParameter => false;
   public int NumberOfGenerics => 0;

   public EnumItems Items { get; }
   public ImmutableArray<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public ImmutableArray<DelegateMethodState> DelegateMethods { get; }

   public SmartEnumSourceGeneratorState(
      TypedMemberStateFactory factory,
      INamedTypeSymbol type,
      KeyMemberState? keyMember,
      ValidationErrorState validationError,
      SmartEnumSettings settings,
      bool hasDerivedTypes,
      CancellationToken cancellationToken)
   {
      KeyMember = keyMember;
      Settings = settings;
      HasDerivedTypes = hasDerivedTypes;
      ValidationError = validationError;

      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      ContainingTypes = type.GetContainingTypes();
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      BaseType = type.GetBaseType(factory);
      Items = new EnumItems(type.GetEnumItems());
      AssignableInstanceFieldsAndProperties = [..type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, true, false, cancellationToken)];
      DelegateMethods = type.GetDelegateMethods();
   }

   public override bool Equals(object? obj)
   {
      return obj is SmartEnumSourceGeneratorState other && Equals(other);
   }

   public bool Equals(SmartEnumSourceGeneratorState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && HasDerivedTypes == other.HasDerivedTypes
             && Equals(KeyMember, other.KeyMember)
             && ValidationError.Equals(other.ValidationError)
             && Settings.Equals(other.Settings)
             && BaseType == other.BaseType
             && Items.Equals(other.Items)
             && AssignableInstanceFieldsAndProperties.SequenceEqual(other.AssignableInstanceFieldsAndProperties)
             && ContainingTypes.SequenceEqual(other.ContainingTypes)
             && DelegateMethods.SequenceEqual(other.DelegateMethods);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ HasDerivedTypes.GetHashCode();
         hashCode = (hashCode * 397) ^ (KeyMember?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ BaseType.GetHashCode();
         hashCode = (hashCode * 397) ^ Items.GetHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ DelegateMethods.ComputeHashCode();

         return hashCode;
      }
   }
}
