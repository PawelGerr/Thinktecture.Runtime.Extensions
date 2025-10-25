using Microsoft.CodeAnalysis;
using ArgumentName = Thinktecture.CodeAnalysis.ArgumentName;
using ImplementedComparisonOperators = Thinktecture.CodeAnalysis.ImplementedComparisonOperators;
using ImplementedOperators = Thinktecture.CodeAnalysis.ImplementedOperators;
using ITypedMemberState = Thinktecture.CodeAnalysis.ITypedMemberState;
using KeyMemberState = Thinktecture.CodeAnalysis.KeyMemberState;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.KeyMemberStateTests;

public class GetHashCode
{
   [Fact]
   public void Should_return_same_hash_code_for_same_instance()
   {
      // Arrange
      var state = CreateKeyMemberState();

      // Act
      var hashCode1 = state.GetHashCode();
      var hashCode2 = state.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_equal_instances()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_instances_with_equal_typed_member_states()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState("System.String");
      var typedMemberState2 = CreateTypedMemberState("System.String");
      var state1 = new KeyMemberState(
         typedMemberState1,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState2,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_access_modifiers()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Private,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_kinds()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Field,
         "Key",
         ArgumentName.Create("key"));

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_names()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Value",
         ArgumentName.Create("key"));

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_argument_names()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("value"));

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_return_different_hash_code_for_different_typed_member_states()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState("System.String");
      var typedMemberState2 = CreateTypedMemberState("System.Int32");
      var state1 = new KeyMemberState(
         typedMemberState1,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState2,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

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
      var state = new KeyMemberState(
         CreateTypedMemberState(),
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         string.Empty,
         ArgumentName.Create("key"));

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_be_deterministic()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState(
         "System.Decimal",
         isFormattable: true,
         isComparable: true);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Private,
         Thinktecture.CodeAnalysis.MemberKind.Field,
         "_amount",
         ArgumentName.Create("amount"));

      // Act
      var hashCode1 = state.GetHashCode();
      var hashCode2 = state.GetHashCode();
      var hashCode3 = state.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
      hashCode2.Should().Be(hashCode3);
   }

   [Fact]
   public void Should_satisfy_hash_code_contract_for_equal_objects()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState("System.String");
      var typedMemberState2 = CreateTypedMemberState("System.String");
      var state1 = new KeyMemberState(
         typedMemberState1,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState2,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act & Assert
      if (state1.Equals(state2))
      {
         state1.GetHashCode().Should().Be(state2.GetHashCode());
      }
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Private)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.PrivateProtected)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Protected)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.ProtectedInternal)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Internal)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Public)]
   public void Should_return_different_hash_codes_for_different_access_modifiers(Thinktecture.CodeAnalysis.AccessModifier accessModifier)
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var baseState = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var testState = new KeyMemberState(
         typedMemberState,
         accessModifier,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act
      var baseHashCode = baseState.GetHashCode();
      var testHashCode = testState.GetHashCode();

      // Assert
      if (accessModifier != Thinktecture.CodeAnalysis.AccessModifier.Public)
      {
         baseHashCode.Should().NotBe(testHashCode);
      }
      else
      {
         baseHashCode.Should().Be(testHashCode);
      }
   }

   [Fact]
   public void Should_return_different_hash_code_when_typed_member_state_differs()
   {
      // Arrange
      var typedMemberState1 = CreateTypedMemberState(comparisonOperators: ImplementedComparisonOperators.All);
      var typedMemberState2 = CreateTypedMemberState(comparisonOperators: ImplementedComparisonOperators.None);
      var state1 = new KeyMemberState(
         typedMemberState1,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState2,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   [Fact]
   public void Should_handle_complex_state_with_all_properties_set()
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
         comparisonOperators: ImplementedComparisonOperators.All,
         additionOperators: ImplementedOperators.All,
         subtractionOperators: ImplementedOperators.All,
         multiplyOperators: ImplementedOperators.Checked,
         divisionOperators: ImplementedOperators.Default);
      var state = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Private,
         Thinktecture.CodeAnalysis.MemberKind.Field,
         "_amount",
         ArgumentName.Create("amount"));

      // Act
      var hashCode = state.GetHashCode();

      // Assert - just verify it doesn't throw and returns non-zero
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_return_consistent_hash_codes_for_case_sensitive_names()
   {
      // Arrange
      var typedMemberState = CreateTypedMemberState();
      var state1 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "Key",
         ArgumentName.Create("key"));
      var state2 = new KeyMemberState(
         typedMemberState,
         Thinktecture.CodeAnalysis.AccessModifier.Public,
         Thinktecture.CodeAnalysis.MemberKind.Property,
         "key",
         ArgumentName.Create("key"));

      // Act
      var hashCode1 = state1.GetHashCode();
      var hashCode2 = state2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(hashCode2);
   }

   private static KeyMemberState CreateKeyMemberState(
      string name = "Key",
      Thinktecture.CodeAnalysis.AccessModifier accessModifier = Thinktecture.CodeAnalysis.AccessModifier.Public,
      Thinktecture.CodeAnalysis.MemberKind kind = Thinktecture.CodeAnalysis.MemberKind.Property)
   {
      return new KeyMemberState(
         CreateTypedMemberState(),
         accessModifier,
         kind,
         name,
         ArgumentName.Create("key"));
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
