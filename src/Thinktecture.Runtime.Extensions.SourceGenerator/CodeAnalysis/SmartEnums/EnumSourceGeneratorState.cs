using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class EnumSourceGeneratorState : ITypeInformation, IEquatable<EnumSourceGeneratorState>
{
   internal const string KEY_EQUALITY_COMPARER_NAME = "KeyEqualityComparer";

   public string? Namespace { get; }
   public string Name { get; }
   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullAnnotated { get; }
   public string TypeMinimallyQualified { get; }

   public IMemberState KeyProperty { get; }
   public bool IsValidatable { get; }
   public BaseTypeState? BaseType { get; }
   public bool SkipToString { get; }

   public bool HasCreateInvalidItemImplementation { get; }
   public bool HasKeyComparerImplementation { get; }
   public bool IsReferenceType { get; }
   public bool IsAbstract { get; }

   private string? _argumentName;
   public string ArgumentName => _argumentName ??= Name.MakeArgumentName();

   public IReadOnlyList<string> ItemNames { get; }
   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }

   public AttributeInfo AttributeInfo { get; }

   public EnumSourceGeneratorState(
      TypedMemberStateFactory factory,
      INamedTypeSymbol type,
      IMemberState keyProperty,
      bool skipToString,
      bool isValidatable,
      bool hasCreateInvalidItemImplementation,
      CancellationToken cancellationToken)
   {
      KeyProperty = keyProperty;
      SkipToString = skipToString;
      IsValidatable = isValidatable;
      HasCreateInvalidItemImplementation = hasCreateInvalidItemImplementation;

      Name = type.Name;
      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullAnnotated = type.IsReferenceType ? $"{TypeFullyQualified}?" : TypeFullyQualified;
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      IsReferenceType = type.IsReferenceType;
      IsAbstract = type.IsAbstract;

      BaseType = type.GetBaseType(factory);
      HasKeyComparerImplementation = HasHasKeyComparerImplementation(type);
      ItemNames = type.EnumerateEnumItems().Select(i => i.Name).ToList();
      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(factory, true, cancellationToken).ToList();

      AttributeInfo = new AttributeInfo(type);
   }

   private static bool HasHasKeyComparerImplementation(INamedTypeSymbol enumType)
   {
      foreach (var member in enumType.GetMembers())
      {
         if (member is not IPropertySymbol property)
            continue;

         if (member.IsStatic && property.Name == KEY_EQUALITY_COMPARER_NAME)
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
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualified == other.TypeFullyQualified
             && IsValidatable == other.IsValidatable
             && HasCreateInvalidItemImplementation == other.HasCreateInvalidItemImplementation
             && HasKeyComparerImplementation == other.HasKeyComparerImplementation
             && IsReferenceType == other.IsReferenceType
             && IsAbstract == other.IsAbstract
             && AttributeInfo.Equals(other.AttributeInfo)
             && KeyProperty.Equals(other.KeyProperty)
             && Equals(BaseType, other.BaseType)
             && ItemNames.EqualsTo(other.ItemNames)
             && AssignableInstanceFieldsAndProperties.EqualsTo(other.AssignableInstanceFieldsAndProperties);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValidatable.GetHashCode();
         hashCode = (hashCode * 397) ^ HasCreateInvalidItemImplementation.GetHashCode();
         hashCode = (hashCode * 397) ^ HasKeyComparerImplementation.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsAbstract.GetHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ KeyProperty.GetHashCode();
         hashCode = (hashCode * 397) ^ (BaseType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ ItemNames.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();

         return hashCode;
      }
   }
}
