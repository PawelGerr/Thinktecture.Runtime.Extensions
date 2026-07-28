#if NET9_0_OR_GREATER
using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace Thinktecture.Runtime.Tests.ComparerAccessorsTests;

// The source generator emits the allocation-free span-based lookup without a capability check when the key member
// equality comparer accessor lives in Thinktecture.ComparerAccessors. This test asserts the invariant that this
// decision relies on: every accessor declared there returns a comparer that provides the alternate lookup of
// FrozenDictionary.
public class AlternateLookupSupport
{
   [Theory]
   [InlineData(nameof(ComparerAccessors.StringOrdinal))]
   [InlineData(nameof(ComparerAccessors.StringOrdinalIgnoreCase))]
   [InlineData(nameof(ComparerAccessors.CurrentCulture))]
   [InlineData(nameof(ComparerAccessors.CurrentCultureIgnoreCase))]
   [InlineData(nameof(ComparerAccessors.InvariantCulture))]
   [InlineData(nameof(ComparerAccessors.InvariantCultureIgnoreCase))]
   [InlineData(nameof(ComparerAccessors.Default<string>))]
   public void Should_support_span_alternate_lookup_when_comparer_comes_from_ComparerAccessors(string accessorName)
   {
      var comparer = GetEqualityComparer(accessorName);
      var frozenDictionary = new Dictionary<string, int> { { "item", 1 } }.ToFrozenDictionary(comparer);

      frozenDictionary.TryGetAlternateLookup<ReadOnlySpan<char>>(out _).Should().BeTrue();
   }

   private static IEqualityComparer<string> GetEqualityComparer(string accessorName)
   {
      return accessorName switch
      {
         nameof(ComparerAccessors.StringOrdinal) => ComparerAccessors.StringOrdinal.EqualityComparer,
         nameof(ComparerAccessors.StringOrdinalIgnoreCase) => ComparerAccessors.StringOrdinalIgnoreCase.EqualityComparer,
         nameof(ComparerAccessors.CurrentCulture) => ComparerAccessors.CurrentCulture.EqualityComparer,
         nameof(ComparerAccessors.CurrentCultureIgnoreCase) => ComparerAccessors.CurrentCultureIgnoreCase.EqualityComparer,
         nameof(ComparerAccessors.InvariantCulture) => ComparerAccessors.InvariantCulture.EqualityComparer,
         nameof(ComparerAccessors.InvariantCultureIgnoreCase) => ComparerAccessors.InvariantCultureIgnoreCase.EqualityComparer,
         nameof(ComparerAccessors.Default<string>) => ComparerAccessors.Default<string>.EqualityComparer,
         _ => throw new ArgumentException($"Unknown comparer accessor '{accessorName}'.", nameof(accessorName))
      };
   }

   // The theory above resolves each accessor through a manual switch, so an accessor added to ComparerAccessors
   // later would stay uncovered without any test failing. This guard fails in that case and points at the theory.
   [Fact]
   public void Should_cover_every_accessor_declared_in_ComparerAccessors()
   {
      var declaredAccessors = typeof(ComparerAccessors).GetNestedTypes();

      declaredAccessors.Length.Should()
                       .Be(7, "every accessor of ComparerAccessors needs an [InlineData] case in "
                              + nameof(Should_support_span_alternate_lookup_when_comparer_comes_from_ComparerAccessors));
   }
}
#endif
