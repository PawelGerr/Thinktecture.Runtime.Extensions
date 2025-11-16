using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ValueObject<int>]
[ObjectFactory<string>(UseForModelBinding = true, UseForSerialization = SerializationFrameworks.All, UseWithEntityFramework = true)]
public partial struct StructValueObject_Generic_IntBased_with_ObjectFactory<T>
   where T : IEquatable<T>
{
   public T? AdditionalValue { get; private init; }

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out StructValueObject_Generic_IntBased_with_ObjectFactory<T> item)
   {
      if (Int32.TryParse(value, out var number))
         return Validate(number, provider, out item);

      item = new StructValueObject_Generic_IntBased_with_ObjectFactory<T>(0);
      return new ValidationError("Not a number");
   }

   public string ToValue()
   {
      return _value.ToString();
   }
}
