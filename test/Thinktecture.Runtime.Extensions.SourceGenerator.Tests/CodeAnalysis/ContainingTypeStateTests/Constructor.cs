using System.Collections.Immutable;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.ContainingTypeStateTests;

public class Constructor
{
   [Fact]
   public void Should_initialize_name_property()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.Name.Should().Be("MyClass");
   }

   [Fact]
   public void Should_initialize_isReferenceType_property()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_initialize_isRecord_property()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyRecord", true, true, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.IsRecord.Should().BeTrue();
   }

   [Fact]
   public void Should_initialize_genericParameters_property()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("TKey", ImmutableArray<string>.Empty));

      // Act
      var state = new ContainingTypeState("MyClass", true, false, genericParams);

      // Assert
      state.GenericParameters.Should().Equal(genericParams);
   }

   [Fact]
   public void Should_handle_empty_name()
   {
      // Arrange & Act
      var state = new ContainingTypeState("", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.Name.Should().Be("");
   }

   [Fact]
   public void Should_handle_empty_generic_parameters()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyClass", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.GenericParameters.Should().BeEmpty();
   }

   [Fact]
   public void Should_handle_value_type()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyStruct", false, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.IsReferenceType.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_reference_type_record()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyRecord", true, true, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.IsReferenceType.Should().BeTrue();
      state.IsRecord.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_value_type_record()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyRecordStruct", false, true, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.IsReferenceType.Should().BeFalse();
      state.IsRecord.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_single_generic_parameter()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty));

      // Act
      var state = new ContainingTypeState("MyClass", true, false, genericParams);

      // Assert
      state.GenericParameters.Should().HaveCount(1);
      state.GenericParameters[0].Name.Should().Be("T");
   }

   [Fact]
   public void Should_handle_multiple_generic_parameters()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("TKey", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("TValue", ImmutableArray<string>.Empty));

      // Act
      var state = new ContainingTypeState("MyClass", true, false, genericParams);

      // Assert
      state.GenericParameters.Should().HaveCount(3);
      state.GenericParameters[0].Name.Should().Be("T");
      state.GenericParameters[1].Name.Should().Be("TKey");
      state.GenericParameters[2].Name.Should().Be("TValue");
   }

   [Fact]
   public void Should_handle_generic_parameters_with_constraints()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("T", ["class", "IDisposable"]));

      // Act
      var state = new ContainingTypeState("MyClass", true, false, genericParams);

      // Assert
      state.GenericParameters[0].Constraints.Should().Equal("class", "IDisposable");
   }

   [Fact]
   public void Should_preserve_generic_parameters_order()
   {
      // Arrange
      var genericParams = ImmutableArray.Create(
         new GenericTypeParameterState("TKey", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("T", ImmutableArray<string>.Empty),
         new GenericTypeParameterState("TValue", ImmutableArray<string>.Empty));

      // Act
      var state = new ContainingTypeState("MyClass", true, false, genericParams);

      // Assert
      state.GenericParameters[0].Name.Should().Be("TKey");
      state.GenericParameters[1].Name.Should().Be("T");
      state.GenericParameters[2].Name.Should().Be("TValue");
   }

   [Fact]
   public void Should_handle_name_with_special_characters()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyClass_123", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.Name.Should().Be("MyClass_123");
   }

   [Fact]
   public void Should_handle_name_with_generic_syntax()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyClass<T>", true, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.Name.Should().Be("MyClass<T>");
   }

   [Fact]
   public void Should_handle_all_false_boolean_flags()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyStruct", false, false, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.IsReferenceType.Should().BeFalse();
      state.IsRecord.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_all_true_boolean_flags()
   {
      // Arrange & Act
      var state = new ContainingTypeState("MyRecord", true, true, ImmutableArray<GenericTypeParameterState>.Empty);

      // Assert
      state.IsReferenceType.Should().BeTrue();
      state.IsRecord.Should().BeTrue();
   }
}
