using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class EnumSettings : IEquatable<EnumSettings>
{
   public string? KeyPropertyName { get; }
   public bool SkipIComparable { get; }
   public bool SkipIParsable { get; }
   public bool SkipIComparisonOperators { get; }
   public bool SkipIFormattable { get; }
   public bool SkipToString { get; }

   public EnumSettings(AttributeData? attribute)
   {
      KeyPropertyName = attribute?.FindKeyPropertyName().TrimAndNullify();
      SkipIComparable = attribute?.FindSkipIComparable() ?? false;
      SkipIParsable = attribute?.FindSkipIParsable() ?? false;
      SkipIComparisonOperators = attribute?.FindSkipIComparisonOperators() ?? false;
      SkipIFormattable = attribute?.FindSkipIFormattable() ?? false;
      SkipToString = attribute?.FindSkipToString() ?? false;
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(EnumSettings? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return KeyPropertyName == other.KeyPropertyName
             && SkipIComparable == other.SkipIComparable
             && SkipIParsable == other.SkipIParsable
             && SkipIComparisonOperators == other.SkipIComparisonOperators
             && SkipIFormattable == other.SkipIFormattable
             && SkipToString == other.SkipToString;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = KeyPropertyName?.GetHashCode() ?? 0;
         hashCode = (hashCode * 397) ^ SkipIComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();

         return hashCode;
      }
   }
}
