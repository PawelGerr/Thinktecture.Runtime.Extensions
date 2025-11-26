using System;
using System.Collections.Generic;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.ConfigurationTests;

public class KeyedValueObjectConfigurationTests
{
   [Fact]
   public void NoMaxLength_should_have_no_op_keyed_value_object_max_length_strategy()
   {
      KeyedValueObjectConfiguration.NoMaxLength.MaxLengthStrategy.Should().BeOfType<NoOpKeyedValueObjectMaxLengthStrategy>();
   }

   [Fact]
   public void MaxLengthStrategy_getter_should_return_no_op_when_null()
   {
      var config = new KeyedValueObjectConfiguration();
      config.MaxLengthStrategy.Should().BeOfType<NoOpKeyedValueObjectMaxLengthStrategy>();
   }

   [Fact]
   public void Equals_should_return_true_for_same_strategy()
   {
      var config1 = new KeyedValueObjectConfiguration
      {
         MaxLengthStrategy = new FixedKeyedValueObjectMaxLengthStrategy(50)
      };

      var config2 = new KeyedValueObjectConfiguration
      {
         MaxLengthStrategy = new FixedKeyedValueObjectMaxLengthStrategy(50)
      };

      config1.Equals(config2).Should().BeTrue();
   }

   [Fact]
   public void Equals_should_return_false_for_different_strategy()
   {
      var config1 = new KeyedValueObjectConfiguration
      {
         MaxLengthStrategy = new FixedKeyedValueObjectMaxLengthStrategy(50)
      };

      var config2 = new KeyedValueObjectConfiguration
      {
         MaxLengthStrategy = new FixedKeyedValueObjectMaxLengthStrategy(100)
      };

      config1.Equals(config2).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_false_for_null()
   {
      KeyedValueObjectConfiguration.NoMaxLength.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_true_for_same_reference()
   {
      var config = KeyedValueObjectConfiguration.NoMaxLength;
      config.Equals(config).Should().BeTrue();
   }

   [Fact]
   public void GetHashCode_should_be_consistent_with_Equals()
   {
      var config1 = new KeyedValueObjectConfiguration
      {
         MaxLengthStrategy = new FixedKeyedValueObjectMaxLengthStrategy(50)
      };

      var config2 = new KeyedValueObjectConfiguration
      {
         MaxLengthStrategy = new FixedKeyedValueObjectMaxLengthStrategy(50)
      };

      config1.GetHashCode().Should().Be(config2.GetHashCode());
   }
}
