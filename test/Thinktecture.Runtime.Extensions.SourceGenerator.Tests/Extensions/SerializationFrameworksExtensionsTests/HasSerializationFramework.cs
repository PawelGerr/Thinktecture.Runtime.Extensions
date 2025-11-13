namespace Thinktecture.Runtime.Tests.SerializationFrameworksExtensionsTests;

public class HasSerializationFramework
{
   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.Json, Thinktecture.CodeAnalysis.SerializationFrameworks.Json, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.All, Thinktecture.CodeAnalysis.SerializationFrameworks.All, true)]
   public void Should_return_true_when_checking_for_exact_same_flag(Thinktecture.CodeAnalysis.SerializationFrameworks value, Thinktecture.CodeAnalysis.SerializationFrameworks flag, bool expected)
   {
      // Act
      var result = value.HasSerializationFramework(flag);

      // Assert
      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.None, Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.None, Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.None, Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, false)]
   public void Should_return_false_when_checking_for_flag_that_is_not_set(Thinktecture.CodeAnalysis.SerializationFrameworks value, Thinktecture.CodeAnalysis.SerializationFrameworks flag, bool expected)
   {
      // Act
      var result = value.HasSerializationFramework(flag);

      // Assert
      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.Json, Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.Json, Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.All, Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.All, Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.All, Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.All, Thinktecture.CodeAnalysis.SerializationFrameworks.Json, true)]
   public void Should_return_true_when_checking_for_single_flag_that_is_part_of_combined_value(Thinktecture.CodeAnalysis.SerializationFrameworks value, Thinktecture.CodeAnalysis.SerializationFrameworks flag, bool expected)
   {
      // Act
      var result = value.HasSerializationFramework(flag);

      // Assert
      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.Json, Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.Json, Thinktecture.CodeAnalysis.SerializationFrameworks.All, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, Thinktecture.CodeAnalysis.SerializationFrameworks.Json, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, Thinktecture.CodeAnalysis.SerializationFrameworks.Json, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, Thinktecture.CodeAnalysis.SerializationFrameworks.All, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, Thinktecture.CodeAnalysis.SerializationFrameworks.All, false)]
   public void Should_return_false_when_checking_for_combined_flag_but_not_all_component_flags_are_set(Thinktecture.CodeAnalysis.SerializationFrameworks value, Thinktecture.CodeAnalysis.SerializationFrameworks flag, bool expected)
   {
      // Act
      var result = value.HasSerializationFramework(flag);

      // Assert
      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson | Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, Thinktecture.CodeAnalysis.SerializationFrameworks.Json, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson | Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson | Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, true)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson | Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson | Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, Thinktecture.CodeAnalysis.SerializationFrameworks.All, true)]
   public void Should_return_true_when_checking_for_flag_using_bitwise_OR_combination(Thinktecture.CodeAnalysis.SerializationFrameworks value, Thinktecture.CodeAnalysis.SerializationFrameworks flag, bool expected)
   {
      // Act
      var result = value.HasSerializationFramework(flag);

      // Assert
      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson | Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, Thinktecture.CodeAnalysis.SerializationFrameworks.Json, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson | Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack, Thinktecture.CodeAnalysis.SerializationFrameworks.Json, false)]
   [InlineData(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson | Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson, Thinktecture.CodeAnalysis.SerializationFrameworks.All, false)]
   public void Should_return_false_when_checking_for_flag_using_bitwise_OR_combination_but_not_all_flags_are_set(Thinktecture.CodeAnalysis.SerializationFrameworks value, Thinktecture.CodeAnalysis.SerializationFrameworks flag, bool expected)
   {
      // Act
      var result = value.HasSerializationFramework(flag);

      // Assert
      result.Should().Be(expected);
   }

   [Fact]
   public void Should_return_true_when_all_individual_flags_are_checked_against_All()
   {
      // Arrange
      var value = Thinktecture.CodeAnalysis.SerializationFrameworks.All;

      // Act & Assert
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson).Should().BeTrue();
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson).Should().BeTrue();
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack).Should().BeTrue();
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.Json).Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_checking_for_any_flag_against_None()
   {
      // Arrange
      var value = Thinktecture.CodeAnalysis.SerializationFrameworks.None;

      // Act & Assert
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.None).Should().BeFalse();
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson).Should().BeFalse();
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.NewtonsoftJson).Should().BeFalse();
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack).Should().BeFalse();
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.Json).Should().BeFalse();
      value.HasSerializationFramework(Thinktecture.CodeAnalysis.SerializationFrameworks.All).Should().BeFalse();
   }
}
