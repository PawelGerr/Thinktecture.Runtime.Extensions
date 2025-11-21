using System;

namespace Thinktecture.Runtime.Tests.TestEnums;

// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
[ObjectFactory<string>]
#if NET9_0_OR_GREATER
[ObjectFactory<ReadOnlySpan<char>>]
#endif
public partial class SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory
{
   public static readonly SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory Item1 = new("Item1");
   public static readonly SmartEnum_StringBased_WithStringAndReadOnlySpanOfCharObjectFactory Item2 = new("Item2");
}
