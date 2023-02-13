using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectSettings : IEquatable<ValueObjectSettings>
{
   public bool SkipFactoryMethods { get; }
   public bool EmptyStringInFactoryMethodsYieldsNull { get; }
   public bool NullInFactoryMethodsYieldsNull { get; }
   public bool SkipCompareTo { get; }
   public bool SkipToString { get; }
   public string DefaultInstancePropertyName { get; }

   public ValueObjectSettings(AttributeData valueObjectAttribute)
   {
      SkipFactoryMethods = valueObjectAttribute.FindSkipFactoryMethods() ?? false;
      EmptyStringInFactoryMethodsYieldsNull = valueObjectAttribute.FindEmptyStringInFactoryMethodsYieldsNull() ?? false;
      NullInFactoryMethodsYieldsNull = EmptyStringInFactoryMethodsYieldsNull || (valueObjectAttribute.FindNullInFactoryMethodsYieldsNull() ?? false);
      SkipCompareTo = valueObjectAttribute.FindSkipCompareTo() ?? false;
      SkipToString = valueObjectAttribute.FindSkipToString() ?? false;
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
             && SkipCompareTo == other.SkipCompareTo
             && SkipToString == other.SkipToString
             && DefaultInstancePropertyName == other.DefaultInstancePropertyName;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SkipFactoryMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ EmptyStringInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ NullInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipCompareTo.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipToString.GetHashCode();
         hashCode = (hashCode * 397) ^ DefaultInstancePropertyName.GetHashCode();

         return hashCode;
      }
   }
}
