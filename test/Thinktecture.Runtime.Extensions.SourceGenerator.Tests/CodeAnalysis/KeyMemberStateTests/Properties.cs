using Microsoft.CodeAnalysis;
using ArgumentName = Thinktecture.CodeAnalysis.ArgumentName;
using ImplementedComparisonOperators = Thinktecture.CodeAnalysis.ImplementedComparisonOperators;
using ImplementedOperators = Thinktecture.CodeAnalysis.ImplementedOperators;
using ITypedMemberState = Thinktecture.CodeAnalysis.ITypedMemberState;
using KeyMemberState = Thinktecture.CodeAnalysis.KeyMemberState;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.KeyMemberStateTests;

public class Properties
{
   [Fact]
   public void IsRecord_should_always_return_false()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsRecord.Should().BeFalse();
   }

   [Fact]
   public void Should_return_access_modifier_from_constructor()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Private,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.AccessModifier.Should().Be(Thinktecture.CodeAnalysis.AccessModifier.Private);
   }

   [Fact]
   public void Should_return_kind_from_constructor()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Field,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.Kind.Should().Be(Thinktecture.CodeAnalysis.MemberKind.Field);
   }

   [Fact]
   public void Should_return_name_from_constructor()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "_customKey",
         ArgumentName.Create("key"));

      // Act & Assert
      state.Name.Should().Be("_customKey");
   }

   [Fact]
   public void Should_return_argument_name_from_constructor()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var argumentName = ArgumentName.Create("customArg");
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         argumentName);

      // Act & Assert
      state.ArgumentName.Should().Be(argumentName);
   }

   [Fact]
   public void Should_delegate_type_fully_qualified_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeFullyQualified: "System.String");
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.TypeFullyQualified.Should().Be("System.String");
   }

   [Fact]
   public void Should_delegate_is_reference_type_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         typeFullyQualified: "System.String",
         isReferenceType: true,
         isValueType: false);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_is_value_type_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         isReferenceType: false,
         isValueType: true);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsValueType.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_special_type_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(specialType: SpecialType.System_String);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.SpecialType.Should().Be(SpecialType.System_String);
   }

   [Fact]
   public void IsTypeParameter_should_return_true_when_type_kind_is_type_parameter()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: TypeKind.TypeParameter);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsTypeParameter.Should().BeTrue();
   }

   [Fact]
   public void IsTypeParameter_should_return_false_when_type_kind_is_not_type_parameter()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: TypeKind.Struct);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsTypeParameter.Should().BeFalse();
   }

   [Fact]
   public void IsInterface_should_return_true_when_type_kind_is_interface()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: TypeKind.Interface);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsInterface.Should().BeTrue();
   }

   [Fact]
   public void IsInterface_should_return_false_when_type_kind_is_not_interface()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: TypeKind.Class);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsInterface.Should().BeFalse();
   }

   [Fact]
   public void Should_delegate_is_formattable_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(isFormattable: true);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsFormattable.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_is_comparable_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(isComparable: true);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsComparable.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_is_parsable_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(isParsable: true);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsParsable.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_is_to_string_return_type_nullable_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(isToStringReturnTypeNullable: true);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsToStringReturnTypeNullable.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_comparison_operators_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(comparisonOperators: ImplementedComparisonOperators.All);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.ComparisonOperators.Should().Be(ImplementedComparisonOperators.All);
   }

   [Fact]
   public void Should_delegate_addition_operators_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(additionOperators: ImplementedOperators.All);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.AdditionOperators.Should().Be(ImplementedOperators.All);
   }

   [Fact]
   public void Should_delegate_subtraction_operators_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(subtractionOperators: ImplementedOperators.Checked);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.SubtractionOperators.Should().Be(ImplementedOperators.Checked);
   }

   [Fact]
   public void Should_delegate_multiply_operators_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(multiplyOperators: ImplementedOperators.Default);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.MultiplyOperators.Should().Be(ImplementedOperators.Default);
   }

   [Fact]
   public void Should_delegate_division_operators_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(divisionOperators: ImplementedOperators.All);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.DivisionOperators.Should().Be(ImplementedOperators.All);
   }

   [Fact]
   public void Should_delegate_is_nullable_struct_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(isNullableStruct: true);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.IsNullableStruct.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_nullable_annotation_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(nullableAnnotation: NullableAnnotation.Annotated);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.NullableAnnotation.Should().Be(NullableAnnotation.Annotated);
   }

   [Theory]
   [InlineData(SpecialType.System_Int32)]
   [InlineData(SpecialType.System_String)]
   [InlineData(SpecialType.System_Decimal)]
   [InlineData(SpecialType.System_Double)]
   [InlineData(SpecialType.None)]
   public void Should_handle_different_special_types(SpecialType specialType)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(specialType: specialType);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.SpecialType.Should().Be(specialType);
   }

   [Theory]
   [InlineData(TypeKind.Class)]
   [InlineData(TypeKind.Struct)]
   [InlineData(TypeKind.Interface)]
   [InlineData(TypeKind.Enum)]
   [InlineData(TypeKind.TypeParameter)]
   public void Should_handle_different_type_kinds(TypeKind typeKind)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: typeKind);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      // Verify the property is correctly checking against TypeParameter
      var isTypeParameter = typeKind == TypeKind.TypeParameter;
      state.IsTypeParameter.Should().Be(isTypeParameter);

      var isInterface = typeKind == TypeKind.Interface;
      state.IsInterface.Should().Be(isInterface);
   }

   [Theory]
   [InlineData(NullableAnnotation.None)]
   [InlineData(NullableAnnotation.NotAnnotated)]
   [InlineData(NullableAnnotation.Annotated)]
   public void Should_handle_different_nullable_annotations(NullableAnnotation nullableAnnotation)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(nullableAnnotation: nullableAnnotation);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.NullableAnnotation.Should().Be(nullableAnnotation);
   }

   [Theory]
   [InlineData(ImplementedComparisonOperators.None)]
   [InlineData(ImplementedComparisonOperators.GreaterThan)]
   [InlineData(ImplementedComparisonOperators.LessThan)]
   [InlineData(ImplementedComparisonOperators.All)]
   public void Should_handle_different_comparison_operators(ImplementedComparisonOperators operators)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(comparisonOperators: operators);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.ComparisonOperators.Should().Be(operators);
   }

   [Theory]
   [InlineData(ImplementedOperators.None)]
   [InlineData(ImplementedOperators.Default)]
   [InlineData(ImplementedOperators.Checked)]
   [InlineData(ImplementedOperators.All)]
   public void Should_handle_different_arithmetic_operators(ImplementedOperators operators)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         additionOperators: operators,
         subtractionOperators: operators,
         multiplyOperators: operators,
         divisionOperators: operators);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      state.AdditionOperators.Should().Be(operators);
      state.SubtractionOperators.Should().Be(operators);
      state.MultiplyOperators.Should().Be(operators);
      state.DivisionOperators.Should().Be(operators);
   }

   [Fact]
   public void Should_handle_complex_type_with_all_properties_set()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         typeFullyQualified: "System.Decimal",
         isReferenceType: false,
         isValueType: true,
         specialType: SpecialType.System_Decimal,
         typeKind: TypeKind.Struct,
         isFormattable: true,
         isComparable: true,
         isParsable: true,
         isToStringReturnTypeNullable: false,
         comparisonOperators: ImplementedComparisonOperators.All,
         additionOperators: ImplementedOperators.All,
         subtractionOperators: ImplementedOperators.All,
         multiplyOperators: ImplementedOperators.All,
         divisionOperators: ImplementedOperators.All,
         isNullableStruct: false,
         nullableAnnotation: NullableAnnotation.None);

      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Private,
         Thinktecture.CodeAnalysis.MemberKind.Field,
         "_amount",
         ArgumentName.Create("amount"));

      // Act & Assert
      state.AccessModifier.Should().Be(Thinktecture.CodeAnalysis.AccessModifier.Private);
      state.Kind.Should().Be(Thinktecture.CodeAnalysis.MemberKind.Field);
      state.Name.Should().Be("_amount");
      state.ArgumentName.Should().Be(ArgumentName.Create("amount"));
      state.IsRecord.Should().BeFalse();
      state.TypeFullyQualified.Should().Be("System.Decimal");
      state.IsReferenceType.Should().BeFalse();
      state.IsValueType.Should().BeTrue();
      state.SpecialType.Should().Be(SpecialType.System_Decimal);
      state.IsTypeParameter.Should().BeFalse();
      state.IsInterface.Should().BeFalse();
      state.IsFormattable.Should().BeTrue();
      state.IsComparable.Should().BeTrue();
      state.IsParsable.Should().BeTrue();
      state.IsToStringReturnTypeNullable.Should().BeFalse();
      state.ComparisonOperators.Should().Be(ImplementedComparisonOperators.All);
      state.AdditionOperators.Should().Be(ImplementedOperators.All);
      state.SubtractionOperators.Should().Be(ImplementedOperators.All);
      state.MultiplyOperators.Should().Be(ImplementedOperators.All);
      state.DivisionOperators.Should().Be(ImplementedOperators.All);
      state.IsNullableStruct.Should().BeFalse();
      state.NullableAnnotation.Should().Be(NullableAnnotation.None);
   }

   private static ITypedMemberState CreateTypedMemberState(
      string typeFullyQualified = "System.Int32",
      bool isReferenceType = false,
      bool isValueType = true,
      SpecialType specialType = SpecialType.System_Int32,
      TypeKind typeKind = TypeKind.Struct,
      bool isFormattable = false,
      bool isComparable = false,
      bool isParsable = false,
      bool isToStringReturnTypeNullable = false,
      ImplementedComparisonOperators comparisonOperators = ImplementedComparisonOperators.None,
      ImplementedOperators additionOperators = ImplementedOperators.None,
      ImplementedOperators subtractionOperators = ImplementedOperators.None,
      ImplementedOperators multiplyOperators = ImplementedOperators.None,
      ImplementedOperators divisionOperators = ImplementedOperators.None,
      bool isNullableStruct = false,
      NullableAnnotation nullableAnnotation = NullableAnnotation.None)
   {
      return new TestTypedMemberState(
         typeFullyQualified,
         isReferenceType,
         isValueType,
         specialType,
         typeKind,
         isFormattable,
         isComparable,
         isParsable,
         isToStringReturnTypeNullable,
         comparisonOperators,
         additionOperators,
         subtractionOperators,
         multiplyOperators,
         divisionOperators,
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
      public bool IsFormattable { get; }
      public bool IsComparable { get; }
      public bool IsParsable { get; }
      public bool IsSpanParsable { get; }
      public bool IsToStringReturnTypeNullable { get; }
      public ImplementedComparisonOperators ComparisonOperators { get; }
      public ImplementedOperators AdditionOperators { get; }
      public ImplementedOperators SubtractionOperators { get; }
      public ImplementedOperators MultiplyOperators { get; }
      public ImplementedOperators DivisionOperators { get; }

      public TestTypedMemberState(
         string typeFullyQualified,
         bool isReferenceType,
         bool isValueType,
         SpecialType specialType,
         TypeKind typeKind,
         bool isFormattable,
         bool isComparable,
         bool isParsable,
         bool isToStringReturnTypeNullable,
         ImplementedComparisonOperators comparisonOperators,
         ImplementedOperators additionOperators,
         ImplementedOperators subtractionOperators,
         ImplementedOperators multiplyOperators,
         ImplementedOperators divisionOperators,
         bool isNullableStruct,
         NullableAnnotation nullableAnnotation,
         bool isSpanParsable = false)
      {
         TypeFullyQualified = typeFullyQualified;
         TypeMinimallyQualified = typeFullyQualified;
         IsReferenceType = isReferenceType;
         IsValueType = isValueType;
         SpecialType = specialType;
         TypeKind = typeKind;
         IsFormattable = isFormattable;
         IsComparable = isComparable;
         IsParsable = isParsable;
         IsSpanParsable = isSpanParsable;
         IsToStringReturnTypeNullable = isToStringReturnTypeNullable;
         ComparisonOperators = comparisonOperators;
         AdditionOperators = additionOperators;
         SubtractionOperators = subtractionOperators;
         MultiplyOperators = multiplyOperators;
         DivisionOperators = divisionOperators;
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
                && NullableAnnotation == other.NullableAnnotation
                && IsFormattable == other.IsFormattable
                && IsComparable == other.IsComparable
                && IsParsable == other.IsParsable
                && IsToStringReturnTypeNullable == other.IsToStringReturnTypeNullable
                && ComparisonOperators == other.ComparisonOperators
                && AdditionOperators == other.AdditionOperators
                && SubtractionOperators == other.SubtractionOperators
                && MultiplyOperators == other.MultiplyOperators
                && DivisionOperators == other.DivisionOperators;
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
            hashCode = (hashCode * 397) ^ IsFormattable.GetHashCode();
            hashCode = (hashCode * 397) ^ IsComparable.GetHashCode();
            hashCode = (hashCode * 397) ^ IsParsable.GetHashCode();
            hashCode = (hashCode * 397) ^ IsToStringReturnTypeNullable.GetHashCode();
            hashCode = (hashCode * 397) ^ (int)ComparisonOperators;
            hashCode = (hashCode * 397) ^ (int)AdditionOperators;
            hashCode = (hashCode * 397) ^ (int)SubtractionOperators;
            hashCode = (hashCode * 397) ^ (int)MultiplyOperators;
            hashCode = (hashCode * 397) ^ (int)DivisionOperators;
            return hashCode;
         }
      }
   }
}
