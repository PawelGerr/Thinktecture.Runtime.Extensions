using System;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.KeyedValueObjectMaxLengthStrategyTests;

public class CustomKeyedValueObjectMaxLengthStrategyTests
{
   [Fact]
   public void Constructor_should_throw_ArgumentNullException_for_null_calculator()
   {
      var act = () => new CustomKeyedValueObjectMaxLengthStrategy(null!);

      act.Should().Throw<ArgumentNullException>().WithParameterName("calculator");
   }

   [Fact]
   public void OverwriteExistingMaxLength_should_be_false_by_default()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => MaxLengthChange.None);

      strategy.OverwriteExistingMaxLength.Should().BeFalse();
   }

   [Fact]
   public void OverwriteExistingMaxLength_should_be_true_when_set()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => MaxLengthChange.None, overwriteExistingMaxLength: true);

      strategy.OverwriteExistingMaxLength.Should().BeTrue();
   }

   [Fact]
   public void GetMaxLength_should_return_Set_when_function_returns_value()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => new MaxLengthChange(50));

      var result = strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(50);
   }

   [Fact]
   public void GetMaxLength_should_return_None_when_function_returns_None()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => MaxLengthChange.None);

      var result = strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeFalse();
   }

   [Fact]
   public void GetMaxLength_should_return_Clear_when_function_returns_null()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => new MaxLengthChange(null));

      var result = strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeTrue();
      result.Value.Should().BeNull();
   }

   [Fact]
   public void GetMaxLength_should_receive_correct_type_parameter()
   {
      Type receivedType = null;
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((type, _) =>
      {
         receivedType = type;
         return MaxLengthChange.None;
      });

      strategy.GetMaxLength(typeof(string), typeof(int));

      receivedType.Should().Be(typeof(string));
   }

   [Fact]
   public void GetMaxLength_should_receive_correct_keyType_parameter()
   {
      Type receivedKeyType = null;
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, keyType) =>
      {
         receivedKeyType = keyType;
         return MaxLengthChange.None;
      });

      strategy.GetMaxLength(typeof(string), typeof(int));

      receivedKeyType.Should().Be(typeof(int));
   }

   [Fact]
   public void GetMaxLength_should_handle_negative_value()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => new MaxLengthChange(-1));

      var result = strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(-1);
   }

   [Fact]
   public void GetMaxLength_should_handle_zero_value()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => new MaxLengthChange(0));

      var result = strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(0);
   }

   [Fact]
   public void GetMaxLength_should_handle_extreme_value()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => new MaxLengthChange(int.MaxValue));

      var result = strategy.GetMaxLength(typeof(object), typeof(string));

      result.IsSet.Should().BeTrue();
      result.Value.Should().Be(int.MaxValue);
   }

   [Fact]
   public void GetMaxLength_should_support_key_type_based_logic()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, keyType) =>
      {
         if (keyType == typeof(string))
            return new MaxLengthChange(100);

         return MaxLengthChange.None;
      });

      var result1 = strategy.GetMaxLength(typeof(object), typeof(string));
      var result2 = strategy.GetMaxLength(typeof(object), typeof(int));

      result1.IsSet.Should().BeTrue();
      result1.Value.Should().Be(100);
      result2.IsSet.Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_true_for_same_instance()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => MaxLengthChange.None);

      strategy.Equals(strategy).Should().BeTrue();
   }

   [Fact]
   public void Equals_should_return_false_for_different_OverwriteExistingMaxLength()
   {
      Func<Type, Type, MaxLengthChange> func = (_, _) => MaxLengthChange.None;
      var strategy1 = new CustomKeyedValueObjectMaxLengthStrategy(func, overwriteExistingMaxLength: false);
      var strategy2 = new CustomKeyedValueObjectMaxLengthStrategy(func, overwriteExistingMaxLength: true);

      strategy1.Equals(strategy2).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_false_for_null()
   {
      var strategy = new CustomKeyedValueObjectMaxLengthStrategy((_, _) => MaxLengthChange.None);

      strategy.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void GetHashCode_should_be_consistent_with_Equals()
   {
      Func<Type, Type, MaxLengthChange> func = (_, _) => MaxLengthChange.None;
      var strategy1 = new CustomKeyedValueObjectMaxLengthStrategy(func, overwriteExistingMaxLength: true);
      var strategy2 = new CustomKeyedValueObjectMaxLengthStrategy(func, overwriteExistingMaxLength: true);

      strategy1.GetHashCode().Should().Be(strategy2.GetHashCode());
   }
}
