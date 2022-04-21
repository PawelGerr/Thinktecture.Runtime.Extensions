using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class ValueObjectMemberSettings : IEquatable<ValueObjectMemberSettings>
{
   public static readonly ValueObjectMemberSettings None = new();

   private readonly AttributeData? _attributeData;

   public bool IsExplicitlyDeclared { get; }

   public string? Comparer { get; }
   public string? EqualityComparer { get; }

   private ValueObjectMemberSettings()
   {
   }

   private ValueObjectMemberSettings(AttributeData attributeData)
   {
      _attributeData = attributeData;
      IsExplicitlyDeclared = true;
      Comparer = attributeData.FindComparer().TrimAndNullify();
      EqualityComparer = attributeData.FindEqualityComparer().TrimAndNullify();
   }

   public static ValueObjectMemberSettings Create(ISymbol member)
   {
      var attr = member.FindAttribute("Thinktecture.ValueObjectEqualityMemberAttribute");

      return attr is null ? None : new ValueObjectMemberSettings(attr);
   }

   public Location? GetAttributeLocationOrNull(CancellationToken cancellationToken)
   {
      return _attributeData?.ApplicationSyntaxReference?.GetSyntax(cancellationToken).GetLocation();
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
             && Comparer == other.Comparer
             && EqualityComparer == other.EqualityComparer;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = IsExplicitlyDeclared.GetHashCode();
         hashCode = (hashCode * 397) ^ (Comparer?.GetHashCode() ?? 0);
         hashCode = (hashCode * 397) ^ (EqualityComparer?.GetHashCode() ?? 0);
         return hashCode;
      }
   }
}
