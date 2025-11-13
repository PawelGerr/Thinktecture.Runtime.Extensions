using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class GetComparerTypes : CompilationTestBase
{
   [Fact]
   public void Should_extract_types_from_two_generic_args_attribute()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int>]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "UnionAttribute" && a.AttributeClass!.TypeArguments.Length == 2);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().NotBeNull();
      result!.Value.ComparerType.ToDisplayString().Should().Be("string");
      result!.Value.ItemType.ToDisplayString().Should().Be("int");
   }

   [Fact]
   public void Should_handle_different_type_arguments()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<int, double>]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "UnionAttribute" && a.AttributeClass!.TypeArguments.Length == 2);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().NotBeNull();
      result!.Value.ComparerType.ToDisplayString().Should().Be("int");
      result!.Value.ItemType.ToDisplayString().Should().Be("double");
   }

   [Fact]
   public void Should_handle_custom_reference_types()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         public class MyType1 { }
         public class MyType2 { }

         [Union<MyType1, MyType2>]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "UnionAttribute" && a.AttributeClass!.TypeArguments.Length == 2);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().NotBeNull();
      result!.Value.ComparerType.ToDisplayString().Should().Be("Test.MyType1");
      result!.Value.ItemType.ToDisplayString().Should().Be("Test.MyType2");
   }

   [Fact]
   public void Should_return_null_for_non_generic_attribute()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestClass");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "UnionAttribute" && a.AttributeClass!.TypeArguments.Length == 0);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_attribute_has_one_type_parameter()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [SmartEnum<int>]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestEnum");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "SmartEnumAttribute" && a.AttributeClass!.TypeArguments.Length == 1);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_attribute_has_three_type_parameters()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, int, double>]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "UnionAttribute" && a.AttributeClass!.TypeArguments.Length == 3);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_first_type_argument_is_error_type()
   {
      // Arrange - UnknownType will be an error type
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<UnknownType, string>]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source, expectedCompilerErrors: ["The type or namespace name 'UnknownType' could not be found (are you missing a using directive or an assembly reference?)"]);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "UnionAttribute" && a.AttributeClass!.TypeArguments.Length == 2);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_when_second_type_argument_is_error_type()
   {
      // Arrange - UnknownType will be an error type
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<string, UnknownType>]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source, expectedCompilerErrors: ["The type or namespace name 'UnknownType' could not be found (are you missing a using directive or an assembly reference?)"]);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "UnionAttribute" && a.AttributeClass!.TypeArguments.Length == 2);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_handle_nested_generic_types()
   {
      // Arrange
      var source = """
         using System.Collections.Generic;
         using Thinktecture;

         namespace Test;

         [Union<List<string>, List<int>>]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "UnionAttribute" && a.AttributeClass!.TypeArguments.Length == 2);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().NotBeNull();
      result!.Value.ComparerType.ToDisplayString().Should().Be("System.Collections.Generic.List<string>");
      result!.Value.ItemType.ToDisplayString().Should().Be("System.Collections.Generic.List<int>");
   }

   [Fact]
   public void Should_handle_array_types()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [Union<int[], string[]>]
         public partial class TestUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "Test.TestUnion");
      var attributeData = testClassSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "UnionAttribute" && a.AttributeClass!.TypeArguments.Length == 2);

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().NotBeNull();
      result!.Value.ComparerType.ToDisplayString().Should().Be("int[]");
      result!.Value.ItemType.ToDisplayString().Should().Be("string[]");
   }

   [Fact]
   public void Should_work_with_real_KeyMemberEqualityComparerAttribute()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinalIgnoreCase, string>]
         [ValueObject<string>]
         public partial class MyValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyValueObject");
      var attributeData = typeSymbol.GetAttributes().First(a => a.AttributeClass?.Name == "KeyMemberEqualityComparerAttribute");

      // Act
      var result = attributeData.GetComparerTypes();

      // Assert
      result.Should().NotBeNull();
      result!.Value.ComparerType.ToDisplayString().Should().Be("Thinktecture.ComparerAccessors.StringOrdinalIgnoreCase");
      result!.Value.ItemType.ToDisplayString().Should().Be("string");
   }
}
