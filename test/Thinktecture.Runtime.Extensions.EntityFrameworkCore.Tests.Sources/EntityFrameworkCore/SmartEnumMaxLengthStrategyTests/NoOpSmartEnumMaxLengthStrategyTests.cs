using System;
using System.Collections.Generic;
using System.Linq;
using Thinktecture.EntityFrameworkCore;


namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.SmartEnumMaxLengthStrategyTests;

public class NoOpSmartEnumMaxLengthStrategyTests
{
   private sealed record TestSmartEnumItem(object Key, object Item, string Identifier) : ISmartEnumItem;

   private readonly NoOpSmartEnumMaxLengthStrategy _strategy = new();

   [Fact]
   public void Instance_should_return_singleton_instance()
   {
      NoOpSmartEnumMaxLengthStrategy.Instance.Should().NotBeNull();
      NoOpSmartEnumMaxLengthStrategy.Instance.Should().BeOfType<NoOpSmartEnumMaxLengthStrategy>();
   }

   [Fact]
   public void OverwriteExistingMaxLength_should_always_be_false()
   {
      _strategy.OverwriteExistingMaxLength.Should().BeFalse();
      NoOpSmartEnumMaxLengthStrategy.Instance.OverwriteExistingMaxLength.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_always_return_None()
   {
      var items = new List<ISmartEnumItem>();

      var result = _strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_return_None_regardless_of_key_type()
   {
      var items = new List<ISmartEnumItem>();

      var result1 = _strategy.GetMaxLength(typeof(object), typeof(string), items);
      var result2 = _strategy.GetMaxLength(typeof(object), typeof(int), items);
      var result3 = _strategy.GetMaxLength(typeof(object), typeof(Guid), items);

      result1.IsSet.Should().BeFalse();
      result2.IsSet.Should().BeFalse();
      result3.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_work_with_empty_items()
   {
      var items = Array.Empty<ISmartEnumItem>();

      var result = _strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_work_with_populated_items()
   {
      var items = new List<ISmartEnumItem>
      {
         new TestSmartEnumItem("ABC", new object(), "Item1"),
         new TestSmartEnumItem("DEFGHIJ", new object(), "Item2")
      };

      var result = _strategy.GetMaxLength(typeof(object), typeof(string), items);

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_true_for_another_instance()
   {
      var other = new NoOpSmartEnumMaxLengthStrategy();

      _strategy.Equals(other).Should().BeTrue();
   }

   [Fact]
   public void Equals_should_return_true_for_singleton_instance()
   {
      _strategy.Equals(NoOpSmartEnumMaxLengthStrategy.Instance).Should().BeTrue();
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
      var other = new NoOpSmartEnumMaxLengthStrategy();

      _strategy.GetHashCode().Should().Be(other.GetHashCode());
      _strategy.GetHashCode().Should().Be(NoOpSmartEnumMaxLengthStrategy.Instance.GetHashCode());
   }
}
