using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis.SmartEnums;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.CodeAnalysis;

public class InstanceMemberInfo : IMemberState, IEquatable<InstanceMemberInfo>
{
   private readonly ITypeSymbol _type;
   private readonly SyntaxToken _identifier;

   public string TypeFullyQualifiedWithNullability { get; }
   public string TypeFullyQualified { get; }
   public string TypeMinimallyQualified { get; }
   public NullableAnnotation NullableAnnotation => _type.NullableAnnotation;

   public string Name => _identifier.Text;
   public Accessibility ReadAccessibility { get; }
   public string ArgumentName { get; }
   public bool IsStatic { get; }
   public bool IsReferenceType => _type.IsReferenceType;
   public bool IsReferenceTypeOrNullableStruct => _type.IsReferenceType || _type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
   public bool IsNullableStruct => _type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;
   public string? NullableQuestionMark => IsReferenceType ? "?" : null;
   public SpecialType SpecialType => _type.SpecialType;
   public bool IsFormattable { get; }
   public bool IsComparable { get; }

   public EnumMemberSettings EnumMemberSettings { get; }
   public ValueObjectMemberSettings ValueObjectMemberSettings { get; }

   private InstanceMemberInfo(
      ISymbol member,
      ITypeSymbol type,
      SyntaxToken identifier,
      Accessibility readAccessibility,
      bool isStatic)
   {
      _type = type;
      TypeFullyQualified = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
      TypeFullyQualifiedWithNullability = type.IsReferenceType && type.NullableAnnotation == NullableAnnotation.Annotated ? $"{TypeFullyQualified}?" : TypeFullyQualified;
      TypeMinimallyQualified = type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

      _identifier = identifier;
      ArgumentName = identifier.Text.MakeArgumentName();

      ReadAccessibility = readAccessibility;
      IsStatic = isStatic;
      EnumMemberSettings = EnumMemberSettings.Create(member);
      ValueObjectMemberSettings = ValueObjectMemberSettings.Create(member);

      foreach (var @interface in type.AllInterfaces)
      {
         if (@interface.IsFormattableInterface())
         {
            IsFormattable = true;
         }
         else if (@interface.IsComparableInterface(_type))
         {
            IsComparable = true;
         }
      }
   }

   public static InstanceMemberInfo CreateFrom(IFieldSymbol field)
   {
      return new(field, field.Type, field.GetIdentifier(), field.DeclaredAccessibility, field.IsStatic);
   }

   public static InstanceMemberInfo CreateFrom(IPropertySymbol property)
   {
      return new(property, property.Type, property.GetIdentifier(), property.DeclaredAccessibility, property.IsStatic);
   }

   public Location GetIdentifierLocation()
   {
      return _identifier.GetLocation();
   }

   public override bool Equals(object? obj)
   {
      return obj is InstanceMemberInfo other && Equals(other);
   }

   public bool Equals(IMemberState? obj)
   {
      return obj is InstanceMemberInfo other && Equals(other);
   }

   public bool Equals(InstanceMemberInfo? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return TypeFullyQualifiedWithNullability == other.TypeFullyQualifiedWithNullability
             && Name == other.Name
             && ReadAccessibility == other.ReadAccessibility
             && IsStatic == other.IsStatic
             && IsReferenceType == other.IsReferenceType
             && _type.OriginalDefinition.SpecialType == other._type.OriginalDefinition.SpecialType
             && SpecialType == other.SpecialType
             && IsFormattable == other.IsFormattable
             && IsComparable == other.IsComparable
             && EnumMemberSettings.Equals(other.EnumMemberSettings)
             && ValueObjectMemberSettings.Equals(other.ValueObjectMemberSettings);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = TypeFullyQualifiedWithNullability.GetHashCode();
         hashCode = (hashCode * 397) ^ Name.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)ReadAccessibility;
         hashCode = (hashCode * 397) ^ IsStatic.GetHashCode();
         hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
         hashCode = (hashCode * 397) ^ _type.OriginalDefinition.SpecialType.GetHashCode();
         hashCode = (hashCode * 397) ^ SpecialType.GetHashCode();
         hashCode = (hashCode * 397) ^ IsFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ IsComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ EnumMemberSettings.GetHashCode();
         hashCode = (hashCode * 397) ^ ValueObjectMemberSettings.GetHashCode();

         return hashCode;
      }
   }

   public IMemberState CreateSymbolState(string name, bool isStatic)
   {
      return new DefaultMemberState(name, _type, name.MakeArgumentName(), isStatic);
   }
}
