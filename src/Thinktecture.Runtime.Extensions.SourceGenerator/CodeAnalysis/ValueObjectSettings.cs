using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

public class ValueObjectSettings : IEquatable<ValueObjectSettings>
{
   public bool SkipFactoryMethods { get; }
   public bool NullInFactoryMethodsYieldsNull { get; }
   public bool SkipCompareTo { get; }

   public ValueObjectSettings(AttributeData valueObjectAttribute)
   {
      SkipFactoryMethods = valueObjectAttribute.FindSkipFactoryMethods() ?? false;
      NullInFactoryMethodsYieldsNull = valueObjectAttribute.FindNullInFactoryMethodsYieldsNull() ?? false;
      SkipCompareTo = valueObjectAttribute.FindSkipCompareTo() ?? false;
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
             && NullInFactoryMethodsYieldsNull == other.NullInFactoryMethodsYieldsNull
             && SkipCompareTo == other.SkipCompareTo;
   }

   public override int GetHashCode()
   {
      unchecked
      {
         var hashCode = SkipFactoryMethods.GetHashCode();
         hashCode = (hashCode * 397) ^ NullInFactoryMethodsYieldsNull.GetHashCode();
         hashCode = (hashCode * 397) ^ SkipCompareTo.GetHashCode();

         return hashCode;
      }
   }
}
