using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.NamedTypeSymbolExtensionsTests;

public class IsNestedInGenericClass
{
   [Fact]
   public void Returns_false_for_top_level_non_generic_class()
   {
      var src = @"
namespace Test;

public class C;
";
      var type = GetTypeSymbol(src, "Test.C");

      var result = type.IsNestedInGenericClass();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_top_level_generic_class()
   {
      var src = @"
namespace Test;

public class C<T>;
";
      var type = GetTypeSymbol(src, "Test.C`1");

      var result = type.IsNestedInGenericClass();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_class_nested_in_non_generic_class()
   {
      var src = @"
namespace Test;

public class Outer
{
   public class Inner
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outer+Inner");

      var result = type.IsNestedInGenericClass();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_class_nested_in_generic_class()
   {
      var src = @"
namespace Test;

public class Outer<T>
{
   public class Inner
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outer`1+Inner");

      var result = type.IsNestedInGenericClass();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_class_nested_in_generic_interface()
   {
      var src = @"
namespace Test;

public interface IOuter<T>
{
   public class Inner
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.IOuter`1+Inner");

      var result = type.IsNestedInGenericClass();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_top_level_class_with_null_containing_type()
   {
      var src = @"
namespace Test;

public class C;
";
      var type = GetTypeSymbol(src, "Test.C");

      // Verify ContainingType is indeed null
      type.ContainingType.Should().BeNull();

      var result = type.IsNestedInGenericClass();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_class_nested_in_generic_class()
   {
      var src = @"
namespace Test;

public class Outer<T>
{
   public class Inner<U>
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outer`1+Inner`1");

      var result = type.IsNestedInGenericClass();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_deeply_nested_class_with_generic_ancestor()
   {
      var src = @"
namespace Test;

public class Level1<T>
{
   public class Level2
   {
      public class Level3
      {
         public class Level4
         {
         }
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.Level1`1+Level2+Level3+Level4");

      var result = type.IsNestedInGenericClass();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_struct_with_no_type_parameters_at_top_level()
   {
      var src = @"
namespace Test;

public struct S;
";
      var type = GetTypeSymbol(src, "Test.S");

      var result = type.IsNestedInGenericClass();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_when_generic_is_at_middle_level_in_chain()
   {
      var src = @"
namespace Test;

public class Level1
{
   public class Level2<T>
   {
      public class Level3
      {
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.Level1+Level2`1+Level3");

      var result = type.IsNestedInGenericClass();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_multiple_containing_types_have_type_parameters()
   {
      var src = @"
namespace Test;

public class Outer<T>
{
   public class Middle<U>
   {
      public class Inner
      {
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outer`1+Middle`1+Inner");

      var result = type.IsNestedInGenericClass();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_deeply_nested_non_generic_chain()
   {
      var src = @"
namespace Test;

public class Level1
{
   public class Level2
   {
      public class Level3
      {
         public class Level4
         {
            public class Level5
            {
            }
         }
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.Level1+Level2+Level3+Level4+Level5");

      var result = type.IsNestedInGenericClass();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_class_nested_in_generic_struct()
   {
      var src = @"
namespace Test;

public struct S<T>
{
   public class Inner
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.S`1+Inner");

      var result = type.IsNestedInGenericClass();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_class_nested_in_non_generic_interface()
   {
      var src = @"
namespace Test;

public interface IOuter
{
   public class Inner
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.IOuter+Inner");

      var result = type.IsNestedInGenericClass();

      result.Should().BeFalse();
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName)
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "IsNestedInGenericClassTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);
      if (type is null)
         throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");

      return type;
   }
}
