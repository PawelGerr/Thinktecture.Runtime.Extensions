#if NET9_0_OR_GREATER
#nullable enable
using System;

namespace Thinktecture.Runtime.Tests.Json.ThinktectureNewtonsoftJsonConverterTests.TestClasses;

// Regression type: a string-keyed Value Object whose ReadOnlySpan<char> object factory is (incorrectly) flagged for
// Newtonsoft.Json. The Newtonsoft converter factory must ignore the span factory and fall back to the string key
// instead of crashing while building the converter (a ref struct cannot be a generic type argument of the converter).
[ValueObject<string>]
[ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.All)]
[KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
public partial class StringValueObjectWithSpanFactoryForNewtonsoft
{
   public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out StringValueObjectWithSpanFactoryForNewtonsoft? item)
   {
      return Validate(value.ToString(), provider, out item);
   }

   public ReadOnlySpan<char> ToValue()
   {
      return _value;
   }
}
#endif
