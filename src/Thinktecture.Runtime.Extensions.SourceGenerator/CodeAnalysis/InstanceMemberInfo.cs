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
   public bool IsErroneous { get; }
   public bool IsAbstract { get; }
   public ValueObjectMemberSettings ValueObjectMemberSettings { get; }
   public SymbolKind Kind { get; }

   public SpecialType SpecialType => _typedMemberState.SpecialType;
   public string TypeFullyQualified => _typedMemberState.TypeFullyQualified;
   public string TypeMinimallyQualified => _typedMemberState.TypeMinimallyQualified;
   public bool IsReferenceType => _typedMemberState.IsReferenceType;
   public bool IsReferenceTypeOrNullableStruct => _typedMemberState.IsReferenceTypeOrNullableStruct;
   public bool IsNullableStruct => _typedMemberState.IsNullableStruct;
   public NullableAnnotation NullableAnnotation => _typedMemberState.NullableAnnotation;

   private InstanceMemberInfo(
      ITypedMemberState typedMemberState,
      ValueObjectMemberSettings settings,
      string name,
      (IFieldSymbol?, IPropertySymbol?) symbol,
      bool isStatic,
      bool isErroneous,
      bool isAbstract,
      SymbolKind kind)
   {
      _typedMemberState = typedMemberState;
      _symbol = symbol;

      Name = name;
      IsStatic = isStatic;
      IsErroneous = isErroneous;
      IsAbstract = isAbstract;
      Kind = kind;
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

      return new(
         factory.Create(field.Type),
         settings,
         field.Name,
         (symbol, null),
         field.IsStatic,
         field.Type.TypeKind == TypeKind.Error,
         field.IsAbstract,
         field.Kind);
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

      return new(
         factory.Create(property.Type),
         settings,
         property.Name,
         (null, symbol),
         property.IsStatic,
         property.Type.TypeKind == TypeKind.Error,
         property.IsAbstract,
         property.Kind);
   }

   public Location? GetIdentifierLocation(CancellationToken cancellationToken)
   {
      if (_symbol.Field is not null)
         return _symbol.Field.GetIdentifier(cancellationToken)?.GetLocation() ?? Location.None;

      if (_symbol.Property is not null)
         return _symbol.Property.GetIdentifier(cancellationToken)?.GetLocation() ?? Location.None;

      return null;
   }

   public bool IsOfType(ITypeSymbol type)
   {
      return SymbolEqualityComparer.IncludeNullability.Equals(_symbol.Field?.Type ?? _symbol.Property?.Type, type);
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
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return _typedMemberState.Equals(other._typedMemberState)
             && Name == other.Name
             && IsStatic == other.IsStatic
             && IsErroneous == other.IsErroneous
             && IsAbstract == other.IsAbstract
             && Kind == other.Kind
             && ValueObjectMemberSettings.Equals(other.ValueObjectMemberSettings);
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = _typedMemberState.GetHashCode();
         hashCode = (hashCode * 397) ^ Name.GetHashCode();
         hashCode = (hashCode * 397) ^ IsStatic.GetHashCode();
         hashCode = (hashCode * 397) ^ IsErroneous.GetHashCode();
         hashCode = (hashCode * 397) ^ IsAbstract.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)Kind;
         hashCode = (hashCode * 397) ^ ValueObjectMemberSettings.GetHashCode();

         return hashCode;
      }
   }
}
