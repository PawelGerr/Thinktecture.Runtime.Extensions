using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectMemberSettings : IEquatable<ValueObjectMemberSettings>
{
   private static readonly ValueObjectMemberSettings _none = new(false);
   private static readonly ValueObjectMemberSettings _noneForString = new(false, null, Constants.ComparerAccessor.ORDINAL_IGNORE_CASE, false, null, Constants.ComparerAccessor.ORDINAL_IGNORE_CASE, false);

   private readonly SyntaxReference? _equalityComparerAttr;
   private readonly SyntaxReference? _comparerAttr;

   public bool IsExplicitlyDeclared { get; }
   public string? ComparerAccessor { get; }
   public string? EqualityComparerAccessor { get; }
   public bool HasInvalidComparerType { get; }
   public bool HasInvalidEqualityComparerType { get; }

   private ValueObjectMemberSettings(bool isExplicitlyDeclared)
   {
      IsExplicitlyDeclared = isExplicitlyDeclared;
   }

   private ValueObjectMemberSettings(
      bool isExplicitlyDeclared,
      SyntaxReference? equalityComparerAttr,
      string? equalityComparerAccessorType,
      bool hasInvalidEqualityComparerType,
      SyntaxReference? comparerAttr,
      string? comparerAccessorType,
      bool hasInvalidComparerType)
      : this(true)
   {
      _equalityComparerAttr = equalityComparerAttr;
      _comparerAttr = comparerAttr;

      IsExplicitlyDeclared = isExplicitlyDeclared;
      HasInvalidComparerType = hasInvalidComparerType;
      HasInvalidEqualityComparerType = hasInvalidEqualityComparerType;
      ComparerAccessor = comparerAccessorType;
      EqualityComparerAccessor = equalityComparerAccessorType;
   }

   public static ValueObjectMemberSettings Create(ISymbol member, ITypeSymbol type, bool canCaptureSymbols)
   {
      var equalityComparerAttr = member.FindAttribute(static type => type.Name == "ValueObjectMemberEqualityComparerAttribute" && type.ContainingNamespace is { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true });
      var comparerAttr = member.FindAttribute(static type => type.Name == "ValueObjectMemberComparerAttribute" && type.ContainingNamespace is { Name: "Thinktecture", ContainingNamespace.IsGlobalNamespace: true });

      if (equalityComparerAttr is null && comparerAttr is null)
         return type.SpecialType == SpecialType.System_String ? _noneForString : _none;

      var equalityComparerGenericTypes = equalityComparerAttr?.GetComparerTypes();
      var equalityComparerAccessorType = equalityComparerGenericTypes?.ComparerType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? (type.SpecialType == SpecialType.System_String ? Constants.ComparerAccessor.ORDINAL_IGNORE_CASE : null);
      var hasInvalidEqualityComparerType = equalityComparerGenericTypes is not null && !SymbolEqualityComparer.Default.Equals(equalityComparerGenericTypes.Value.ItemType, type);

      var comparerGenericTypes = comparerAttr?.GetComparerTypes();
      var comparerAccessorType = comparerGenericTypes?.ComparerType?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? (type.SpecialType == SpecialType.System_String ? Constants.ComparerAccessor.ORDINAL_IGNORE_CASE : null);
      var hasInvalidComparerType = comparerGenericTypes is not null && !SymbolEqualityComparer.Default.Equals(comparerGenericTypes.Value.ItemType, type);

      return new ValueObjectMemberSettings(true,
                                           canCaptureSymbols ? equalityComparerAttr?.ApplicationSyntaxReference : null,
                                           equalityComparerAccessorType,
                                           hasInvalidEqualityComparerType,
                                           canCaptureSymbols ? comparerAttr?.ApplicationSyntaxReference : null,
                                           comparerAccessorType,
                                           hasInvalidComparerType);
   }

   public Location? GetEqualityComparerAttributeLocationOrNull(CancellationToken cancellationToken)
   {
      return _equalityComparerAttr?.GetSyntax(cancellationToken).GetLocation();
   }

   public Location? GetComparerAttributeLocationOrNull(CancellationToken cancellationToken)
   {
      return _comparerAttr?.GetSyntax(cancellationToken).GetLocation();
   }

   public override bool Equals(object? obj)
   {
      return obj is ValueObjectMemberSettings other && Equals(other);
   }

   public bool Equals(ValueObjectMemberSettings? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return IsExplicitlyDeclared == other.IsExplicitlyDeclared
             && ComparerAccessor == other.ComparerAccessor
             && EqualityComparerAccessor == other.EqualityComparerAccessor
             && HasInvalidEqualityComparerType == other.HasInvalidEqualityComparerType
             && HasInvalidComparerType == other.HasInvalidComparerType;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = IsExplicitlyDeclared.GetHashCode();
         hashCode = (hashCode * 397) ^ (ComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ (EqualityComparerAccessor?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ HasInvalidEqualityComparerType.GetHashCode();
         hashCode = (hashCode * 397) ^ HasInvalidComparerType.GetHashCode();
         return hashCode;
      }
   }
}
