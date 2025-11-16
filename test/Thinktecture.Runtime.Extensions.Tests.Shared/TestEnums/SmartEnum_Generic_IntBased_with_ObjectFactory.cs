using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
[ObjectFactory<string>(UseForModelBinding = true, UseForSerialization = SerializationFrameworks.All, UseWithEntityFramework = true)]
public partial class SmartEnum_Generic_IntBased_with_ObjectFactory<T>
   where T : IEquatable<T>
{
   public static readonly SmartEnum_Generic_IntBased_with_ObjectFactory<T> Item1 = new(1, default);
   public static readonly SmartEnum_Generic_IntBased_with_ObjectFactory<T> Item2 = new(2, default);
   public static readonly SmartEnum_Generic_IntBased_with_ObjectFactory<T> Item3 = new(3, default);

   public T? Value { get; }

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out SmartEnum_Generic_IntBased_with_ObjectFactory<T>? item)
   {
      if (Int32.TryParse(value, out var number))
         return Validate(number, provider, out item);

      item = null;
      return new ValidationError("Not a number");
   }

   public string ToValue()
   {
      return Key.ToString();
   }
}
