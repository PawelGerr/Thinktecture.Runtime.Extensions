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
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }

   public KeyMemberState? KeyMember { get; }
   public ValidationErrorState ValidationError { get; }
   public SmartEnumSettings Settings { get; }
   public BaseTypeState? BaseType { get; }

   public bool HasDerivedTypes { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }
   public bool IsAbstract { get; }

   public bool IsReferenceType => true;        // Smart Enums cannot be structs
   public bool IsStruct => false;              // Smart Enums cannot be structs
   public bool DisallowsDefaultValue => false; // Smart Enums cannot be structs
   public bool IsEqualWithReferenceEquality => true;
   public bool IsRecord => false;
   public bool IsTypeParameter => false;
   public int NumberOfGenerics => 0;

   public EnumItems Items { get; }
   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public IReadOnlyList<DelegateMethodState> DelegateMethods { get; }

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
      NullableAnnotation = type.NullableAnnotation;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
      IsAbstract = type.IsAbstract;

      BaseType = type.GetBaseType(factory);
      Items = new EnumItems(type.GetEnumItems());
      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, true, false, cancellationToken).ToList();
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
             && NullableAnnotation == other.NullableAnnotation
             && IsAbstract == other.IsAbstract
             && Equals(KeyMember, other.KeyMember)
             && ValidationError.Equals(other.ValidationError)
             && Settings.Equals(other.Settings)
             && Equals(BaseType, other.BaseType)
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
         hashCode = (hashCode * 397) ^ (int)NullableAnnotation;
         hashCode = (hashCode * 397) ^ IsAbstract.GetHashCode();
         hashCode = (hashCode * 397) ^ (KeyMember?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ (BaseType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ Items.GetHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();
         hashCode = (hashCode * 397) ^ DelegateMethods.ComputeHashCode();

         return hashCode;
      }
   }
}
