using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;
using ArgumentName = Thinktecture.CodeAnalysis.ArgumentName;
using ITypedMemberState = Thinktecture.CodeAnalysis.ITypedMemberState;
using DefaultMemberState = Thinktecture.CodeAnalysis.DefaultMemberState;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.DefaultMemberStateTests;

public class Properties
{
   [Fact]
   public void Name_should_return_constructor_value()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var name = "PropertyName";

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         name,
         ArgumentName.Create("propertyName"));

      // Assert
      state.Name.Should().Be(name);
   }

   [Fact]
   public void ArgumentName_should_return_constructor_value()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var argumentName = ArgumentName.Create("myArgument");

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "MyProperty",
         argumentName);

      // Assert
      state.ArgumentName.Should().Be(argumentName);
   }

   [Fact]
   public void SpecialType_should_delegate_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(specialType: SpecialType.System_String);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));

      // Assert
      state.SpecialType.Should().Be(SpecialType.System_String);
   }

   [Fact]
   public void IsTypeParameter_should_be_true_when_type_kind_is_type_parameter()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: TypeKind.TypeParameter);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "T",
         ArgumentName.Create("t"));

      // Assert
      state.IsTypeParameter.Should().BeTrue();
   }

   [Theory]
   [InlineData(TypeKind.Class, false)]
   [InlineData(TypeKind.Struct, false)]
   [InlineData(TypeKind.Interface, false)]
   [InlineData(TypeKind.Enum, false)]
   [InlineData(TypeKind.Delegate, false)]
   [InlineData(TypeKind.Array, false)]
   [InlineData(TypeKind.TypeParameter, true)]
   public void IsTypeParameter_should_match_type_kind(TypeKind typeKind, bool expectedResult)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: typeKind);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Member",
         ArgumentName.Create("member"));

      // Assert
      state.IsTypeParameter.Should().Be(expectedResult);
   }

   [Fact]
   public void TypeFullyQualified_should_delegate_to_typed_member_state()
   {
      // Arrange
      var typeFullyQualified = "System.Collections.Generic.List<System.String>";
      var typedMemberState = CreateTypedMemberState(typeFullyQualified: typeFullyQualified);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Items",
         ArgumentName.Create("items"));

      // Assert
      state.TypeFullyQualified.Should().Be(typeFullyQualified);
   }

   [Fact]
   public void IsReferenceType_should_delegate_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         typeFullyQualified: "System.String",
         isReferenceType: true,
         isValueType: false);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));

      // Assert
      state.IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void IsValueType_should_delegate_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         typeFullyQualified: "System.Int32",
         isReferenceType: false,
         isValueType: true);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Count",
         ArgumentName.Create("count"));

      // Assert
      state.IsValueType.Should().BeTrue();
   }

   [Fact]
   public void IsNullableStruct_should_delegate_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(isNullableStruct: true);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "OptionalValue",
         ArgumentName.Create("optionalValue"));

      // Assert
      state.IsNullableStruct.Should().BeTrue();
   }

   [Fact]
   public void NullableAnnotation_should_delegate_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(nullableAnnotation: NullableAnnotation.Annotated);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));

      // Assert
      state.NullableAnnotation.Should().Be(NullableAnnotation.Annotated);
   }

   [Fact]
   public void IsInterface_should_be_true_when_type_kind_is_interface()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: TypeKind.Interface);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Service",
         ArgumentName.Create("service"));

      // Assert
      state.IsInterface.Should().BeTrue();
   }

   [Theory]
   [InlineData(TypeKind.Class, false)]
   [InlineData(TypeKind.Struct, false)]
   [InlineData(TypeKind.Interface, true)]
   [InlineData(TypeKind.Enum, false)]
   [InlineData(TypeKind.Delegate, false)]
   public void IsInterface_should_match_type_kind(TypeKind typeKind, bool expectedResult)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: typeKind);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Member",
         ArgumentName.Create("member"));

      // Assert
      state.IsInterface.Should().Be(expectedResult);
   }

   [Fact]
   public void IsRecord_should_always_return_false()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState(typeKind: TypeKind.Class);
      var typedMemberState2 = CreateTypedMemberState(typeKind: TypeKind.Struct);
      var typedMemberState3 = CreateTypedMemberState(typeKind: TypeKind.Interface);

      // Act
      var state1 = new DefaultMemberState(typedMemberState1, "Member1", ArgumentName.Create("member1"));
      var state2 = new DefaultMemberState(typedMemberState2, "Member2", ArgumentName.Create("member2"));
      var state3 = new DefaultMemberState(typedMemberState3, "Member3", ArgumentName.Create("member3"));

      // Assert
      state1.IsRecord.Should().BeFalse();
      state2.IsRecord.Should().BeFalse();
      state3.IsRecord.Should().BeFalse();
   }

   [Theory]
   [InlineData(SpecialType.None)]
   [InlineData(SpecialType.System_Object)]
   [InlineData(SpecialType.System_String)]
   [InlineData(SpecialType.System_Int32)]
   [InlineData(SpecialType.System_Decimal)]
   [InlineData(SpecialType.System_Boolean)]
   public void SpecialType_should_delegate_all_special_types(SpecialType specialType)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(specialType: specialType);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Value",
         ArgumentName.Create("value"));

      // Assert
      state.SpecialType.Should().Be(specialType);
   }

   [Theory]
   [InlineData(NullableAnnotation.None)]
   [InlineData(NullableAnnotation.Annotated)]
   [InlineData(NullableAnnotation.NotAnnotated)]
   public void NullableAnnotation_should_delegate_all_nullable_annotations(NullableAnnotation annotation)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(nullableAnnotation: annotation);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Value",
         ArgumentName.Create("value"));

      // Assert
      state.NullableAnnotation.Should().Be(annotation);
   }

   [Fact]
   public void Should_correctly_reflect_changes_in_underlying_typed_member_state_reference()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState(
         typeFullyQualified: "System.String",
         specialType: SpecialType.System_String);
      var typedMemberState2 = CreateTypedMemberState(
         typeFullyQualified: "System.Int32",
         specialType: SpecialType.System_Int32);

      var state1 = new DefaultMemberState(typedMemberState1, "Name", ArgumentName.Create("name"));
      var state2 = new DefaultMemberState(typedMemberState2, "Name", ArgumentName.Create("name"));

      // Assert - different underlying states should result in different property values
      state1.TypeFullyQualified.Should().Be("System.String");
      state1.SpecialType.Should().Be(SpecialType.System_String);

      state2.TypeFullyQualified.Should().Be("System.Int32");
      state2.SpecialType.Should().Be(SpecialType.System_Int32);
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
