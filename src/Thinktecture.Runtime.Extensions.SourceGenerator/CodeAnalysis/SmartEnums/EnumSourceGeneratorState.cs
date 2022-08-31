using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public class EnumSourceGeneratorState : ISourceGeneratorState, IEquatable<EnumSourceGeneratorState>
{
   private readonly INamedTypeSymbol _enumType;

   public string? Namespace { get; }
   public string EnumTypeFullyQualified { get; }
   public string EnumTypeMinimallyQualified { get; }

   public DefaultMemberState KeyProperty { get; }

   public bool IsValidatable { get; }

   public IBaseEnumState? BaseEnum { get; }
   public BaseTypeState? BaseType { get; }

   [MemberNotNullWhen(true, nameof(BaseEnum))]
   public bool HasBaseEnum => BaseEnum is not null;

   public bool HasCreateInvalidImplementation { get; }
   public bool IsReferenceType { get; }
   public bool IsAbstract { get; }
   public string Name { get; }
   public EnumSettings Settings { get; }

   public IReadOnlyList<string> ItemNames { get; }
   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public IReadOnlyList<string> FullyQualifiedDerivedTypes { get; }

   public AttributeInfo AttributeInfo { get; }

   public EnumSourceGeneratorState(
      INamedTypeSymbol type,
      ImmutableArray<INamedTypeSymbol> genericEnumTypes,
      INamedTypeSymbol enumInterface)
   {
      if (enumInterface is null)
         throw new ArgumentNullException(nameof(enumInterface));

      _enumType = type ?? throw new ArgumentNullException(nameof(type));

      Namespace = type.ContainingNamespace?.IsGlobalNamespace == true ? null : type.ContainingNamespace?.ToString();
      EnumTypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      EnumTypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
      Name = type.Name;

      IsReferenceType = type.IsReferenceType;
      IsAbstract = type.IsAbstract;
      IsValidatable = enumInterface.IsValidatableEnumInterface();
      Settings = new EnumSettings(type.FindEnumGenerationAttribute());

      BaseEnum = GetBaseEnum(type);
      BaseType = type.BaseType is null || type.BaseType.SpecialType == SpecialType.System_Object
                    ? null
                    : new BaseTypeState(type.BaseType, BaseEnum);

      var keyType = enumInterface.TypeArguments[0];
      KeyProperty = (BaseEnum?.Settings ?? Settings).CreateKeyProperty(keyType);
      HasCreateInvalidImplementation = type.HasCreateInvalidImplementation(keyType);

      ItemNames = type.EnumerateEnumItems().Select(i => i.Name).ToList();
      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true).ToList();
      FullyQualifiedDerivedTypes = type.FindDerivedInnerEnums()
                                       .Select(t => t.Type)
                                       .Concat(genericEnumTypes)
                                       .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                                       .Select(t => t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                                       .ToList();

      AttributeInfo = new AttributeInfo(type);
   }

   public Location GetFirstLocation()
   {
      return _enumType.DeclaringSyntaxReferences.First().GetSyntax().GetLocation();
   }

   private static IBaseEnumState? GetBaseEnum(INamedTypeSymbol type)
   {
      if (!type.BaseType.IsEnum(out var enumInterfaces))
         return null;

      var baseInterface = enumInterfaces.GetValidEnumInterface(type.BaseType);

      if (baseInterface is null)
         return null;

      return SymbolEqualityComparer.Default.Equals(type.ContainingAssembly, type.BaseType.ContainingAssembly)
                ? new SameAssemblyBaseEnumState(type.BaseType, baseInterface.TypeArguments[0])
                : new BaseEnumState(type.BaseType);
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

      return EnumTypeFullyQualified == other.EnumTypeFullyQualified
             && IsValidatable == other.IsValidatable
             && HasCreateInvalidImplementation == other.HasCreateInvalidImplementation
             && IsReferenceType == other.IsReferenceType
             && IsAbstract == other.IsAbstract
             && AttributeInfo.Equals(other.AttributeInfo)
             && KeyProperty.Equals(other.KeyProperty)
             && Equals(BaseEnum, other.BaseEnum)
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
         var hashCode = EnumTypeFullyQualified.GetHashCode();
         hashCode = (hashCode * 397) ^ IsValidatable.GetHashCode();
         hashCode = (hashCode * 397) ^ HasCreateInvalidImplementation.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsAbstract.GetHashCode();
         hashCode = (hashCode * 397) ^ AttributeInfo.GetHashCode();
         hashCode = (hashCode * 397) ^ KeyProperty.GetHashCode();
         hashCode = (hashCode * 397) ^ (BaseEnum?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ (BaseType?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ ItemNames.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();
         hashCode = (hashCode * 397) ^ FullyQualifiedDerivedTypes.ComputeHashCode();

         return hashCode;
      }
   }
}
