using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;
using ArgumentName = Thinktecture.CodeAnalysis.ArgumentName;
using ITypedMemberState = Thinktecture.CodeAnalysis.ITypedMemberState;
using KeyMemberState = Thinktecture.CodeAnalysis.KeyMemberState;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.KeyMemberStateTests;

public class Constructor
{
   [Fact]
   public void Should_initialize_all_properties()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var accessModifier = Thinktecture.CodeAnalysis.AccessModifier.Public;
      var kind = Thinktecture.CodeAnalysis.MemberKind.Property;
      var name = "Key";
      var argumentName = ArgumentName.Create("key");

      // Act
      var state = new KeyMemberState(typedMemberState, accessModifier, kind, name, argumentName);

      // Assert
      state.AccessModifier.Should().Be(accessModifier);
      state.Kind.Should().Be(kind);
      state.Name.Should().Be(name);
      state.ArgumentName.Should().Be(argumentName);
   }

   [Fact]
   public void Should_initialize_with_private_field()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var accessModifier = Thinktecture.CodeAnalysis.AccessModifier.Private;
      var kind = Thinktecture.CodeAnalysis.MemberKind.Field;
      var name = "_key";
      var argumentName = ArgumentName.Create("key");

      // Act
      var state = new KeyMemberState(typedMemberState, accessModifier, kind, name, argumentName);

      // Assert
      state.AccessModifier.Should().Be(Thinktecture.CodeAnalysis.AccessModifier.Private);
      state.Kind.Should().Be(Thinktecture.CodeAnalysis.MemberKind.Field);
      state.Name.Should().Be("_key");
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
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

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
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.IsRecord.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_empty_name()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         string.Empty,
         ArgumentName.Create("key"));

      // Assert
      state.Name.Should().Be(string.Empty);
   }

   [Fact]
   public void Should_handle_name_with_special_characters()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var name = "_key123Value";

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Private,
         Thinktecture.CodeAnalysis.MemberKind.Field,
         name,
         ArgumentName.Create("key"));

      // Assert
      state.Name.Should().Be(name);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Private)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.PrivateProtected)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Protected)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.ProtectedInternal)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Internal)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Public)]
   public void Should_handle_all_access_modifiers(Thinktecture.CodeAnalysis.AccessModifier accessModifier)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         accessModifier,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.AccessModifier.Should().Be(accessModifier);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.MemberKind.Field)]
   [InlineData(Thinktecture.CodeAnalysis.MemberKind.Property)]
   public void Should_handle_all_member_kinds(Thinktecture.CodeAnalysis.MemberKind kind)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         kind,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.Kind.Should().Be(kind);
   }

   [Fact]
   public void Should_delegate_comparison_operators_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         comparisonOperators: ImplementedComparisonOperators.All);

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.ComparisonOperators.Should().Be(ImplementedComparisonOperators.All);
   }

   [Fact]
   public void Should_delegate_arithmetic_operators_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         additionOperators: ImplementedOperators.All,
         subtractionOperators: ImplementedOperators.All,
         multiplyOperators: ImplementedOperators.Checked,
         divisionOperators: ImplementedOperators.Default);

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.AdditionOperators.Should().Be(ImplementedOperators.All);
      state.SubtractionOperators.Should().Be(ImplementedOperators.All);
      state.MultiplyOperators.Should().Be(ImplementedOperators.Checked);
      state.DivisionOperators.Should().Be(ImplementedOperators.Default);
   }

   [Fact]
   public void Should_delegate_interface_flags_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         isFormattable: true,
         isComparable: true,
         isParsable: true);

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.IsFormattable.Should().BeTrue();
      state.IsComparable.Should().BeTrue();
      state.IsParsable.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_nullable_properties_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         isNullableStruct: true,
         nullableAnnotation: NullableAnnotation.Annotated);

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.IsNullableStruct.Should().BeTrue();
      state.NullableAnnotation.Should().Be(NullableAnnotation.Annotated);
   }

   [Fact]
   public void Should_delegate_type_parameter_check_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: TypeKind.TypeParameter);

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.IsTypeParameter.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_interface_check_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(typeKind: TypeKind.Interface);

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.IsInterface.Should().BeTrue();
   }

   [Fact]
   public void Should_delegate_to_string_nullability_to_typed_member_state()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(isToStringReturnTypeNullable: true);

      // Act
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Assert
      state.IsToStringReturnTypeNullable.Should().BeTrue();
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
         NullableAnnotation nullableAnnotation)
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
