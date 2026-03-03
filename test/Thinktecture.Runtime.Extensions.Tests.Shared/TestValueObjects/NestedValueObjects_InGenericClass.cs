namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
public partial class ValueObjects_NestedInGenericClass
{
   public partial class GenericOuter<T>
   {
      [ValueObject<int>]
      public sealed partial class KeyedValueObject;

      [ComplexValueObject]
      public sealed partial class ComplexValueObject
      {
         public int Value { get; }
      }
   }
}
