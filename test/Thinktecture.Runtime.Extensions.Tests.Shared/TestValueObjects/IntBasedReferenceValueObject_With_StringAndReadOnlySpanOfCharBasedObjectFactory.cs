#if NET9_0_OR_GREATER
using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ValueObject<int>]
[ObjectFactory<string>]
[ObjectFactory<ReadOnlySpan<char>>]
public partial class IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory
{
   public static ValidationError? Validate(string? value, IFormatProvider? provider, out IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory? item)
   {
      return Validate(Int32.Parse(value!), provider, out item);
   }

   public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out IntBasedReferenceValueObject_With_StringAndReadOnlySpanOfCharBasedObjectFactory? item)
   {
      return Validate(Int32.Parse(value), provider, out item);
   }
}

#endif
