using System;
using Thinktecture.Runtime.Tests.TestEnums;

namespace Thinktecture.Runtime.Tests.EnumTests;

// Regression tests for a string-keyed Smart Enum whose custom key equality comparer does not implement
// IAlternateEqualityComparer<ReadOnlySpan<char>, string>. Before the fix, the generated GetLookups called
// FrozenDictionary.GetAlternateLookup unconditionally, which throws InvalidOperationException on net9.0/net10.0;
// the caching Lazy then made every member (Items/Get/TryGet) fail permanently. The net8.0 build was unaffected.
public class NonAlternateLookupComparer
{
   [Fact]
   public void Should_expose_items_without_throwing()
   {
      SmartEnum_StringBased_WithNonAlternateLookupComparer.Items.Should()
                                                          .HaveCount(2)
                                                          .And.Contain(SmartEnum_StringBased_WithNonAlternateLookupComparer.Item1)
                                                          .And.Contain(SmartEnum_StringBased_WithNonAlternateLookupComparer.Item2);
   }

   [Fact]
   public void Should_get_item_by_string_key()
   {
      SmartEnum_StringBased_WithNonAlternateLookupComparer.Get("Item1")
                                                          .Should().Be(SmartEnum_StringBased_WithNonAlternateLookupComparer.Item1);
   }

   [Fact]
   public void Should_try_get_item_by_string_key()
   {
      SmartEnum_StringBased_WithNonAlternateLookupComparer.TryGet("Item2", out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_StringBased_WithNonAlternateLookupComparer.Item2);
   }

   [Fact]
   public void Should_return_false_from_try_get_for_unknown_string_key()
   {
      SmartEnum_StringBased_WithNonAlternateLookupComparer.TryGet("unknown", out var item).Should().BeFalse();
      item.Should().BeNull();
   }

#if NET9_0_OR_GREATER
   [Fact]
   public void Should_get_item_by_ReadOnlySpanOfChar()
   {
      SmartEnum_StringBased_WithNonAlternateLookupComparer.Get("Item1".AsSpan())
                                                          .Should().Be(SmartEnum_StringBased_WithNonAlternateLookupComparer.Item1);
   }

   [Fact]
   public void Should_try_get_item_by_ReadOnlySpanOfChar()
   {
      SmartEnum_StringBased_WithNonAlternateLookupComparer.TryGet("Item2".AsSpan(), out var item).Should().BeTrue();
      item.Should().Be(SmartEnum_StringBased_WithNonAlternateLookupComparer.Item2);
   }

   [Fact]
   public void Should_return_false_from_try_get_by_ReadOnlySpanOfChar_for_unknown_key()
   {
      SmartEnum_StringBased_WithNonAlternateLookupComparer.TryGet("unknown".AsSpan(), out var item).Should().BeFalse();
      item.Should().BeNull();
   }
#endif
}
