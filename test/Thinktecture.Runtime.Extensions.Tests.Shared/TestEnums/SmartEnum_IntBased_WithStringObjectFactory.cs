using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
[ObjectFactory<string>]
public partial class SmartEnum_IntBased_WithStringObjectFactory
{
   public static readonly SmartEnum_IntBased_WithStringObjectFactory Item1 = new(1);
   public static readonly SmartEnum_IntBased_WithStringObjectFactory Item2 = new(2);

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out SmartEnum_IntBased_WithStringObjectFactory? item)
   {
      var key = Int32.Parse(value!, provider);
      return Validate(key, provider, out item);
   }
}
