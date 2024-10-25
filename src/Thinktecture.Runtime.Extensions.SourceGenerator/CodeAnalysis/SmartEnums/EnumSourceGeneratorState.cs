namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class EnumSourceGeneratorState : ITypeInformation, IEquatable<EnumSourceGeneratorState>
{
   public string? Namespace { get; }
   public string Name { get; }
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public bool IsEqualWithReferenceEquality => !Settings.IsValidatable;
   public IReadOnlyList<ContainingTypeState> ContainingTypes { get; }

   public KeyMemberState? KeyMember { get; }
   public ValidationErrorState ValidationError { get; }
   public EnumSettings Settings { get; }
   public BaseTypeState? BaseType { get; }

   public bool HasCreateInvalidItemImplementation { get; }
   public bool HasDerivedTypes { get; }
   public bool IsReferenceType { get; }
   public NullableAnnotation NullableAnnotation { get; }
   public bool IsNullableStruct { get; }
   public bool IsAbstract { get; }

   public EnumItems Items { get; }
   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }

   public EnumSourceGeneratorState(
      TypedMemberStateFactory factory,
      INamedTypeSymbol type,
      KeyMemberState? keyMember,
      ValidationErrorState validationError,
      ImmutableArray<ISymbol> nonIgnoredMembers,
      EnumSettings settings,
      bool hasCreateInvalidItemImplementation,
      bool hasDerivedTypes,
      CancellationToken cancellationToken)
   {
      KeyMember = keyMember;
      Settings = settings;
      HasCreateInvalidItemImplementation = hasCreateInvalidItemImplementation;
      HasDerivedTypes = hasDerivedTypes;
      ValidationError = validationError;

      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      ContainingTypes = type.GetContainingTypes();
      TypeFullyQualified = type.ToFullyQualifiedDisplayString();
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;
      NullableAnnotation = type.NullableAnnotation;
      IsNullableStruct = type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
      IsAbstract = type.IsAbstract;

      BaseType = type.GetBaseType(factory);
      Items = new EnumItems(type.GetEnumItems(nonIgnoredMembers));
      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(nonIgnoredMembers, factory, true, false, cancellationToken).ToList();
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumSourceGeneratorState other && Equals(other);
   }

   public bool Equals(EnumSourceGeneratorState? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && HasCreateInvalidItemImplementation == other.HasCreateInvalidItemImplementation
             && HasDerivedTypes == other.HasDerivedTypes
             && IsReferenceType == other.IsReferenceType
             && NullableAnnotation == other.NullableAnnotation
             && IsAbstract == other.IsAbstract
             && Equals(KeyMember, other.KeyMember)
             && ValidationError.Equals(other.ValidationError)
             && Settings.Equals(other.Settings)
             && Equals(BaseType, other.BaseType)
             && Items.Equals(other.Items)
             && AssignableInstanceFieldsAndProperties.SequenceEqual(other.AssignableInstanceFieldsAndProperties)
             && ContainingTypes.SequenceEqual(other.ContainingTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ HasCreateInvalidItemImplementation.GetHashCode();
         hashCode = (hashCode * 397) ^ HasDerivedTypes.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)NullableAnnotation;
         hashCode = (hashCode * 397) ^ IsAbstract.GetHashCode();
         hashCode = (hashCode * 397) ^ (KeyMember?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ (BaseType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ Items.GetHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();
         hashCode = (hashCode * 397) ^ ContainingTypes.ComputeHashCode();

         return hashCode;
      }
   }
}
