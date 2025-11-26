using System;
using System.Collections.Generic;
using System.Linq;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.SmartEnumMaxLengthStrategyTests;

public class CustomSmartEnumMaxLengthStrategyTests
{
   private sealed record TestSmartEnumItem(object Key, object Item, string Identifier) : ISmartEnumItem;

   [Fact]
   public void Constructor_should_throw_ArgumentNullException_for_null_calculator()
   {
      var act = () => new CustomSmartEnumMaxLengthStrategy(null!);

      act.Should().Throw<ArgumentNullException>().WithParameterName("calculator");
   }

   [Fact]
   public void OverwriteExistingMaxLength_should_be_false_by_default()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => MaxLengthChange.None);

      strategy.OverwriteExistingMaxLength.Should().BeFalse();
   }

   [Fact]
   public void OverwriteExistingMaxLength_should_be_true_when_set()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => MaxLengthChange.None, overwriteExistingMaxLength: true);

      strategy.OverwriteExistingMaxLength.Should().BeTrue();
   }

   [Fact]
   public void GetMaxLength_should_return_Set_when_function_returns_value()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => new MaxLengthChange(50));
      var items = new List<ISmartEnumItem>();

      var result = strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(50);
   }

   [Fact]
   public void GetMaxLength_should_return_None_when_function_returns_None()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => MaxLengthChange.None);
      var items = new List<ISmartEnumItem>();

      var result = strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_return_Clear_when_function_returns_null()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => new MaxLengthChange(null));
      var items = new List<ISmartEnumItem>();

      var result = strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().BeNull();
   }

   [Fact]
   public void GetMaxLength_should_receive_correct_items_collection()
   {
      IReadOnlyList<ISmartEnumItem> receivedItems = null;
      var strategy = new CustomSmartEnumMaxLengthStrategy((_, _, items) =>
      {
         receivedItems = items;
         return MaxLengthChange.None;
      });

      var items = new List<ISmartEnumItem>
                  {
                     new TestSmartEnumItem("A", new object(), "Item1")
                  };

      strategy.GetMaxLength(typeof(object), typeof(string), items);

      receivedItems.Should().BeSameAs(items);
   }

   [Fact]
   public void GetMaxLength_should_receive_correct_type_parameter()
   {
      Type receivedType = null;
      var strategy = new CustomSmartEnumMaxLengthStrategy((type, _, _) =>
      {
         receivedType = type;
         return MaxLengthChange.None;
      });

      var items = new List<ISmartEnumItem>();
      strategy.GetMaxLength(typeof(string), typeof(int), items);

      receivedType.Should().Be(typeof(string));
   }

   [Fact]
   public void GetMaxLength_should_receive_correct_keyType_parameter()
   {
      Type receivedKeyType = null;
      var strategy = new CustomSmartEnumMaxLengthStrategy((_, keyType, _) =>
      {
         receivedKeyType = keyType;
         return MaxLengthChange.None;
      });

      var items = new List<ISmartEnumItem>();
      strategy.GetMaxLength(typeof(string), typeof(int), items);

      receivedKeyType.Should().Be(typeof(int));
   }

   [Fact]
   public void GetMaxLength_should_handle_negative_value()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => new MaxLengthChange(-1));
      var items = new List<ISmartEnumItem>();

      var result = strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(-1);
   }

   [Fact]
   public void GetMaxLength_should_handle_zero_value()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => new MaxLengthChange(0));
      var items = new List<ISmartEnumItem>();

      var result = strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(0);
   }

   [Fact]
   public void GetMaxLength_should_handle_extreme_value()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => new MaxLengthChange(int.MaxValue));
      var items = new List<ISmartEnumItem>();

      var result = strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(int.MaxValue);
   }

   [Fact]
   public void GetMaxLength_should_use_items_to_calculate_max_length()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, keyType, items) =>
      {
         if (keyType != typeof(string))
            return MaxLengthChange.None;

         var max = items.OfType<TestSmartEnumItem>()
                        .Where(i => i.Key is string)
                        .Max(i => ((string)i.Key).Length);

         return new MaxLengthChange(max);
      });

      var items = new List<ISmartEnumItem>
                  {
                     new TestSmartEnumItem("A", new object(), "Item1"),
                     new TestSmartEnumItem("ABCDE", new object(), "Item2"),
                     new TestSmartEnumItem("ABC", new object(), "Item3")
                  };

      var result = strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(5);
   }

   [Fact]
   public void Equals_should_return_true_for_same_instance()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => MaxLengthChange.None);

      strategy.Equals(strategy).Should().BeTrue();
   }

   [Fact]
   public void Equals_should_return_false_for_different_OverwriteExistingMaxLength()
   {
      Func<Type, Type, IReadOnlyList<ISmartEnumItem>, MaxLengthChange> func = (_, _, _) => MaxLengthChange.None;
      var strategy1 = new CustomSmartEnumMaxLengthStrategy(func, overwriteExistingMaxLength: false);
      var strategy2 = new CustomSmartEnumMaxLengthStrategy(func, overwriteExistingMaxLength: true);

      strategy1.Equals(strategy2).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_false_for_null()
   {
      var strategy = new CustomSmartEnumMaxLengthStrategy(static (_, _, _) => MaxLengthChange.None);

      strategy.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void GetHashCode_should_be_consistent_with_Equals()
   {
      Func<Type, Type, IReadOnlyList<ISmartEnumItem>, MaxLengthChange> func = (_, _, _) => MaxLengthChange.None;
      var strategy1 = new CustomSmartEnumMaxLengthStrategy(func, overwriteExistingMaxLength: true);
      var strategy2 = new CustomSmartEnumMaxLengthStrategy(func, overwriteExistingMaxLength: true);

      strategy1.GetHashCode().Should().Be(strategy2.GetHashCode());
   }
}
