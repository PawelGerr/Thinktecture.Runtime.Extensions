using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeParameterSymbolExtensionsTests;

public class GetGenericTypeParametersTests : CompilationTestBase
{
   [Fact]
   public void Should_return_empty_list_for_default_immutable_array()
   {
      // Arrange
      var generics = default(ImmutableArray<ITypeParameterSymbol>);

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_empty_list_for_empty_immutable_array()
   {
      // Arrange
      var generics = ImmutableArray<ITypeParameterSymbol>.Empty;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_single_unconstrained_type_parameter()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class GenericClass<T>
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_type_parameter_with_class_constraint()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class GenericClass<T> where T : class
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("class");
   }

   [Fact]
   public void Should_return_type_parameter_with_struct_constraint()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class GenericClass<T> where T : struct
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("struct");
   }

   [Fact]
   public void Should_return_type_parameter_with_notnull_constraint()
   {
      // Arrange
      var source = @"
#nullable enable
namespace Test
{
   public class GenericClass<T> where T : notnull
   {
   }
}";
      var compilation = CreateCompilation(source, nullableContextOptions: NullableContextOptions.Enable);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("notnull");
   }

   [Fact]
   public void Should_return_type_parameter_with_new_constraint()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class GenericClass<T> where T : new()
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("new()");
   }

   [Fact]
   public void Should_return_type_parameter_with_unmanaged_constraint()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class GenericClass<T> where T : unmanaged
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      // Note: 'unmanaged' constraint implies 'struct', so Roslyn reports both
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(2);
      result[0].Constraints[0].Should().Be("struct");
      result[0].Constraints[1].Should().Be("unmanaged");
   }

   [Fact]
   public void Should_return_type_parameter_with_base_type_constraint()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class BaseClass { }

   public class GenericClass<T> where T : BaseClass
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("global::Test.BaseClass");
   }

   [Fact]
   public void Should_return_type_parameter_with_interface_constraint()
   {
      // Arrange
      var source = @"
namespace Test
{
   public interface IMyInterface { }

   public class GenericClass<T> where T : IMyInterface
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("global::Test.IMyInterface");
   }

   [Fact]
   public void Should_return_type_parameter_with_multiple_interface_constraints()
   {
      // Arrange
      var source = @"
namespace Test
{
   public interface IFirst { }
   public interface ISecond { }

   public class GenericClass<T> where T : IFirst, ISecond
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(2);
      result[0].Constraints[0].Should().Be("global::Test.IFirst");
      result[0].Constraints[1].Should().Be("global::Test.ISecond");
   }

   [Fact]
   public void Should_return_type_parameter_with_class_interface_and_new_constraints()
   {
      // Arrange
      var source = @"
namespace Test
{
   public interface IMyInterface { }

   public class GenericClass<T> where T : class, IMyInterface, new()
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(3);
      result[0].Constraints[0].Should().Be("class");
      result[0].Constraints[1].Should().Be("global::Test.IMyInterface");
      result[0].Constraints[2].Should().Be("new()");
   }

   [Fact]
   public void Should_return_type_parameter_with_base_class_interface_and_new_constraints()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class BaseClass { }
   public interface IMyInterface { }

   public class GenericClass<T> where T : BaseClass, IMyInterface, new()
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(3);
      result[0].Constraints[0].Should().Be("global::Test.BaseClass");
      result[0].Constraints[1].Should().Be("global::Test.IMyInterface");
      result[0].Constraints[2].Should().Be("new()");
   }

   [Fact]
   public void Should_return_multiple_unconstrained_type_parameters()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class GenericClass<T1, T2, T3>
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`3");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(3);
      result[0].Name.Should().Be("T1");
      result[0].Constraints.Should().BeEmpty();
      result[1].Name.Should().Be("T2");
      result[1].Constraints.Should().BeEmpty();
      result[2].Name.Should().Be("T3");
      result[2].Constraints.Should().BeEmpty();
   }

   [Fact]
   public void Should_return_multiple_type_parameters_with_different_constraints()
   {
      // Arrange
      var source = @"
namespace Test
{
   public interface IMyInterface { }

   public class GenericClass<T1, T2, T3>
      where T1 : class
      where T2 : struct
      where T3 : IMyInterface, new()
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`3");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(3);

      result[0].Name.Should().Be("T1");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("class");

      result[1].Name.Should().Be("T2");
      result[1].Constraints.Should().HaveCount(1);
      result[1].Constraints[0].Should().Be("struct");

      result[2].Name.Should().Be("T3");
      result[2].Constraints.Should().HaveCount(2);
      result[2].Constraints[0].Should().Be("global::Test.IMyInterface");
      result[2].Constraints[1].Should().Be("new()");
   }

   [Fact]
   public void Should_return_type_parameters_with_cross_reference_constraint()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class GenericClass<T1, T2> where T2 : T1
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`2");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(2);

      result[0].Name.Should().Be("T1");
      result[0].Constraints.Should().BeEmpty();

      result[1].Name.Should().Be("T2");
      result[1].Constraints.Should().HaveCount(1);
      result[1].Constraints[0].Should().Be("T1");
   }

   [Fact]
   public void Should_return_type_parameters_with_complex_cross_references()
   {
      // Arrange
      var source = @"
using System;
namespace Test
{
   public class GenericClass<T1, T2, T3>
      where T1 : IComparable<T1>
      where T2 : T1, IComparable<T2>
      where T3 : T2
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`3");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(3);

      result[0].Name.Should().Be("T1");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("global::System.IComparable<T1>");

      result[1].Name.Should().Be("T2");
      result[1].Constraints.Should().HaveCount(2);
      result[1].Constraints[0].Should().Be("T1");
      result[1].Constraints[1].Should().Be("global::System.IComparable<T2>");

      result[2].Name.Should().Be("T3");
      result[2].Constraints.Should().HaveCount(1);
      result[2].Constraints[0].Should().Be("T2");
   }

   [Fact]
   public void Should_return_type_parameter_with_nested_generic_constraint()
   {
      // Arrange
      var source = @"
using System.Collections.Generic;
namespace Test
{
   public class GenericClass<T> where T : IEnumerable<int>
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("global::System.Collections.Generic.IEnumerable<int>");
   }

   [Fact]
   public void Should_handle_type_parameters_with_notnull_and_interface_constraints()
   {
      // Arrange
      var source = @"
#nullable enable
namespace Test
{
   public interface IMyInterface { }

   public class GenericClass<T> where T : notnull, IMyInterface
   {
   }
}";
      var compilation = CreateCompilation(source, nullableContextOptions: NullableContextOptions.Enable);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(2);
      result[0].Constraints[0].Should().Be("global::Test.IMyInterface");
      result[0].Constraints[1].Should().Be("notnull");
   }

   [Fact]
   public void Should_return_type_parameters_from_generic_method()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class MyClass
   {
      public void GenericMethod<T1, T2>() where T1 : class where T2 : struct
      {
      }
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyClass");
      var methodSymbol = typeSymbol.GetMembers("GenericMethod").OfType<IMethodSymbol>().First();
      var generics = methodSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(2);

      result[0].Name.Should().Be("T1");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("class");

      result[1].Name.Should().Be("T2");
      result[1].Constraints.Should().HaveCount(1);
      result[1].Constraints[0].Should().Be("struct");
   }

   [Fact]
   public void Should_preserve_constraint_order()
   {
      // Arrange
      var source = @"
namespace Test
{
   public class BaseClass { }
   public interface IFirst { }
   public interface ISecond { }
   public interface IThird { }

   public class GenericClass<T> where T : BaseClass, IFirst, ISecond, IThird, new()
   {
   }
}";
      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.GenericClass`1");
      var generics = typeSymbol.TypeParameters;

      // Act
      var result = generics.GetGenericTypeParameters();

      // Assert
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(5);
      result[0].Constraints[0].Should().Be("global::Test.BaseClass");
      result[0].Constraints[1].Should().Be("global::Test.IFirst");
      result[0].Constraints[2].Should().Be("global::Test.ISecond");
      result[0].Constraints[3].Should().Be("global::Test.IThird");
      result[0].Constraints[4].Should().Be("new()");
   }
}
