namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class EnumSourceGeneratorState : ITypeInformation, IEquatable<EnumSourceGeneratorState>
{
   public string? Namespace { get; }
   public string Name { get; }
   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullAnnotated { get; }
   public string TypeMinimallyQualified { get; }
   public bool IsEqualWithReferenceEquality => !Settings.IsValidatable;

   public IMemberState KeyProperty { get; }
   public ValidationErrorState ValidationError { get; }
   public EnumSettings Settings { get; }
   public BaseTypeState? BaseType { get; }

   public bool HasCreateInvalidItemImplementation { get; }
   public bool HasKeyComparerImplementation { get; }
   public bool IsReferenceType { get; }
   public bool IsAbstract { get; }

   private ArgumentName? _argumentName;
   public ArgumentName ArgumentName => _argumentName ??= Name.MakeArgumentName();

   public EnumItemNames ItemNames { get; }
   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }

   public EnumSourceGeneratorState(
      TypedMemberStateFactory factory,
      INamedTypeSymbol type,
      IMemberState keyProperty,
      ValidationErrorState validationError,
      ImmutableArray<ISymbol> nonIgnoredMembers,
      EnumSettings settings,
      bool hasCreateInvalidItemImplementation, CancellationToken cancellationToken)
   {
      KeyProperty = keyProperty;
      Settings = settings;
      HasCreateInvalidItemImplementation = hasCreateInvalidItemImplementation;
      ValidationError = validationError;

      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullAnnotated = type.IsReferenceType ? $"{TypeFullyQualified}?" : TypeFullyQualified;
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;
      IsAbstract = type.IsAbstract;

      BaseType = type.GetBaseType(factory);
      HasKeyComparerImplementation = HasHasKeyComparerImplementation(nonIgnoredMembers);
      ItemNames = new EnumItemNames(type.GetEnumItems(nonIgnoredMembers));
      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(nonIgnoredMembers, factory, true, false, cancellationToken).ToList();
   }

   private static bool HasHasKeyComparerImplementation(ImmutableArray<ISymbol> nonIgnoredMembers)
   {
      foreach (var member in nonIgnoredMembers)
      {
         if (member is not IPropertySymbol property)
            continue;

         if (member.IsStatic && property.Name == Constants.KEY_EQUALITY_COMPARER_NAME)
            return true;
      }

      return false;
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
             && HasKeyComparerImplementation == other.HasKeyComparerImplementation
             && IsReferenceType == other.IsReferenceType
             && IsAbstract == other.IsAbstract
             && KeyProperty.Equals(other.KeyProperty)
             && ValidationError.Equals(other.ValidationError)
             && Settings.Equals(other.Settings)
             && Equals(BaseType, other.BaseType)
             && ItemNames.Equals(other.ItemNames)
             && AssignableInstanceFieldsAndProperties.SequenceEqual(other.AssignableInstanceFieldsAndProperties);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ HasCreateInvalidItemImplementation.GetHashCode();
         hashCode = (hashCode * 397) ^ HasKeyComparerImplementation.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsAbstract.GetHashCode();
         hashCode = (hashCode * 397) ^ KeyProperty.GetHashCode();
         hashCode = (hashCode * 397) ^ ValidationError.GetHashCode();
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ (BaseType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ ItemNames.GetHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();

         return hashCode;
      }
   }
}
