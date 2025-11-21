using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

#if NET9_0_OR_GREATER
// ReSharper disable once InconsistentNaming
[SmartEnum<int>]
[ObjectFactory<string>]
[ObjectFactory<ReadOnlySpan<char>>]
public partial class SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory
{
   public static readonly SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory Item1 = new(1);
   public static readonly SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory Item2 = new(2);

   public static ValidationError? Validate(string? value, IFormatProvider? provider, out SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory? item)
   {
      var key = Int32.Parse(value!, provider);
      return Validate(key, provider, out item);
   }

   public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out SmartEnum_IntBased_WithStringAndReadOnlySpanOfCharObjectFactory? item)
   {
      var key = Int32.Parse(value, provider);
      return Validate(key, provider, out item);
   }
}
#endif
