using Thinktecture.EntityFrameworkCore;

namespace Thinktecture.Runtime.Tests.EntityFrameworkCore.ConfigurationTests;

public class ConfigurationTests
{
   [Fact]
   public void Default_should_have_default_smart_enum_configuration()
   {
      Configuration.Default.SmartEnums.Should().Be(SmartEnumConfiguration.Default);
   }

   [Fact]
   public void Default_should_have_no_max_length_for_keyed_value_objects()
   {
      Configuration.Default.KeyedValueObjects.Should().Be(KeyedValueObjectConfiguration.NoMaxLength);
   }

   [Fact]
   public void Default_should_use_constructor_for_read()
   {
      Configuration.Default.UseConstructorForRead.Should().BeTrue();
   }

   [Fact]
   public void NoMaxLength_should_have_no_max_length_for_smart_enums()
   {
      Configuration.NoMaxLength.SmartEnums.Should().Be(SmartEnumConfiguration.NoMaxLength);
   }

   [Fact]
   public void NoMaxLength_should_have_no_max_length_for_keyed_value_objects()
   {
      Configuration.NoMaxLength.KeyedValueObjects.Should().Be(KeyedValueObjectConfiguration.NoMaxLength);
   }

   [Fact]
   public void Equals_should_return_true_for_same_configuration_values()
   {
      var config1 = new Configuration
                    {
                       SmartEnums = SmartEnumConfiguration.Default,
                       KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength,
                       UseConstructorForRead = true
                    };

      var config2 = new Configuration
                    {
                       SmartEnums = SmartEnumConfiguration.Default,
                       KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength,
                       UseConstructorForRead = true
                    };

      config1.Equals(config2).Should().BeTrue();
   }

   [Fact]
   public void Equals_should_return_false_for_different_smart_enum_configuration()
   {
      var config1 = new Configuration
                    {
                       SmartEnums = SmartEnumConfiguration.Default,
                       KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength
                    };

      var config2 = new Configuration
                    {
                       SmartEnums = SmartEnumConfiguration.NoMaxLength,
                       KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength
                    };

      config1.Equals(config2).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_false_for_different_use_constructor_for_read()
   {
      var config1 = new Configuration
                    {
                       UseConstructorForRead = true
                    };

      var config2 = new Configuration
                    {
                       UseConstructorForRead = false
                    };

      config1.Equals(config2).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_false_for_null()
   {
      Configuration.Default.Equals(null).Should().BeFalse();
   }

   [Fact]
   public void Equals_should_return_true_for_same_reference()
   {
      var config = Configuration.Default;
      config.Equals(config).Should().BeTrue();
   }

   [Fact]
   public void GetHashCode_should_be_consistent_with_Equals()
   {
      var config1 = new Configuration
                    {
                       SmartEnums = SmartEnumConfiguration.Default,
                       KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength,
                       UseConstructorForRead = true
                    };

      var config2 = new Configuration
                    {
                       SmartEnums = SmartEnumConfiguration.Default,
                       KeyedValueObjects = KeyedValueObjectConfiguration.NoMaxLength,
                       UseConstructorForRead = true
                    };

      config1.GetHashCode().Should().Be(config2.GetHashCode());
   }
}
