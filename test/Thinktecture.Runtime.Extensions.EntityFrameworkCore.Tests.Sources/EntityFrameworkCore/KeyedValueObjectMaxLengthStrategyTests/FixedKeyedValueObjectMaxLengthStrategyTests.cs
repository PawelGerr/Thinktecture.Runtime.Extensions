using System;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.KeyedValueObjectMaxLengthStrategyTests;

public class FixedKeyedValueObjectMaxLengthStrategyTests
{
   [Fact]
   public void Constructor_should_throw_ArgumentOutOfRangeException_for_zero()
   {
      var act = () => new FixedKeyedValueObjectMaxLengthStrategy(0);

      act.Should().Throw<ArgumentOutOfRangeException>()
         .WithParameterName("maxLength")
         .WithMessage("*Max length must be greater than zero.*");
   }

   [Fact]
   public void Constructor_should_throw_ArgumentOutOfRangeException_for_negative_value()
   {
      var act = () => new FixedKeyedValueObjectMaxLengthStrategy(-1);

      act.Should().Throw<ArgumentOutOfRangeException>()
         .WithParameterName("maxLength")
         .WithMessage("*Max length must be greater than zero.*");
   }

   [Fact]
   public void OverwriteExistingMaxLength_should_be_false_by_default()
   {
      var strategy = new FixedKeyedValueObjectMaxLengthStrategy(50);

      strategy.OverwriteExistingMaxLength.Should().BeFalse();
   }

   [Fact]
   public void OverwriteExistingMaxLength_should_be_true_when_set()
   {
      var strategy = new FixedKeyedValueObjectMaxLengthStrategy(50, overwriteExistingMaxLength: true);

      strategy.OverwriteExistingMaxLength.Should().BeTrue();
   }

   [Fact]
   public void GetMaxLength_should_return_fixed_value_for_string_key_type()
   {
      var strategy = new FixedKeyedValueObjectMaxLengthStrategy(100);

      var result = strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(100);
   }

   [Fact]
   public void GetMaxLength_should_return_None_for_non_string_key_type()
   {
      var strategy = new FixedKeyedValueObjectMaxLengthStrategy(100);

      var result = strategy.GetMaxLength(typeof(object), typeof(int));

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_return_None_for_Guid_key_type()
   {
      var strategy = new FixedKeyedValueObjectMaxLengthStrategy(100);

      var result = strategy.GetMaxLength(typeof(object), typeof(Guid));

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_handle_extreme_value()
   {
      var strategy = new FixedKeyedValueObjectMaxLengthStrategy(int.MaxValue);

      var result = strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(int.MaxValue);
   }

   [Fact]
   public void GetMaxLength_should_handle_small_value()
   {
      var strategy = new FixedKeyedValueObjectMaxLengthStrategy(1);

      var result = strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(1);
   }

   [Fact]
   public void Equals_should_return_true_for_same_value_and_overwrite_setting()
   {
      var strategy1 = new FixedKeyedValueObjectMaxLengthStrategy(50, overwriteExistingMaxLength: true);
      var strategy2 = new FixedKeyedValueObjectMaxLengthStrategy(50, overwriteExistingMaxLength: true);

      strategy1.Equals(strategy2).Should().BeTrue();
   }

   [Fact]
   public void Equals_should_return_false_for_different_value()
   {
      var strategy1 = new FixedKeyedValueObjectMaxLengthStrategy(50);
      var strategy2 = new FixedKeyedValueObjectMaxLengthStrategy(100);

      strategy1.Equals(strategy2).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_false_for_different_OverwriteExistingMaxLength()
   {
      var strategy1 = new FixedKeyedValueObjectMaxLengthStrategy(50, overwriteExistingMaxLength: false);
      var strategy2 = new FixedKeyedValueObjectMaxLengthStrategy(50, overwriteExistingMaxLength: true);

      strategy1.Equals(strategy2).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_false_for_null()
   {
      var strategy = new FixedKeyedValueObjectMaxLengthStrategy(50);

      strategy.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_true_for_same_reference()
   {
      var strategy = new FixedKeyedValueObjectMaxLengthStrategy(50);

      strategy.Equals(strategy).Should().BeTrue();
   }

   [Fact]
   public void GetHashCode_should_be_consistent_with_Equals()
   {
      var strategy1 = new FixedKeyedValueObjectMaxLengthStrategy(50, overwriteExistingMaxLength: true);
      var strategy2 = new FixedKeyedValueObjectMaxLengthStrategy(50, overwriteExistingMaxLength: true);

      strategy1.GetHashCode().Should().Be(strategy2.GetHashCode());
   }
}
