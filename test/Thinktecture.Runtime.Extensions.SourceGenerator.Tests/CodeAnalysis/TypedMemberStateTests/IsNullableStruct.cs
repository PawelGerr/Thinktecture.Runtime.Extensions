using System.Linq;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.CodeAnalysis.TypedMemberStateTests;

public class IsNullableStruct : CompilationTestBase
{
   [Fact]
   public void NonNullable_builtin_value_type_should_not_be_nullable_struct()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public int P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.SpecialType.Should().Be(SpecialType.System_Int32);
      state.IsNullableStruct.Should().BeFalse("int without '?' is not a nullable struct");
   }

   [Fact]
   public void Nullable_builtin_value_type_should_be_nullable_struct()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public int? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      // int? is represented as INamedTypeSymbol constructed from System.Nullable<T>
      state.IsNullableStruct.Should().BeTrue("int? is Nullable<int>");
   }

   [Fact]
   public void NonNullable_custom_struct_should_not_be_nullable_struct()
   {
      // Arrange
      var source = """
         namespace N
         {
            public struct S { }
            public class C
            {
               public S P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.IsValueType.Should().BeTrue();
      p.Type.NullableAnnotation.Should().NotBe(NullableAnnotation.Annotated, "non-nullable struct type should not be annotated");
      state.IsNullableStruct.Should().BeFalse("custom struct without '?' is not a nullable struct");
   }

   [Fact]
   public void Nullable_custom_struct_should_be_nullable_struct()
   {
      // Arrange
      var source = """
         namespace N
         {
            public struct S { }
            public class C
            {
               public S? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      state.IsNullableStruct.Should().BeTrue("S? is Nullable<S>");
   }

   [Fact]
   public void Type_parameter_with_struct_constraint_without_question_mark_should_not_be_nullable_struct()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class G<T> where T : struct
            {
               public T P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var g = GetTypeSymbol(compilation, "N.G`1");
      var p = g.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.TypeKind.Should().Be(TypeKind.TypeParameter);
      state.IsNullableStruct.Should().BeFalse("T (where T : struct) without '?' is not nullable");
   }

   [Fact]
   public void Type_parameter_with_struct_constraint_with_question_mark_should_be_nullable_struct()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class G<T> where T : struct
            {
               public T? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var g = GetTypeSymbol(compilation, "N.G`1");
      var p = g.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      // T? where T : struct is represented as Nullable<T>
      state.IsNullableStruct.Should().BeTrue("T? (where T : struct) is Nullable<T>");
   }

   [Fact]
   public void Reference_type_with_question_mark_should_not_be_nullable_struct()
   {
      // Arrange
      var source = """
         namespace N
         {
            public class C
            {
               public string? P { get; }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var c = GetTypeSymbol(compilation, "N.C");
      var p = c.GetMembers("P").OfType<IPropertySymbol>().Single();

      // Act
      var state = new TypedMemberState(p.Type);

      // Assert
      p.Type.IsReferenceType.Should().BeTrue();
      p.Type.NullableAnnotation.Should().Be(NullableAnnotation.Annotated);
      state.IsNullableStruct.Should().BeFalse("reference types with '?' are not nullable structs");
   }
}
