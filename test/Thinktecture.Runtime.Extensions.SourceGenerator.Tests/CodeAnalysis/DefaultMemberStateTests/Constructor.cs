using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;
using ArgumentName = Thinktecture.CodeAnalysis.ArgumentName;
using ITypedMemberState = Thinktecture.CodeAnalysis.ITypedMemberState;
using DefaultMemberState = Thinktecture.CodeAnalysis.DefaultMemberState;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.DefaultMemberStateTests;

public class Constructor
{
   [Fact]
   public void Should_initialize_all_properties()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var name = "MyProperty";
      var argumentName = ArgumentName.Create("myProperty");

      // Act
      var state = new DefaultMemberState(typedMemberState, name, argumentName);

      // Assert
      state.Name.Should().Be(name);
      state.ArgumentName.Should().Be(argumentName);
   }

   [Fact]
   public void Should_delegate_type_properties_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         typeFullyQualified: "System.String",
         isReferenceType: true,
         isValueType: false,
         specialType: SpecialType.System_String);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));

      // Assert
      state.TypeFullyQualified.Should().Be("System.String");
      state.IsReferenceType.Should().BeTrue();
      state.IsValueType.Should().BeFalse();
      state.SpecialType.Should().Be(SpecialType.System_String);
   }

   [Fact]
   public void Should_set_is_record_to_false()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Name",
         ArgumentName.Create("name"));

      // Assert
      state.IsRecord.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_empty_name()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         string.Empty,
         ArgumentName.Create("value"));

      // Assert
      state.Name.Should().Be(string.Empty);
   }

   [Fact]
   public void Should_handle_name_with_special_characters()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var name = "_myProperty123Value";

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         name,
         ArgumentName.Create("value"));

      // Assert
      state.Name.Should().Be(name);
   }

   [Fact]
   public void Should_delegate_is_type_parameter_to_typed_member_state()
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

   [Fact]
   public void Should_delegate_is_interface_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: TypeKind.Interface);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "IMyInterface",
         ArgumentName.Create("myInterface"));

      // Assert
      state.IsInterface.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_nullable_struct_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         isNullableStruct: true,
         nullableAnnotation: NullableAnnotation.Annotated);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Value",
         ArgumentName.Create("value"));

      // Assert
      state.IsNullableStruct.Should().BeTrue();
      state.NullableAnnotation.Should().Be(NullableAnnotation.Annotated);
   }

   [Fact]
   public void Should_handle_argument_name_with_render_as_is()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var argumentName = ArgumentName.Create("MyParam", renderAsIs: true);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "MyProperty",
         argumentName);

      // Assert
      state.ArgumentName.Should().Be(argumentName);
      state.ArgumentName.RenderAsIs.Should().BeTrue();
   }

   [Theory]
   [InlineData("Property1", "property1")]
   [InlineData("_field", "field")]
   [InlineData("MyValue", "myValue")]
   [InlineData("ID", "id")]
   public void Should_handle_various_name_and_argument_name_combinations(string name, string argName)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var argumentName = ArgumentName.Create(argName);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         name,
         argumentName);

      // Assert
      state.Name.Should().Be(name);
      state.ArgumentName.Name.Should().Be(argName);
   }

   [Fact]
   public void Should_delegate_all_special_types_correctly()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(specialType: SpecialType.System_Decimal);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Amount",
         ArgumentName.Create("amount"));

      // Assert
      state.SpecialType.Should().Be(SpecialType.System_Decimal);
   }

   [Theory]
   [InlineData(TypeKind.Class)]
   [InlineData(TypeKind.Struct)]
   [InlineData(TypeKind.Interface)]
   [InlineData(TypeKind.Enum)]
   [InlineData(TypeKind.TypeParameter)]
   public void Should_correctly_identify_type_parameter_for_various_type_kinds(TypeKind typeKind)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: typeKind);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Member",
         ArgumentName.Create("member"));

      // Assert
      state.IsTypeParameter.Should().Be(typeKind == TypeKind.TypeParameter);
   }

   [Theory]
   [InlineData(TypeKind.Class)]
   [InlineData(TypeKind.Struct)]
   [InlineData(TypeKind.Interface)]
   [InlineData(TypeKind.Enum)]
   [InlineData(TypeKind.TypeParameter)]
   public void Should_correctly_identify_interface_for_various_type_kinds(TypeKind typeKind)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: typeKind);

      // Act
      var state = new DefaultMemberState(
         typedMemberState,
         "Member",
         ArgumentName.Create("member"));

      // Assert
      state.IsInterface.Should().Be(typeKind == TypeKind.Interface);
   }

   [Theory]
   [InlineData(NullableAnnotation.None)]
   [InlineData(NullableAnnotation.Annotated)]
   [InlineData(NullableAnnotation.NotAnnotated)]
   public void Should_delegate_nullable_annotation(NullableAnnotation annotation)
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
