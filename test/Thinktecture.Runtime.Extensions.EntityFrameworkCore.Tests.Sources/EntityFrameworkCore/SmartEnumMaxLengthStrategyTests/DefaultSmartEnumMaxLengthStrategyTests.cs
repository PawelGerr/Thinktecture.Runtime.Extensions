using System;
using System.Collections.Generic;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.SmartEnumMaxLengthStrategyTests;

public class DefaultSmartEnumMaxLengthStrategyTests
{
   private sealed record TestSmartEnumItem(object Key, object Item, string Identifier) : ISmartEnumItem;

   private readonly DefaultSmartEnumMaxLengthStrategy _strategy = new();

   [Fact]
   public void OverwriteExistingMaxLength_should_be_false()
   {
      _strategy.OverwriteExistingMaxLength.Should().BeFalse();
   }

   [Fact]
   public void Instance_should_return_singleton_instance()
   {
      DefaultSmartEnumMaxLengthStrategy.Instance.Should().NotBeNull();
      DefaultSmartEnumMaxLengthStrategy.Instance.Should().BeOfType<DefaultSmartEnumMaxLengthStrategy>();
   }

   [Fact]
   public void GetMaxLength_should_return_None_for_empty_items_collection()
   {
      var result = _strategy.GetMaxLength(typeof(string), typeof(string), Array.Empty<ISmartEnumItem>());

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_return_None_for_non_string_key_type()
   {
      var items = new List<ISmartEnumItem>
                  {
                     new TestSmartEnumItem(42, new object(), "Item1")
                  };

      var result = _strategy.GetMaxLength(typeof(object), typeof(int), items);

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_compute_correct_length_for_single_string_item()
   {
      var items = new List<ISmartEnumItem>
                  {
                     new TestSmartEnumItem("ABC", new object(), "Item1")
                  };

      var result = _strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(10); // 3 rounded up to next multiple of 10
   }

   [Fact]
   public void GetMaxLength_should_find_maximum_length_from_multiple_items()
   {
      var items = new List<ISmartEnumItem>
                  {
                     new TestSmartEnumItem("A", new object(), "Item1"),
                     new TestSmartEnumItem("ABCDEFGHIJ", new object(), "Item2"),
                     new TestSmartEnumItem("ABC", new object(), "Item3")
                  };

      var result = _strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(10);
   }

   [Fact]
   public void GetMaxLength_should_round_up_to_next_multiple_of_10()
   {
      var items = new List<ISmartEnumItem>
                  {
                     new TestSmartEnumItem("ABCDEFGHIJK", new object(), "Item1") // 11 characters
                  };

      var result = _strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(20); // 11 rounded up to 20
   }

   [Fact]
   public void GetMaxLength_should_handle_unicode_characters_correctly()
   {
      var items = new List<ISmartEnumItem>
                  {
                     new TestSmartEnumItem("😀😀😀", new object(), "Item1") // 3 emoji characters
                  };

      var result = _strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      // Each emoji is 1 character in .NET string length
      result.Value.Should().Be(10); // 3 rounded up to 10
   }

   [Fact]
   public void GetMaxLength_should_handle_very_long_strings()
   {
      var longString = new string('A', 95);
      var items = new List<ISmartEnumItem>
                  {
                     new TestSmartEnumItem(longString, new object(), "Item1")
                  };

      var result = _strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(100); // 95 rounded up to 100
   }

   [Fact]
   public void GetMaxLength_should_throw_ArgumentNullException_for_null_type()
   {
      var items = new List<ISmartEnumItem>();

      var act = () => _strategy.GetMaxLength(null!, typeof(string), items);

      act.Should().Throw<ArgumentNullException>().WithParameterName("type");
   }

   [Fact]
   public void GetMaxLength_should_throw_ArgumentNullException_for_null_keyType()
   {
      var items = new List<ISmartEnumItem>();

      var act = () => _strategy.GetMaxLength(typeof(object), null!, items);

      act.Should().Throw<ArgumentNullException>().WithParameterName("keyType");
   }

   [Fact]
   public void GetMaxLength_should_throw_ArgumentNullException_for_null_items()
   {
      var act = () => _strategy.GetMaxLength(typeof(object), typeof(string), null!);

      act.Should().Throw<ArgumentNullException>().WithParameterName("items");
   }

   [Fact]
   public void Equals_should_return_true_for_another_instance()
   {
      var other = new DefaultSmartEnumMaxLengthStrategy();

      _strategy.Equals(other).Should().BeTrue();
   }

   [Fact]
   public void Equals_should_return_false_for_null()
   {
      _strategy.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_true_for_same_reference()
   {
      _strategy.Equals(_strategy).Should().BeTrue();
   }

   [Fact]
   public void GetHashCode_should_be_same_for_all_instances()
   {
      var other = new DefaultSmartEnumMaxLengthStrategy();

      _strategy.GetHashCode().Should().Be(other.GetHashCode());
   }
}
