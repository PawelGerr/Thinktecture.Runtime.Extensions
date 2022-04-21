using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public abstract class EnumSourceGeneratorStateBase<TBaseEnumExtension>
   : ISourceGeneratorState, IEquatable<EnumSourceGeneratorStateBase<TBaseEnumExtension>>, IComparable<EnumSourceGeneratorStateBase<TBaseEnumExtension>>
   where TBaseEnumExtension : IEquatable<TBaseEnumExtension>
{
   private readonly INamedTypeSymbol _enumType;

   public string? Namespace { get; }
   public string EnumTypeFullyQualified { get; }
   public string EnumTypeMinimallyQualified { get; }

   public DefaultMemberState KeyProperty { get; }

   public bool IsValidatable { get; }

   public IBaseEnumState<TBaseEnumExtension>? BaseEnum { get; private set; }

   [MemberNotNullWhen(true, nameof(BaseEnum))]
   public bool HasBaseEnum => BaseEnum is not null;

   public bool HasCreateInvalidImplementation { get; }
   public bool IsReferenceType { get; }
   public bool IsAbstract { get; }
   public bool HasStructLayoutAttribute { get; }
   public string Name { get; }
   public EnumSettings Settings { get; }

   public IReadOnlyList<string> ItemNames { get; }
   public IReadOnlyList<InstanceMemberInfo> AssignableInstanceFieldsAndProperties { get; }
   public IReadOnlyList<string> FullyQualifiedDerivedTypes { get; }

   public EnumSourceGeneratorStateBase(
      INamedTypeSymbol type,
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
      HasStructLayoutAttribute = type.HasStructLayoutAttribute();
      IsValidatable = enumInterface.IsValidatableEnumInterface();
      Settings = new EnumSettings(type.FindEnumGenerationAttribute());

      var keyType = enumInterface.TypeArguments[0];
      HasCreateInvalidImplementation = type.HasCreateInvalidImplementation(keyType);

      DetermineBaseEnum();

      var keyPropertyName = GetKeyPropertyName(HasBaseEnum ? BaseEnum.Settings : Settings);
      KeyProperty = new DefaultMemberState(keyPropertyName, keyType, keyPropertyName.MakeArgumentName(), false);

      ItemNames = type.EnumerateEnumItems().Select(i => i.Name).ToList();
      AssignableInstanceFieldsAndProperties = type.GetAssignableFieldsAndPropertiesAndCheckForReadOnly(true);
      FullyQualifiedDerivedTypes = type.FindDerivedInnerTypes().Select(t => t.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)).ToList();
   }

   public Location GetFirstLocation()
   {
      return _enumType.DeclaringSyntaxReferences.First().GetSyntax().GetLocation();
   }

   private static string GetKeyPropertyName(EnumSettings enumSettingsAttribute)
   {
      var name = enumSettingsAttribute.KeyPropertyName;

      if (name is not null)
      {
         if (!StringComparer.OrdinalIgnoreCase.Equals(name, "item"))
            return name;
      }

      return "Key";
   }

   public void SetBaseType(EnumSourceGeneratorStateBase<TBaseEnumExtension> other)
   {
      if (BaseEnum is SameAssemblyBaseEnumState<TBaseEnumExtension>)
         return;

      BaseEnum = new SameAssemblyBaseEnumState<TBaseEnumExtension>(other, other._enumType, GetBaseEnumExtension(other._enumType));
   }

   protected abstract TBaseEnumExtension GetBaseEnumExtension(INamedTypeSymbol baseType);

   private void DetermineBaseEnum()
   {
      if (_enumType.BaseType is null)
         return;

      if (!_enumType.BaseType.IsEnum(out var enumInterfaces))
         return;

      var baseInterface = enumInterfaces.GetValidEnumInterface(_enumType.BaseType);

      if (baseInterface is null)
         return;

      BaseEnum = new BaseEnumState<TBaseEnumExtension>(_enumType.BaseType, GetBaseEnumExtension(_enumType.BaseType));
   }

   internal bool HasBaseType()
   {
      return _enumType.BaseType is not null && _enumType.BaseType.SpecialType != SpecialType.System_Object;
   }

   internal bool IsBaseType(EnumSourceGeneratorStateBase<TBaseEnumExtension> baseTypeCandidate)
   {
      return SymbolEqualityComparer.Default.Equals(_enumType.BaseType, baseTypeCandidate._enumType);
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumSourceGeneratorStateBase<TBaseEnumExtension> other && Equals(other);
   }

   public bool Equals(EnumSourceGeneratorStateBase<TBaseEnumExtension>? other)
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
             && HasStructLayoutAttribute == other.HasStructLayoutAttribute
             && KeyProperty.Equals(other.KeyProperty)
             && Equals(BaseEnum, other.BaseEnum)
             && Settings.Equals(other.Settings)
             && ItemNames.EqualsTo(other.ItemNames)
             && AssignableInstanceFieldsAndProperties.EqualsTo(other.AssignableInstanceFieldsAndProperties)
             && FullyQualifiedDerivedTypes.Equals(other.FullyQualifiedDerivedTypes);
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
         hashCode = (hashCode * 397) ^ HasStructLayoutAttribute.GetHashCode();
         hashCode = (hashCode * 397) ^ KeyProperty.GetHashCode();
         hashCode = (hashCode * 397) ^ (BaseEnum?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ Settings.GetHashCode();
         hashCode = (hashCode * 397) ^ ItemNames.ComputeHashCode();
         hashCode = (hashCode * 397) ^ AssignableInstanceFieldsAndProperties.ComputeHashCode();
         hashCode = (hashCode * 397) ^ FullyQualifiedDerivedTypes.ComputeHashCode();

         return hashCode;
      }
   }

   public int CompareTo(EnumSourceGeneratorStateBase<TBaseEnumExtension>? other)
   {
      if (other is null || SymbolEqualityComparer.Default.Equals(_enumType.BaseType, other._enumType))
         return 1;

      if (SymbolEqualityComparer.Default.Equals(other._enumType.BaseType, _enumType))
         return -1;

      if (_enumType.BaseType is null)
         return other._enumType.BaseType is null ? 0 : -1;

      return other._enumType.BaseType is null ? 1 : 0;
   }
}
