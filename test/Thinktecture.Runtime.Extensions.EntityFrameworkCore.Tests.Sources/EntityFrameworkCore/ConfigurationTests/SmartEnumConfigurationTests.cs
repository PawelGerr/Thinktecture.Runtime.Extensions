using System;
using System.Collections.Generic;
using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.ConfigurationTests;

public class SmartEnumConfigurationTests
{
   [Fact]
   public void Default_should_have_default_smart_enum_max_length_strategy()
   {
      SmartEnumConfiguration.Default.MaxLengthStrategy.Should().BeOfType<DefaultSmartEnumMaxLengthStrategy>();
   }

   [Fact]
   public void NoMaxLength_should_have_no_op_smart_enum_max_length_strategy()
   {
      SmartEnumConfiguration.NoMaxLength.MaxLengthStrategy.Should().BeOfType<NoOpSmartEnumMaxLengthStrategy>();
   }

   [Fact]
   public void MaxLengthStrategy_getter_should_return_default_when_null()
   {
      var config = new SmartEnumConfiguration();
      config.MaxLengthStrategy.Should().BeOfType<DefaultSmartEnumMaxLengthStrategy>();
   }

   [Fact]
   public void Equals_should_return_true_for_same_strategy()
   {
      var config1 = new SmartEnumConfiguration
      {
         MaxLengthStrategy = new FixedSmartEnumMaxLengthStrategy(50)
      };

      var config2 = new SmartEnumConfiguration
      {
         MaxLengthStrategy = new FixedSmartEnumMaxLengthStrategy(50)
      };

      config1.Equals(config2).Should().BeTrue();
   }

   [Fact]
   public void Equals_should_return_false_for_different_strategy()
   {
      var config1 = new SmartEnumConfiguration
      {
         MaxLengthStrategy = new FixedSmartEnumMaxLengthStrategy(50)
      };

      var config2 = new SmartEnumConfiguration
      {
         MaxLengthStrategy = new FixedSmartEnumMaxLengthStrategy(100)
      };

      config1.Equals(config2).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_false_for_null()
   {
      SmartEnumConfiguration.Default.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_true_for_same_reference()
   {
      var config = SmartEnumConfiguration.Default;
      config.Equals(config).Should().BeTrue();
   }

   [Fact]
   public void GetHashCode_should_be_consistent_with_Equals()
   {
      var config1 = new SmartEnumConfiguration
      {
         MaxLengthStrategy = new FixedSmartEnumMaxLengthStrategy(50)
      };

      var config2 = new SmartEnumConfiguration
      {
         MaxLengthStrategy = new FixedSmartEnumMaxLengthStrategy(50)
      };

      config1.GetHashCode().Should().Be(config2.GetHashCode());
   }
}
