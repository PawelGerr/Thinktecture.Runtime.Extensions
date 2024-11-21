namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectMemberSettings : IEquatable<ValueObjectMemberSettings>
{
   public static readonly ValueObjectMemberSettings None = new(false);

   private readonly SyntaxReference? _equalityComparerAttr;

   public bool IsExplicitlyDeclared { get; }
   public string? EqualityComparerAccessor { get; }
   public bool HasInvalidEqualityComparerType { get; }

   private ValueObjectMemberSettings(bool isExplicitlyDeclared)
   {
      IsExplicitlyDeclared = isExplicitlyDeclared;
   }

   private ValueObjectMemberSettings(
      bool isExplicitlyDeclared,
      SyntaxReference? equalityComparerAttr,
      string? equalityComparerAccessorType,
      bool hasInvalidEqualityComparerType)
      : this(true)
   {
      _equalityComparerAttr = equalityComparerAttr;

      IsExplicitlyDeclared = isExplicitlyDeclared;
      HasInvalidEqualityComparerType = hasInvalidEqualityComparerType;
      EqualityComparerAccessor = equalityComparerAccessorType;
   }

   public static ValueObjectMemberSettings Create(ISymbol member, ITypeSymbol type, bool canCaptureSymbols)
   {
      AttributeData? equalityComparerAttr = null;

      foreach (var attribute in member.GetAttributes())
      {
         if (attribute.AttributeClass.IsValueObjectMemberEqualityComparerAttribute())
            equalityComparerAttr = attribute;
      }

      if (equalityComparerAttr is null)
         return None;

      var equalityComparerGenericTypes = equalityComparerAttr.GetComparerTypes();
      var equalityComparerAccessorType = equalityComparerGenericTypes?.ComparerType?.ToFullyQualifiedDisplayString();
      var hasInvalidEqualityComparerType = equalityComparerGenericTypes is not null && !SymbolEqualityComparer.Default.Equals(equalityComparerGenericTypes.Value.ItemType, type);

      return new ValueObjectMemberSettings(true,
                                           canCaptureSymbols ? equalityComparerAttr.ApplicationSyntaxReference : null,
                                           equalityComparerAccessorType,
                                           hasInvalidEqualityComparerType);
   }

   public Location? GetEqualityComparerAttributeLocationOrNull(CancellationToken cancellationToken)
   {
      return _equalityComparerAttr?.GetSyntax(cancellationToken).GetLocation();
   }

   public override bool Equals(object? obj)
   {
      return obj is ValueObjectMemberSettings other && Equals(other);
   }

   public bool Equals(ValueObjectMemberSettings? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return IsExplicitlyDeclared == other.IsExplicitlyDeclared
             && EqualityComparerAccessor == other.EqualityComparerAccessor
             && HasInvalidEqualityComparerType == other.HasInvalidEqualityComparerType;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = IsExplicitlyDeclared.GetHashCode();
         hashCode = (hashCode * 397) ^ (EqualityComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ HasInvalidEqualityComparerType.GetHashCode();
         return hashCode;
      }
   }
}
