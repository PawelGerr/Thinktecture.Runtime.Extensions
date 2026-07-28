using System;
using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.TestEnums;

// Regression type for the FrozenDictionary.GetAlternateLookup issue on net9.0/net10.0.
//
// The key equality comparer is a hand-written IEqualityComparer<string> that deliberately does NOT implement
// IAlternateEqualityComparer<ReadOnlySpan<char>, string>. Unlike the BCL StringComparer instances, such a
// comparer cannot back a zero-allocation span-based alternate lookup, so the generated GetLookups must not
// call FrozenDictionary.GetAlternateLookup unconditionally (which throws InvalidOperationException and, because
// the surrounding Lazy caches the exception, permanently breaks Items/Get/TryGet/Validate/Switch/Map).
// ReSharper disable once InconsistentNaming
[SmartEnum<string>]
[KeyMemberComparer<NonAlternateOrdinalStringComparerAccessor, string>]
[KeyMemberEqualityComparer<NonAlternateOrdinalStringComparerAccessor, string>]
public partial class SmartEnum_StringBased_WithNonAlternateLookupComparer
{
   public static readonly SmartEnum_StringBased_WithNonAlternateLookupComparer Item1 = new("Item1");
   public static readonly SmartEnum_StringBased_WithNonAlternateLookupComparer Item2 = new("Item2");
}

public sealed class NonAlternateOrdinalStringComparerAccessor : IEqualityComparerAccessor<string>, IComparerAccessor<string>
{
   public static IEqualityComparer<string> EqualityComparer { get; } = new NonAlternateOrdinalStringComparer();
   public static IComparer<string> Comparer => StringComparer.Ordinal;
}

// A plain IEqualityComparer<string> with ordinal semantics. It does not implement
// IAlternateEqualityComparer<ReadOnlySpan<char>, string>, so FrozenDictionary.TryGetAlternateLookup returns false.
public sealed class NonAlternateOrdinalStringComparer : IEqualityComparer<string>
{
   public bool Equals(string? x, string? y)
   {
      return String.Equals(x, y, StringComparison.Ordinal);
   }

   public int GetHashCode(string obj)
   {
      return StringComparer.Ordinal.GetHashCode(obj);
   }
}
