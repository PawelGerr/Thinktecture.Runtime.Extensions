using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;
using ArgumentName = Thinktecture.CodeAnalysis.ArgumentName;
using ITypedMemberState = Thinktecture.CodeAnalysis.ITypedMemberState;
using DefaultMemberState = Thinktecture.CodeAnalysis.DefaultMemberState;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.DefaultMemberStateTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_same_hash_code_for_equal_instances()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_same_instance()
   {
      // Arrange
      var state = CreateDefaultMemberState();

      // Act
      var hashCode1 = state.GetHashCode();
      var hashCode2 = state.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_equal_instances_with_different_typed_member_state_instances()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_for_different_names()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_for_different_argument_names()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_for_different_typed_member_states()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_handle_empty_name()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_be_case_sensitive()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_when_only_typed_member_state_differs_by_special_type()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_when_typed_member_state_differs_by_nullable_annotation()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_when_typed_member_state_differs_by_type_kind()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_complex_equal_instances()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_for_whitespace_differences_in_names()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_when_argument_name_differs_by_render_as_is()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_when_argument_names_have_same_render_as_is()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_when_typed_member_state_differs_by_is_reference_type()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_codes_when_typed_member_state_differs_by_is_nullable_struct()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_states_with_identical_empty_argument_names()
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_satisfy_equals_hash_code_contract()
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

      state1.GetHashCode().Should().Be(state2.GetHashCode());
   }

   [Theory]
   [InlineData("Name1", "name1", "Name2", "name2")]
   [InlineData("_field", "field", "_value", "value")]
   [InlineData("Property", "property", "Property", "prop")]
   public void Should_return_different_hash_codes_for_different_instances(
      string name1, string argName1, string name2, string argName2)
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
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_be_consistent_across_multiple_calls()
   {
      // Arrange
      var state = CreateDefaultMemberState();

      // Act
      var hashCode1 = state.GetHashCode();
      var hashCode2 = state.GetHashCode();
      var hashCode3 = state.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
      hashCode2.Should().Be(hashCode3);
   }

   [Fact]
   public void Should_handle_hash_code_collisions_gracefully()
   {
      // Arrange - Create multiple different instances and collect their hash codes
      var typedMemberState = CreateTypedMemberState();
      var states = new[]
      {
         new DefaultMemberState(typedMemberState, "Name1", ArgumentName.Create("name1")),
         new DefaultMemberState(typedMemberState, "Name2", ArgumentName.Create("name2")),
         new DefaultMemberState(typedMemberState, "Name3", ArgumentName.Create("name3")),
         new DefaultMemberState(typedMemberState, "Value1", ArgumentName.Create("value1")),
         new DefaultMemberState(typedMemberState, "Value2", ArgumentName.Create("value2"))
      };

      // Act
      var hashCodes = states.Select(s => s.GetHashCode()).ToList();

      // Assert - Not all hash codes should be the same (though collisions are possible)
      hashCodes.Distinct().Count().Should().BeGreaterThan(1);
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
