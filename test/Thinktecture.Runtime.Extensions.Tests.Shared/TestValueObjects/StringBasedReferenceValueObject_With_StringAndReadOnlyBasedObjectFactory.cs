#if NET9_0_OR_GREATER
using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ValueObject<string>]
[ObjectFactory<string>]
[ObjectFactory<ReadOnlySpan<char>>]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial class StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory
{
   public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out StringBasedReferenceValueObject_With_StringAndReadOnlyBasedObjectFactory? item)
   {
      return Validate(value.ToString(), provider, out item);
   }
}
#endif
