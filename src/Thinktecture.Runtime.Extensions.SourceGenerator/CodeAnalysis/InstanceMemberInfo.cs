using Thinktecture.CodeAnalysis.ValueObjects;
using Thinktecture.Json;

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

   public bool IsRecord => false;

   public SpecialType SpecialType => _typedMemberState.SpecialType;
   public bool IsTypeParameter => _typedMemberState.TypeKind == TypeKind.TypeParameter;
   public string TypeFullyQualified => _typedMemberState.TypeFullyQualified;
   public string TypeMinimallyQualified => _typedMemberState.TypeMinimallyQualified;
   public bool IsReferenceType => _typedMemberState.IsReferenceType;
   public bool IsReferenceTypeOrNullableStruct => _typedMemberState.IsReferenceTypeOrNullableStruct;
   public bool IsNullableStruct => _typedMemberState.IsNullableStruct;
   public NullableAnnotation NullableAnnotation { get; }
   public bool DisallowsDefaultValue { get; }
   public int? MessagePackKey { get; }
   public JsonIgnoreCondition? JsonIgnoreCondition { get; }

   private InstanceMemberInfo(
      ITypedMemberState typedMemberState,
      ValueObjectMemberSettings settings,
      string name,
      (IFieldSymbol?, IPropertySymbol?) symbol,
      bool isStatic,
      bool isErroneous,
      bool isAbstract,
      bool disallowsDefaultValue,
      SymbolKind kind,
      NullableAnnotation nullableAnnotation,
      int? messagePackKey,
      JsonIgnoreCondition? jsonIgnoreCondition)
   {
      _typedMemberState = typedMemberState;
      _symbol = symbol;

      Name = name;
      IsStatic = isStatic;
      IsErroneous = isErroneous;
      IsAbstract = isAbstract;
      DisallowsDefaultValue = disallowsDefaultValue;
      Kind = kind;
      NullableAnnotation = nullableAnnotation;
      MessagePackKey = messagePackKey;
      JsonIgnoreCondition = jsonIgnoreCondition;
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
         IsDisallowingDefaultValue(field.Type),
         field.Kind,
         field.NullableAnnotation,
         GetMessagePackKey(field),
         GetJsonIgnoreCondition(field));
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
         IsDisallowingDefaultValue(property.Type),
         property.Kind,
         property.NullableAnnotation,
         GetMessagePackKey(property),
         GetJsonIgnoreCondition(property));
   }

   private static int? GetMessagePackKey(ISymbol member)
   {
      var messagePackKeyAttribute = member.FindAttribute(a => a.IsMessagePackKeyAttribute());

      if (messagePackKeyAttribute is null || messagePackKeyAttribute.ConstructorArguments.Length != 1)
         return null;

      var ctorArg = messagePackKeyAttribute.ConstructorArguments[0];

      if (ctorArg.Type is null)
         return null;

      return ctorArg.Type.SpecialType == SpecialType.System_Int32
             && ctorArg.Value is int value
                ? value
                : null;
   }

   private static JsonIgnoreCondition? GetJsonIgnoreCondition(ISymbol member)
   {
      var jsonIgnoreAttribute = member.FindAttribute(a => a.IsJsonIgnoreAttribute());

      if (jsonIgnoreAttribute is null)
         return null;

      return jsonIgnoreAttribute.FindJsonIgnoreCondition() ?? Json.JsonIgnoreCondition.Always;
   }

   private static bool IsDisallowingDefaultValue(ITypeSymbol type)
   {
      if (type.ImplementsIDisallowDefaultValue())
         return true;

      if (type.IsReferenceType
          || type.TypeKind == TypeKind.Error
          || type.SpecialType != SpecialType.None) // Non-value objects are allowed to have default values
         return false;

      if (!type.IsValueObjectType(out var attribute))
         return false;

      return !attribute.FindAllowDefaultStructs()
             || (
                   attribute.AttributeClass.IsKeyedValueObjectAttribute()
                   && attribute.AttributeClass.TypeArguments[0].IsReferenceType
                );
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
             && DisallowsDefaultValue == other.DisallowsDefaultValue
             && Kind == other.Kind
             && NullableAnnotation == other.NullableAnnotation
             && MessagePackKey == other.MessagePackKey
             && JsonIgnoreCondition == other.JsonIgnoreCondition
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
         hashCode = (hashCode * 397) ^ DisallowsDefaultValue.GetHashCode();
         hashCode = (hashCode * 397) ^ (int)Kind;
         hashCode = (hashCode * 397) ^ (int)NullableAnnotation;
         hashCode = (hashCode * 397) ^ MessagePackKey.GetHashCode();
         hashCode = (hashCode * 397) ^ JsonIgnoreCondition.GetHashCode();
         hashCode = (hashCode * 397) ^ ValueObjectMemberSettings.GetHashCode();

         return hashCode;
      }
   }
}
