using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class EnumSourceGeneratorState :
   ISourceGeneratorState,
   IEquatable<EnumSourceGeneratorState>
{
   internal const string KEY_EQUALITY_COMPARER_NAME = "KeyEqualityComparer";

   private readonly INamedTypeSymbol _enumType;

   public string? Namespace { get; }
   public string TypeFullyQualified { get; }
   public string TypeFullyQualifiedNullAnnotated { get; }

   private string? _typeMinimallyQualified;
   public string TypeMinimallyQualified => _typeMinimallyQualified ??= _enumType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

   public IMemberState KeyProperty { get; }

   public bool IsValidatable { get; }

   public BaseTypeState? BaseType { get; }

   public bool HasCreateInvalidImplementation { get; }
   public bool HasKeyComparerImplementation { get; }
   public bool IsReferenceType { get; }
   public bool IsAbstract { get; }
   public string Name => _enumType.Name;
   public EnumSettings Settings { get; }

   public IReadOnlyList<string> ItemNames { get; }
   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public IReadOnlyList<string> FullyQualifiedDerivedTypes { get; }

   public AttributeInfo AttributeInfo { get; }

   public EnumSourceGeneratorState(
      INamedTypeSymbol type,
      INamedTypeSymbol enumInterface,
      CancellationToken cancellationToken)
   {
      if (enumInterface is null)
         throw new ArgumentNullException(nameof(enumInterface));

      _enumType = type ?? throw new ArgumentNullException(nameof(type));

      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedNullAnnotated = type.IsReferenceType ? $"{TypeFullyQualified}?" : TypeFullyQualified;

      IsReferenceType = type.IsReferenceType;
      IsAbstract = type.IsAbstract;
      IsValidatable = enumInterface.IsValidatableEnumInterface();
      Settings = new EnumSettings(type.FindEnumGenerationAttribute());

      BaseType = type.GetBaseType();

      var keyType = enumInterface.TypeArguments[0];
      KeyProperty = Settings.CreateKeyProperty(keyType);
      HasCreateInvalidImplementation = type.HasCreateInvalidImplementation(keyType, cancellationToken);
      HasKeyComparerImplementation = HasHasKeyComparerImplementation(type);

      ItemNames = type.EnumerateEnumItems().Select(i => i.Name).ToList();
      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true, cancellationToken).ToList();
      FullyQualifiedDerivedTypes = type.FindDerivedInnerEnums()
                                       .Select(t => t.Type)
                                       .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                                       .Select(t => t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                                       .ToList();

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

   public Location GetFirstLocation(CancellationToken cancellationToken)
   {
      return _enumType.DeclaringSyntaxReferences.First().GetSyntax(cancellationToken).GetLocation();
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
             && HasCreateInvalidImplementation == other.HasCreateInvalidImplementation
             && HasKeyComparerImplementation == other.HasKeyComparerImplementation
             && IsReferenceType == other.IsReferenceType
             && IsAbstract == other.IsAbstract
             && AttributeInfo.Equals(other.AttributeInfo)
             && KeyProperty.Equals(other.KeyProperty)
             && Equals(BaseType, other.BaseType)
             && Settings.Equals(other.Settings)
             && ItemNames.EqualsTo(other.ItemNames)
             && AssignableInstanceFieldsAndProperties.EqualsTo(other.AssignableInstanceFieldsAndProperties)
             && FullyQualifiedDerivedTypes.EqualsTo(other.FullyQualifiedDerivedTypes);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValidatable.GetHashCode();
         hashCode = (hashCode * 397) ^ HasCreateInvalidImplementation.GetHashCode();
         hashCode = (hashCode * 397) ^ HasKeyComparerImplementation.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsAbstract.GetHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ KeyProperty.GetHashCode();
         hashCode = (hashCode * 397) ^ (BaseType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ ItemNames.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();
         hashCode = (hashCode * 397) ^ FullyQualifiedDerivedTypes.ComputeHashCode();

         return hashCode;
      }
   }

   public ImmutableArray<IInterfaceCodeGenerator> GetInterfaceCodeGenerators()
   {
      var generators = ImmutableArray<IInterfaceCodeGenerator>.Empty;

      if (!Settings.SkipIFormattable && KeyProperty.IsFormattable)
         generators = generators.Add(FormattableCodeGenerator.Instance);

      if (!Settings.SkipIComparable && KeyProperty.IsComparable)
         generators = generators.Add(ComparableCodeGenerator.Default);

      if (!Settings.SkipIParsable && (KeyProperty.IsString() || KeyProperty.IsParsable))
         generators = generators.Add(IsValidatable ? ParsableCodeGenerator.InstanceForValidatableEnum : ParsableCodeGenerator.Instance);

      if (KeyProperty.HasComparisonOperators && ComparisonOperatorsCodeGenerator.TryGet(Settings.ComparisonOperators, null, out var comparisonOperatorsCodeGenerator))
         generators = generators.Add(comparisonOperatorsCodeGenerator);

      return generators;
   }
}
