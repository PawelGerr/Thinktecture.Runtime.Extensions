using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class AllValueObjectSettings : IEquatable<AllValueObjectSettings>
{
   public bool SkipFactoryMethods { get; }
   public bool EmptyStringInFactoryMethodsYieldsNull { get; }
   public bool NullInFactoryMethodsYieldsNull { get; }
   public bool SkipIComparable { get; }
   public bool SkipIParsable { get; }
   public bool SkipIFormattable { get; }
   public bool SkipToString { get; }
   public OperatorsGeneration AdditionOperators { get; }
   public OperatorsGeneration SubtractionOperators { get; }
   public OperatorsGeneration MultiplyOperators { get; }
   public OperatorsGeneration DivisionOperators { get; }
   public OperatorsGeneration ComparisonOperators { get; }
   public OperatorsGeneration EqualityComparisonOperators { get; }
   public string DefaultInstancePropertyName { get; }

   public AllValueObjectSettings(AttributeData valueObjectAttribute)
   {
      SkipFactoryMethods = valueObjectAttribute.FindSkipFactoryMethods() ?? false;
      EmptyStringInFactoryMethodsYieldsNull = valueObjectAttribute.FindEmptyStringInFactoryMethodsYieldsNull() ?? false;
      NullInFactoryMethodsYieldsNull = EmptyStringInFactoryMethodsYieldsNull || (valueObjectAttribute.FindNullInFactoryMethodsYieldsNull() ?? false);
      SkipIComparable = valueObjectAttribute.FindSkipIComparable() ?? false;
      SkipIParsable = SkipFactoryMethods || (valueObjectAttribute.FindSkipIParsable() ?? false);
      SkipIFormattable = valueObjectAttribute.FindSkipIFormattable() ?? false;
      SkipToString = valueObjectAttribute.FindSkipToString() ?? false;
      AdditionOperators = SkipFactoryMethods ? OperatorsGeneration.None : valueObjectAttribute.FindAdditionOperators();
      SubtractionOperators = SkipFactoryMethods ? OperatorsGeneration.None : valueObjectAttribute.FindSubtractionOperators();
      MultiplyOperators = SkipFactoryMethods ? OperatorsGeneration.None : valueObjectAttribute.FindMultiplyOperators();
      DivisionOperators = SkipFactoryMethods ? OperatorsGeneration.None : valueObjectAttribute.FindDivisionOperators();
      EqualityComparisonOperators = valueObjectAttribute.FindEqualityComparisonOperators();
      ComparisonOperators = valueObjectAttribute.FindComparisonOperators();
      DefaultInstancePropertyName = valueObjectAttribute.FindDefaultInstancePropertyName() ?? "Empty";

      // Comparison operators depend on the equality comparison operators
      if (ComparisonOperators > EqualityComparisonOperators)
         EqualityComparisonOperators = ComparisonOperators;
   }

   public override bool Equals(object? obj)
   {
      return obj is AllValueObjectSettings other && Equals(other);
   }

   public bool Equals(AllValueObjectSettings? other)
   {
      if (other is null)
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
             && AdditionOperators == other.AdditionOperators
             && SubtractionOperators == other.SubtractionOperators
             && MultiplyOperators == other.MultiplyOperators
             && DivisionOperators == other.DivisionOperators
             && ComparisonOperators == other.ComparisonOperators
             && EqualityComparisonOperators == other.EqualityComparisonOperators
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
         hashCode = (hashCode * 397) ^ AdditionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ SubtractionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ MultiplyOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ DivisionOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ ComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ EqualityComparisonOperators.GetHashCode();
         hashCode = (hashCode * 397) ^ DefaultInstancePropertyName.GetHashCode();

         return hashCode;
      }
   }
}
