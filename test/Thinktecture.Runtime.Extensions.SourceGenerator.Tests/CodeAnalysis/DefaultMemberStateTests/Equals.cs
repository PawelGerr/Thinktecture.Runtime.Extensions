using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;
using ArgumentName = Thinktecture.CodeAnalysis.ArgumentName;
using ITypedMemberState = Thinktecture.CodeAnalysis.ITypedMemberState;
using DefaultMemberState = Thinktecture.CodeAnalysis.DefaultMemberState;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.DefaultMemberStateTests;

public class Equals
{
   [Fact]
   public void Should_return_true_for_same_instance()
   {
      // Arrange
      var state = CreateDefaultMemberState();

      // Act
      var result = state.Equals(state);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_reference_equal_instances()
   {
      // Arrange
      var state = CreateDefaultMemberState();
      DefaultMemberState sameReference = state;

      // Act
      var result = state.Equals(sameReference);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_instances()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_with_equal_typed_member_states()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState("System.String");
      var typedMemberState2 = CreateTypedMemberState("System.String");
      var state1 = new DefaultMemberState(
         typedMemberState1,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState2,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_different_names()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState,
         "Value",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_argument_names()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("value"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_typed_member_states()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState("System.String");
      var typedMemberState2 = CreateTypedMemberState("System.Int32");
      var state1 = new DefaultMemberState(
         typedMemberState1,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState2,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_null_typed_parameter()
   {
      // Arrange
      var state = CreateDefaultMemberState();

      // Act
      var result = state.Equals(null);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_type_object()
   {
      // Arrange
      var state = CreateDefaultMemberState();
      var differentType = new object();

      // Act
      var result = state.Equals(differentType);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_equal_instances_via_object_overload()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));
      object state2 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_empty_name_equality()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         string.Empty,
         ArgumentName.Create("value"));
      var state2 = new DefaultMemberState(
         typedMemberState,
         string.Empty,
         ArgumentName.Create("value"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_be_case_sensitive_for_name_comparison()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState,
         "name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_only_typed_member_state_differs_by_special_type()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState("System.String", specialType: SpecialType.System_String);
      var typedMemberState2 = CreateTypedMemberState("System.String", specialType: SpecialType.None);
      var state1 = new DefaultMemberState(
         typedMemberState1,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState2,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_typed_member_state_differs_by_nullable_annotation()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState(nullableAnnotation: NullableAnnotation.Annotated);
      var typedMemberState2 = CreateTypedMemberState(nullableAnnotation: NullableAnnotation.None);
      var state1 = new DefaultMemberState(
         typedMemberState1,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState2,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_typed_member_state_differs_by_type_kind()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState(typeKind: TypeKind.Class);
      var typedMemberState2 = CreateTypedMemberState(typeKind: TypeKind.Struct);
      var state1 = new DefaultMemberState(
         typedMemberState1,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState2,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_complex_equal_instances()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState(
         typeFullyQualified: "System.Decimal",
         isReferenceType: false,
         isValueType: true,
         specialType: SpecialType.System_Decimal,
         typeKind: TypeKind.Struct,
         isNullableStruct: false,
         nullableAnnotation: NullableAnnotation.None);
      var typedMemberState2 = CreateTypedMemberState(
         typeFullyQualified: "System.Decimal",
         isReferenceType: false,
         isValueType: true,
         specialType: SpecialType.System_Decimal,
         typeKind: TypeKind.Struct,
         isNullableStruct: false,
         nullableAnnotation: NullableAnnotation.None);

      var state1 = new DefaultMemberState(
         typedMemberState1,
         "_amount",
         ArgumentName.Create("amount"));
      var state2 = new DefaultMemberState(
         typedMemberState2,
         "_amount",
         ArgumentName.Create("amount"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_whitespace_differences_in_names()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState,
         "Name ",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_argument_name_differs_by_render_as_is()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name", renderAsIs: false));
      var state2 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name", renderAsIs: true));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_argument_names_have_same_render_as_is()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name", renderAsIs: true));
      var state2 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name", renderAsIs: true));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_typed_member_state_differs_by_is_reference_type()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState(
         typeFullyQualified: "CustomType",
         isReferenceType: true,
         isValueType: false);
      var typedMemberState2 = CreateTypedMemberState(
         typeFullyQualified: "CustomType",
         isReferenceType: false,
         isValueType: true);
      var state1 = new DefaultMemberState(
         typedMemberState1,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState2,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_typed_member_state_differs_by_is_nullable_struct()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState(isNullableStruct: true);
      var typedMemberState2 = CreateTypedMemberState(isNullableStruct: false);
      var state1 = new DefaultMemberState(
         typedMemberState1,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState2,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_null_object_overload()
   {
      // Arrange
      var state = CreateDefaultMemberState();
      object? nullObject = null;

      // Act
      var result = state.Equals(nullObject);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_for_states_with_identical_empty_argument_names()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create(string.Empty));
      var state2 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create(string.Empty));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_empty_vs_non_empty_argument_names()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create(string.Empty));
      var state2 = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().BeFalse();
   }

   [Theory]
   [InlineData("Name", "name", "Name", "name", true)]
   [InlineData("Name", "name", "Name", "value", false)]
   [InlineData("Name", "name", "Value", "name", false)]
   [InlineData("_field", "field", "_field", "field", true)]
   [InlineData("_field", "field", "_field", "value", false)]
   public void Should_handle_various_name_and_argument_name_combinations(
      string name1, string argName1, string name2, string argName2, bool expectedEqual)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new DefaultMemberState(
         typedMemberState,
         name1,
         ArgumentName.Create(argName1));
      var state2 = new DefaultMemberState(
         typedMemberState,
         name2,
         ArgumentName.Create(argName2));

      // Act
      var result = state1.Equals(state2);

      // Assert
      result.Should().Be(expectedEqual);
   }

   [Fact]
   public void Should_use_structural_equality_not_reference_equality_for_typed_member_state()
   {
      // Arrange - Create two different but equal typed member state instances
      var typedMemberState1 = CreateTypedMemberState(
         typeFullyQualified: "System.String",
         specialType: SpecialType.System_String,
         typeKind: TypeKind.Class,
         isReferenceType: true,
         isValueType: false);
      var typedMemberState2 = CreateTypedMemberState(
         typeFullyQualified: "System.String",
         specialType: SpecialType.System_String,
         typeKind: TypeKind.Class,
         isReferenceType: true,
         isValueType: false);

      // Ensure they are different instances but structurally equal
      ReferenceEquals(typedMemberState1, typedMemberState2).Should().BeFalse();
      typedMemberState1.Equals(typedMemberState2).Should().BeTrue();

      var state1 = new DefaultMemberState(
         typedMemberState1,
         "Name",
         ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(
         typedMemberState2,
         "Name",
         ArgumentName.Create("name"));

      // Act
      var result = state1.Equals(state2);

      // Assert - Should be equal even though typed member states are different instances
      result.Should().BeTrue();
   }

   private static DefaultMemberState CreateDefaultMemberState(
      string name = "Name",
      string argumentName = "name")
   {
      return new DefaultMemberState(
         CreateTypedMemberState(),
         name,
         ArgumentName.Create(argumentName));
   }

   private static ITypedMemberState CreateTypedMemberState(
      string typeFullyQualified = "System.Int32",
      bool isReferenceType = false,
      bool isValueType = true,
      SpecialType specialType = SpecialType.System_Int32,
      TypeKind typeKind = TypeKind.Struct,
      bool isNullableStruct = false,
      NullableAnnotation nullableAnnotation = NullableAnnotation.None)
   {
      return new TestTypedMemberState(
         typeFullyQualified,
         isReferenceType,
         isValueType,
         specialType,
         typeKind,
         isNullableStruct,
         nullableAnnotation);
   }

   private sealed class TestTypedMemberState : ITypedMemberState
   {
      public string TypeFullyQualified { get; }
      public string TypeMinimallyQualified { get; }
      public SpecialType SpecialType { get; }
      public TypeKind TypeKind { get; }
      public bool IsReferenceType { get; }
      public bool IsValueType { get; }
      public bool IsTypeParameter => TypeKind == TypeKind.TypeParameter;
      public bool IsReferenceTypeOrNullableStruct => IsReferenceType || IsNullableStruct;
      public bool IsNullableStruct { get; }
      public NullableAnnotation NullableAnnotation { get; }
      public bool IsFormattable => false;
      public bool IsComparable => false;
      public bool IsParsable => false;
      public bool IsSpanParsable => false;
      public bool IsToStringReturnTypeNullable => false;
      public ImplementedComparisonOperators ComparisonOperators => ImplementedComparisonOperators.None;
      public ImplementedOperators AdditionOperators => ImplementedOperators.None;
      public ImplementedOperators SubtractionOperators => ImplementedOperators.None;
      public ImplementedOperators MultiplyOperators => ImplementedOperators.None;
      public ImplementedOperators DivisionOperators => ImplementedOperators.None;

      public TestTypedMemberState(
         string typeFullyQualified,
         bool isReferenceType,
         bool isValueType,
         SpecialType specialType,
         TypeKind typeKind,
         bool isNullableStruct,
         NullableAnnotation nullableAnnotation)
      {
         TypeFullyQualified = typeFullyQualified;
         TypeMinimallyQualified = typeFullyQualified;
         IsReferenceType = isReferenceType;
         IsValueType = isValueType;
         SpecialType = specialType;
         TypeKind = typeKind;
         IsNullableStruct = isNullableStruct;
         NullableAnnotation = nullableAnnotation;
      }

      public bool Equals(ITypedMemberState? other)
      {
         if (other is null)
            return false;
         if (ReferenceEquals(this, other))
            return true;

         return TypeFullyQualified == other.TypeFullyQualified
                && SpecialType == other.SpecialType
                && TypeKind == other.TypeKind
                && IsReferenceType == other.IsReferenceType
                && IsValueType == other.IsValueType
                && IsNullableStruct == other.IsNullableStruct
                && NullableAnnotation == other.NullableAnnotation;
      }

      public override bool Equals(object? obj)
      {
         return obj is TestTypedMemberState other && Equals(other);
      }

      public override int GetHashCode()
      {
         unchecked
         {
            var hashCode = TypeFullyQualified.GetHashCode();
            hashCode = (hashCode * 397) ^ (int)SpecialType;
            hashCode = (hashCode * 397) ^ (int)TypeKind;
            hashCode = (hashCode * 397) ^ IsReferenceType.GetHashCode();
            hashCode = (hashCode * 397) ^ IsValueType.GetHashCode();
            hashCode = (hashCode * 397) ^ IsNullableStruct.GetHashCode();
            hashCode = (hashCode * 397) ^ (int)NullableAnnotation;
            return hashCode;
         }
      }
   }
}
