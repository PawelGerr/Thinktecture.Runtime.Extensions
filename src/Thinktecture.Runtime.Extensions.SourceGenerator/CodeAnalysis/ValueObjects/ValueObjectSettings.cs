using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectSettings : IEquatable<ValueObjectSettings>
{
   public bool SkipFactoryMethods { get; }
   public bool EmptyStringInFactoryMethodsYieldsNull { get; }
   public bool NullInFactoryMethodsYieldsNull { get; }
   public bool SkipIComparable { get; }
   public bool SkipIParsable { get; }
   public bool SkipIFormattable { get; }
   public bool SkipToString { get; }
   public bool SkipIAdditionOperators { get; }
   public bool SkipISubtractionOperators { get; }
   public bool SkipIMultiplyOperators { get; }
   public bool SkipIDivisionOperators { get; }
   public bool SkipIComparisonOperators { get; }
   public string DefaultInstancePropertyName { get; }

   public ValueObjectSettings(AttributeData valueObjectAttribute)
   {
      SkipFactoryMethods = valueObjectAttribute.FindSkipFactoryMethods() ?? false;
      EmptyStringInFactoryMethodsYieldsNull = valueObjectAttribute.FindEmptyStringInFactoryMethodsYieldsNull() ?? false;
      NullInFactoryMethodsYieldsNull = EmptyStringInFactoryMethodsYieldsNull || (valueObjectAttribute.FindNullInFactoryMethodsYieldsNull() ?? false);
      SkipIComparable = valueObjectAttribute.FindSkipIComparable() ?? false;
      SkipIParsable = SkipFactoryMethods || (valueObjectAttribute.FindSkipIParsable() ?? false);
      SkipIFormattable = valueObjectAttribute.FindSkipIFormattable() ?? false;
      SkipToString = valueObjectAttribute.FindSkipToString() ?? false;
      SkipIAdditionOperators = SkipFactoryMethods || (valueObjectAttribute.FindSkipIAdditionOperators() ?? false);
      SkipISubtractionOperators = SkipFactoryMethods || (valueObjectAttribute.FindSkipISubtractionOperators() ?? false);
      SkipIMultiplyOperators = SkipFactoryMethods || (valueObjectAttribute.FindSkipIMultiplyOperators() ?? false);
      SkipIDivisionOperators = SkipFactoryMethods || (valueObjectAttribute.FindSkipIDivisionOperators() ?? false);
      SkipIComparisonOperators = valueObjectAttribute.FindSkipIComparisonOperators() ?? false;
      DefaultInstancePropertyName = valueObjectAttribute.FindDefaultInstancePropertyName() ?? "Empty";
   }

   public override bool Equals(object? obj)
   {
      return obj is ValueObjectSettings other && Equals(other);
   }

   public bool Equals(ValueObjectSettings? other)
   {
      if (ReferenceEquals(null, other))
         return false;
      if (ReferenceEquals(this, other))
         return true;

      return SkipFactoryMethods == other.SkipFactoryMethods
             && EmptyStringInFactoryMethodsYieldsNull == other.EmptyStringInFactoryMethodsYieldsNull
             && NullInFactoryMethodsYieldsNull == other.NullInFactoryMethodsYieldsNull
             && SkipIComparable == other.SkipIComparable
             && SkipIParsable == other.SkipIParsable
             && SkipIFormattable == other.SkipIFormattable
             && SkipToString == other.SkipToString
             && SkipIAdditionOperators == other.SkipIAdditionOperators
             && SkipISubtractionOperators == other.SkipISubtractionOperators
             && SkipIMultiplyOperators == other.SkipIMultiplyOperators
             && SkipIDivisionOperators == other.SkipIDivisionOperators
             && SkipIComparisonOperators == other.SkipIComparisonOperators
             && DefaultInstancePropertyName == other.DefaultInstancePropertyName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SkipFactoryMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ EmptyStringInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ NullInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIComparable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIParsable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIFormattable.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIAdditionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipISubtractionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIMultiplyOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIDivisionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipIComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ DefaultInstancePropertyName.GetHashCode();

         return hashCode;
      }
   }
}
