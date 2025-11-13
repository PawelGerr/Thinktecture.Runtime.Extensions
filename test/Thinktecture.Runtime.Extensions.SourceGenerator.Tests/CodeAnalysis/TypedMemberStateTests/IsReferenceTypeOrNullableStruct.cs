using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.TypedMemberStateTests;

public class IsReferenceTypeOrNullableStruct : CompilationTestBase
{
   [Fact]
   public void TypeParameter_with_struct_constraint_should_not_be_treated_as_reference_or_nullable_struct()
   {
      // Arrange
      var source = """
         namespace TestNamespace
         {
            public class C<T>
               where T : struct
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var cSymbol = GetTypeSymbol(compilation, "TestNamespace.C`1");
      var tParam = cSymbol.TypeParameters[0]; // ITypeParameterSymbol for T

      // Sanity checks about Roslyn's view on type parameters
      tParam.TypeKind.Should().Be(TypeKind.TypeParameter);
      tParam.HasValueTypeConstraint.Should().BeTrue("T : struct");
      tParam.HasReferenceTypeConstraint.Should().BeFalse();

      // Act
      var state = new TypedMemberState(tParam);

      // Assert
      state.IsTypeParameter.Should().BeTrue("T is a type parameter");
      state.IsReferenceType.Should().BeFalse("type parameters are not marked as reference types");
      state.IsValueType.Should().BeTrue("Roslyn marks type parameters constrained to struct as value types");
      state.IsNullableStruct.Should().BeFalse("T : struct is not a nullable struct");

      // Under test
      state.IsReferenceTypeOrNullableStruct.Should().BeFalse("T : struct must not be treated as reference type or nullable struct");
   }

   [Fact]
   public void TypeParameter_with_class_constraint_should_be_treated_as_reference_or_nullable_struct()
   {
      // Arrange
      var source = """
         namespace TestNamespace
         {
            public class C<T>
               where T : class
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var cSymbol = GetTypeSymbol(compilation, "TestNamespace.C`1");
      var tParam = cSymbol.TypeParameters[0];

      // Sanity checks
      tParam.TypeKind.Should().Be(TypeKind.TypeParameter);
      tParam.HasReferenceTypeConstraint.Should().BeTrue("T : class");
      tParam.HasValueTypeConstraint.Should().BeFalse();

      // Act
      var state = new TypedMemberState(tParam);

      // Assert
      state.IsTypeParameter.Should().BeTrue();
      state.IsValueType.Should().BeFalse();
      state.IsNullableStruct.Should().BeFalse();

      // Expression: IsReferenceType || IsNullableStruct || (IsTypeParameter && !IsValueType)
      state.IsReferenceTypeOrNullableStruct.Should().BeTrue("T : class should be treated like a reference type");
   }

   [Fact]
   public void Unconstrained_TypeParameter_should_be_treated_as_reference_or_nullable_struct_conservatively()
   {
      // Arrange
      var source = """
         namespace TestNamespace
         {
            public class C<T>
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var cSymbol = GetTypeSymbol(compilation, "TestNamespace.C`1");
      var tParam = cSymbol.TypeParameters[0];

      // Sanity checks
      tParam.TypeKind.Should().Be(TypeKind.TypeParameter);
      tParam.HasReferenceTypeConstraint.Should().BeFalse();
      tParam.HasValueTypeConstraint.Should().BeFalse();

      // Act
      var state = new TypedMemberState(tParam);

      // Assert
      state.IsTypeParameter.Should().BeTrue();
      state.IsValueType.Should().BeFalse();
      state.IsNullableStruct.Should().BeFalse();

      // With no constraints, treat as reference-or-nullable-struct to be safe in nullability-sensitive contexts
      state.IsReferenceTypeOrNullableStruct.Should().BeTrue("unconstrained T should be treated as potentially reference type or nullable struct");
   }
}
