using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.NamedTypeSymbolExtensionsTests;

public class GetValidateFactoryArgumentsReturnType : CompilationTestBase
{
   [Fact]
   public void Should_return_null_when_no_ValidateFactoryArguments_method_exists()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static void SomeOtherMethod() { }
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_ValidateFactoryArguments_returns_void()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static void ValidateFactoryArguments() { }
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_fully_qualified_type_name_when_ValidateFactoryArguments_returns_ValidationError()
   {
      var src = @"
using Thinktecture;

namespace Test;

public class MyClass
{
   public static ValidationError? ValidateFactoryArguments() => null;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Thinktecture.ValidationError?");
   }

   [Fact]
   public void Should_return_fully_qualified_type_name_when_ValidateFactoryArguments_returns_custom_type()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   public static CustomError ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_return_fully_qualified_type_name_when_ValidateFactoryArguments_returns_nullable_custom_type()
   {
      var src = @"
#nullable enable

namespace Test;

public class CustomError
{
}

public class MyClass
{
   public static CustomError? ValidateFactoryArguments() => null;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError?");
   }

   [Fact]
   public void Should_return_null_when_ValidateFactoryArguments_is_not_static()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   public CustomError ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_ValidateFactoryArguments_is_generic()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   public static CustomError ValidateFactoryArguments<T>() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_first_non_void_ValidateFactoryArguments_when_multiple_overloads()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   public static void ValidateFactoryArguments(int x) { }
   public static CustomError ValidateFactoryArguments(string s) => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_return_fully_qualified_type_name_for_generic_return_type()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class MyClass
{
   public static List<string> ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::System.Collections.Generic.List<string>");
   }

   [Fact]
   public void Should_return_fully_qualified_type_name_for_nested_generic_return_type()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class MyClass
{
   public static Dictionary<string, List<int>> ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<int>>");
   }

   [Fact]
   public void Should_return_fully_qualified_type_name_for_tuple_return_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static (string, int) ValidateFactoryArguments() => default;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::System.ValueTuple<string, int>");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_in_struct()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public struct MyStruct
{
   public static CustomError ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyStruct");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_in_record()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public record MyRecord
{
   public static CustomError ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyRecord");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_in_nested_class()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class Outer
{
   public class Inner
   {
      public static CustomError ValidateFactoryArguments() => null!;
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outer+Inner");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_return_null_when_no_members_exist()
   {
      var src = @"
namespace Test;

public class MyClass
{
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_only_non_method_members_exist()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static int Field;
   public static string Property { get; set; }
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_with_parameters()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   public static CustomError ValidateFactoryArguments(int value, string name) => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_return_fully_qualified_type_name_for_array_return_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static string[] ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("string[]");
   }

   [Fact]
   public void Should_return_fully_qualified_type_name_for_multidimensional_array()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static string[,] ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("string[,]");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_with_private_accessibility()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   private static CustomError ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_with_protected_accessibility()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   protected static CustomError ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_with_internal_accessibility()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   internal static CustomError ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_returning_value_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static int ValidateFactoryArguments() => 0;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("int");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_returning_nullable_value_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static int? ValidateFactoryArguments() => null;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("int?");
   }

   [Fact]
   public void Should_ignore_methods_with_similar_names()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   public static CustomError ValidateFactoryArgumentsInternal() => null!;
   public static CustomError MyValidateFactoryArguments() => null!;
   public static void ValidateFactoryArguments_WithSuffix() { }
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_with_ref_parameters()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   public static CustomError ValidateFactoryArguments(ref int value) => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_with_out_parameters()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   public static CustomError ValidateFactoryArguments(out int value)
   {
      value = 0;
      return null!;
   }
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_with_in_parameters()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass
{
   public static CustomError ValidateFactoryArguments(in int value) => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_handle_ValidateFactoryArguments_in_generic_class()
   {
      var src = @"
namespace Test;

public class CustomError
{
}

public class MyClass<T>
{
   public static CustomError ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.MyClass`1");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.CustomError");
   }

   [Fact]
   public void Should_return_fully_qualified_type_name_with_namespace()
   {
      var src = @"
namespace Test.SubNamespace;

public class CustomError
{
}

public class MyClass
{
   public static CustomError ValidateFactoryArguments() => null!;
}
";
      var type = GetTypeSymbol(src, "Test.SubNamespace.MyClass");

      var result = type.GetValidateFactoryArgumentsReturnType();

      result.Should().NotBeNull();
      result.Should().Be("global::Test.SubNamespace.CustomError");
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName)
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "GetValidateFactoryArgumentsReturnTypeTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);
      if (type is null)
         throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");

      return type;
   }
}
