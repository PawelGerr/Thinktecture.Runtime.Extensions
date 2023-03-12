using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis.ValueObjects;

namespace Thinktecture.CodeAnalysis;

public sealed class InstanceMemberInfo : IMemberState, IEquatable<InstanceMemberInfo>
{
   private readonly ITypedMemberState _typedMemberState;
   private readonly (IFieldSymbol? Field, IPropertySymbol? Property) _symbol;

   private string? _argumentName;
   public string ArgumentName => _argumentName ??= Name.MakeArgumentName();

   public string Name { get; }
   public bool IsStatic { get; }
   public ValueObjectMemberSettings ValueObjectMemberSettings { get; }

   public SpecialType SpecialType => _typedMemberState.SpecialType;
   public string TypeFullyQualified => _typedMemberState.TypeFullyQualified;
   public string TypeFullyQualifiedNullAnnotated => _typedMemberState.TypeFullyQualifiedNullAnnotated;
   public string TypeFullyQualifiedWithNullability => _typedMemberState.TypeFullyQualifiedWithNullability;
   public string TypeMinimallyQualified => _typedMemberState.TypeMinimallyQualified;
   public string TypeFullyQualifiedNullable => _typedMemberState.TypeFullyQualifiedNullable;
   public bool IsReferenceType => _typedMemberState.IsReferenceType;
   public bool IsReferenceTypeOrNullableStruct => _typedMemberState.IsReferenceTypeOrNullableStruct;
   public bool IsFormattable => _typedMemberState.IsFormattable;
   public bool IsComparable => _typedMemberState.IsComparable;
   public bool IsParsable => _typedMemberState.IsParsable;
   public bool IsNullableStruct => _typedMemberState.IsNullableStruct;
   public NullableAnnotation NullableAnnotation => _typedMemberState.NullableAnnotation;
   public bool HasComparisonOperators => _typedMemberState.HasComparisonOperators;
   public bool HasAdditionOperators => _typedMemberState.HasAdditionOperators;
   public bool HasSubtractionOperators => _typedMemberState.HasSubtractionOperators;
   public bool HasMultiplyOperators => _typedMemberState.HasMultiplyOperators;
   public bool HasDivisionOperators => _typedMemberState.HasDivisionOperators;

   private InstanceMemberInfo(
      ITypedMemberState typedMemberState,
      ValueObjectMemberSettings settings,
      string name,
      (IFieldSymbol?, IPropertySymbol?) symbol,
      bool isStatic)
   {
      _typedMemberState = typedMemberState;
      _symbol = symbol;

      Name = name;
      IsStatic = isStatic;
      ValueObjectMemberSettings = settings;
   }

   public static InstanceMemberInfo? CreateOrNull(
      TypedMemberStateFactory factory,
      IFieldSymbol field,
      bool populateValueObjectMemberSettings,
      bool allowedCaptureSymbols)
   {
      if (field.Type.Kind == SymbolKind.ErrorType)
         return null;

      var symbol = allowedCaptureSymbols ? field : null;
      var settings = populateValueObjectMemberSettings ? ValueObjectMemberSettings.Create(field, field.Type, allowedCaptureSymbols) : ValueObjectMemberSettings.None;

      return new(factory.Create(field.Type), settings, field.Name, (symbol, null), field.IsStatic);
   }

   public static InstanceMemberInfo? CreateOrNull(
      TypedMemberStateFactory factory,
      IPropertySymbol property,
      bool populateValueObjectMemberSettings,
      bool allowedCaptureSymbols)
   {
      if (property.Type.Kind == SymbolKind.ErrorType)
         return null;

      var symbol = allowedCaptureSymbols ? property : null;
      var settings = populateValueObjectMemberSettings ? ValueObjectMemberSettings.Create(property, property.Type, allowedCaptureSymbols) : ValueObjectMemberSettings.None;

      return new(factory.Create(property.Type), settings, property.Name, (null, symbol), property.IsStatic);
   }

   public Location GetIdentifierLocation(CancellationToken cancellationToken)
   {
      if (_symbol.Field is not null)
         return _symbol.Field.GetIdentifier(cancellationToken)?.GetLocation() ?? Location.None;

      if (_symbol.Property is not null)
         return _symbol.Property.GetIdentifier(cancellationToken)?.GetLocation() ?? Location.None;

      return Location.None;
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

      return _typedMemberState.Equals(other._typedMemberState)
             && Name == other.Name
             && IsStatic == other.IsStatic
             && ValueObjectMemberSettings.Equals(other.ValueObjectMemberSettings);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = _typedMemberState.GetHashCode();
         hashCode = (hashCode * 397) ^ Name.GetHashCode();
         hashCode = (hashCode * 397) ^ IsStatic.GetHashCode();
         hashCode = (hashCode * 397) ^ ValueObjectMemberSettings.GetHashCode();

         return hashCode;
      }
   }
}
