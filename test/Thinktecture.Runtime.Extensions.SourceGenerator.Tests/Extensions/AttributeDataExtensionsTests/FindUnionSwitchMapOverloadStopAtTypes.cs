using System.Linq;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class FindUnionSwitchMapOverloadStopAtTypes : CompilationTestBase
{
   [Fact]
   public void Should_return_empty_list_when_parameter_not_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         public class BaseUnion { }
         public class DerivedA : BaseUnion { }
         public class DerivedB : BaseUnion { }

         [UnionSwitchMapOverload()]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().BeEmpty();
   }

   [Fact]
   public void Should_extract_single_type()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         public class BaseUnion { }
         public class DerivedA : BaseUnion { }
         public class DerivedB : BaseUnion { }

         [UnionSwitchMapOverload(StopAt = new[] { typeof(DerivedA) })]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().ContainSingle();
      result[0].Should().Be("global::Test.DerivedA");
   }

   [Fact]
   public void Should_extract_multiple_types()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         public class BaseUnion { }
         public class DerivedA : BaseUnion { }
         public class DerivedB : BaseUnion { }
         public class DerivedC : BaseUnion { }

         [UnionSwitchMapOverload(StopAt = new[] { typeof(DerivedA), typeof(DerivedC) })]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain("global::Test.DerivedA");
      result.Should().Contain("global::Test.DerivedC");
   }

   [Fact]
   public void Should_return_empty_list_when_StopAt_is_empty_array()
   {
      // Arrange
      var source = """
         using Thinktecture;
         using System;

         namespace Test;

         public class BaseUnion { }
         public class DerivedA : BaseUnion { }

         [UnionSwitchMapOverload(StopAt = new Type[0])]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().BeEmpty();
   }

   [Fact]
   public void Should_skip_error_types()
   {
      // Arrange - UnknownType will be an error type
      var source = """
         using Thinktecture;

         namespace Test;

         public class BaseUnion { }
         public class DerivedA : BaseUnion { }

         [UnionSwitchMapOverload(StopAt = new[] { typeof(DerivedA), typeof(UnknownType) })]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source, expectedCompilerErrors: ["The type or namespace name 'UnknownType' could not be found (are you missing a using directive or an assembly reference?)"]);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().ContainSingle();
      result[0].Should().Be("global::Test.DerivedA");
   }

   [Fact]
   public void Should_handle_types_from_different_namespaces()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test.Namespace1
         {
            public class Type1 { }
         }

         namespace Test.Namespace2
         {
            public class Type2 { }
         }

         namespace Test
         {
            public class BaseUnion { }

            [UnionSwitchMapOverload(StopAt = new[] { typeof(Namespace1.Type1), typeof(Namespace2.Type2) })]
            public partial class MyUnion : BaseUnion
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain("global::Test.Namespace1.Type1");
      result.Should().Contain("global::Test.Namespace2.Type2");
   }

   [Fact]
   public void Should_handle_generic_types()
   {
      // Arrange
      var source = """
         using Thinktecture;
         using System.Collections.Generic;

         namespace Test;

         public class BaseUnion { }

         [UnionSwitchMapOverload(StopAt = new[] { typeof(List<int>), typeof(Dictionary<string, int>) })]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain("global::System.Collections.Generic.List<int>");
      result.Should().Contain("global::System.Collections.Generic.Dictionary<string, int>");
   }

   [Fact]
   public void Should_handle_nested_types()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         public class OuterClass
         {
            public class InnerClass
            {
               public class DeepClass { }
            }
         }

         public class BaseUnion { }

         [UnionSwitchMapOverload(StopAt = new[] { typeof(OuterClass.InnerClass.DeepClass) })]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().ContainSingle();
      result[0].Should().Be("global::Test.OuterClass.InnerClass.DeepClass");
   }

   [Fact]
   public void Should_handle_nullable_value_types()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         public class BaseUnion { }

         [UnionSwitchMapOverload(StopAt = new[] { typeof(int?), typeof(double?) })]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain("int?");
      result.Should().Contain("double?");
   }

   [Fact]
   public void Should_handle_array_types()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         public class BaseUnion { }

         [UnionSwitchMapOverload(StopAt = new[] { typeof(int[]), typeof(string[]) })]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().HaveCount(2);
      result.Should().Contain("int[]");
      result.Should().Contain("string[]");
   }

   [Fact]
   public void Should_preserve_order_of_types()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         public class TypeA { }
         public class TypeB { }
         public class TypeC { }
         public class BaseUnion { }

         [UnionSwitchMapOverload(StopAt = new[] { typeof(TypeC), typeof(TypeA), typeof(TypeB) })]
         public partial class MyUnion : BaseUnion
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.MyUnion");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindUnionSwitchMapOverloadStopAtTypes();

      // Assert
      result.Should().HaveCount(3);
      result[0].Should().Be("global::Test.TypeC");
      result[1].Should().Be("global::Test.TypeA");
      result[2].Should().Be("global::Test.TypeB");
   }
}
