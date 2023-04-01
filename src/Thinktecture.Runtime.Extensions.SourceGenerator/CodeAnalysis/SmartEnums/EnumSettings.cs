using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class EnumSettings : IEquatable<EnumSettings>
{
   public string? KeyPropertyName { get; }
   public bool SkipIComparable { get; }
   public bool SkipIParsable { get; }
   public OperatorsGeneration ComparisonOperators { get; }
   public OperatorsGeneration EqualityComparisonOperators { get; }
   public bool SkipIFormattable { get; }
   public bool SkipToString { get; }

   public EnumSettings(AttributeData? attribute)
   {
      KeyPropertyName = attribute?.FindKeyPropertyName().TrimAndNullify();
      SkipIComparable = attribute?.FindSkipIComparable() ?? false;
      SkipIParsable = attribute?.FindSkipIParsable() ?? false;
      ComparisonOperators = attribute?.FindComparisonOperators() ?? OperatorsGeneration.Default;
      EqualityComparisonOperators = attribute?.FindEqualityComparisonOperators() ?? OperatorsGeneration.Default;
      SkipIFormattable = attribute?.FindSkipIFormattable() ?? false;
      SkipToString = attribute?.FindSkipToString() ?? false;

      // Comparison operators depend on the equality comparison operators
      if (ComparisonOperators > EqualityComparisonOperators)
         EqualityComparisonOperators = ComparisonOperators;
   }

   public override bool Equals(object? obj)
   {
      return obj is EnumSettings enumSettings && Equals(enumSettings);
   }

   public bool Equals(EnumSettings? other)
   {
      if (other is null)
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return KeyPropertyName == other.KeyPropertyName
             && SkipIComparable == other.SkipIComparable
             && SkipIParsable == other.SkipIParsable
             && ComparisonOperators == other.ComparisonOperators
             && EqualityComparisonOperators == other.EqualityComparisonOperators
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
         hashCode = (hashCode * 397) ^ ComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ EqualityComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();

         return hashCode;
      }
   }
}
