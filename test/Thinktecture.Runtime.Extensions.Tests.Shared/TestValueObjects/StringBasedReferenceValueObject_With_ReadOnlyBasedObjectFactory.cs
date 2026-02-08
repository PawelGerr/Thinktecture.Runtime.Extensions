#if NET9_0_OR_GREATER
using System;

namespace Thinktecture.Runtime.Tests.TestValueObjects;

// ReSharper disable once InconsistentNaming
[ValueObject<string>]
[ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
[KeyMemberComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
public partial class StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory
{
   public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out StringBasedReferenceValueObject_With_ReadOnlyBasedObjectFactory? item)
   {
      return Validate(value.ToString(), provider, out item);
   }

   public ReadOnlySpan<char> ToValue()
   {
      return _value;
   }
}
#endif
