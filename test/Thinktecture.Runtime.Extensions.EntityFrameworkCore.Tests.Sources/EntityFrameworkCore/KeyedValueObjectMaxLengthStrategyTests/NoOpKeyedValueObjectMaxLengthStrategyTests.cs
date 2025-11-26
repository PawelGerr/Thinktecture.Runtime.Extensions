using System;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.KeyedValueObjectMaxLengthStrategyTests;

public class NoOpKeyedValueObjectMaxLengthStrategyTests
{
   private readonly NoOpKeyedValueObjectMaxLengthStrategy _strategy = new();

   [Fact]
   public void Instance_should_return_singleton_instance()
   {
      NoOpKeyedValueObjectMaxLengthStrategy.Instance.Should().NotBeNull();
      NoOpKeyedValueObjectMaxLengthStrategy.Instance.Should().BeOfType<NoOpKeyedValueObjectMaxLengthStrategy>();
   }

   [Fact]
   public void OverwriteExistingMaxLength_should_always_be_false()
   {
      _strategy.OverwriteExistingMaxLength.Should().BeFalse();
      NoOpKeyedValueObjectMaxLengthStrategy.Instance.OverwriteExistingMaxLength.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_always_return_None()
   {
      var result = _strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_return_None_regardless_of_key_type()
   {
      var result1 = _strategy.GetMaxLength(typeof(object), typeof(string));
      var result2 = _strategy.GetMaxLength(typeof(object), typeof(int));
      var result3 = _strategy.GetMaxLength(typeof(object), typeof(Guid));

      result1.IsSet.Should().BeFalse();
      result2.IsSet.Should().BeFalse();
      result3.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_return_None_for_various_types()
   {
      var result1 = _strategy.GetMaxLength(typeof(string), typeof(string));
      var result2 = _strategy.GetMaxLength(typeof(int), typeof(int));
      var result3 = _strategy.GetMaxLength(typeof(object), typeof(decimal));

      result1.IsSet.Should().BeFalse();
      result2.IsSet.Should().BeFalse();
      result3.IsSet.Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_true_for_another_instance()
   {
      var other = new NoOpKeyedValueObjectMaxLengthStrategy();

      _strategy.Equals(other).Should().BeTrue();
   }

   [Fact]
   public void Equals_should_return_true_for_singleton_instance()
   {
      _strategy.Equals(NoOpKeyedValueObjectMaxLengthStrategy.Instance).Should().BeTrue();
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
      var other = new NoOpKeyedValueObjectMaxLengthStrategy();

      _strategy.GetHashCode().Should().Be(other.GetHashCode());
      _strategy.GetHashCode().Should().Be(NoOpKeyedValueObjectMaxLengthStrategy.Instance.GetHashCode());
   }
}
