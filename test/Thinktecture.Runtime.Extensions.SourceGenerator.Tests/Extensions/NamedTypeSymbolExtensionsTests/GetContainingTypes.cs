using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.NamedTypeSymbolExtensionsTests;

public class GetContainingTypes
{
   [Fact]
   public void Returns_empty_list_for_top_level_class()
   {
      var src = @"
namespace Test;

public class TopLevel;
";
      var type = GetTypeSymbol(src, "Test.TopLevel");

      var result = type.GetContainingTypes();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Returns_single_containing_type_for_nested_class()
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

      var result = type.GetContainingTypes();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Outer");
      result[0].IsReferenceType.Should().BeTrue();
      result[0].IsRecord.Should().BeFalse();
      result[0].GenericParameters.Should().BeEmpty();
   }

   [Fact]
   public void Returns_multiple_containing_types_for_deeply_nested_class()
   {
      var src = @"
namespace Test;

public class Level1
{
   public class Level2
   {
      public class Level3
      {
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.Level1+Level2+Level3");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(2);
      result[0].Name.Should().Be("Level1");
      result[0].IsReferenceType.Should().BeTrue();
      result[1].Name.Should().Be("Level2");
      result[1].IsReferenceType.Should().BeTrue();
   }

   [Fact]
   public void Returns_containing_struct_for_class_nested_in_struct()
   {
      var src = @"
namespace Test;

public struct OuterStruct
{
   public class InnerClass
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.OuterStruct+InnerClass");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("OuterStruct");
      result[0].IsReferenceType.Should().BeFalse();
      result[0].IsRecord.Should().BeFalse();
   }

   [Fact]
   public void Returns_containing_record_for_class_nested_in_record()
   {
      var src = @"
namespace Test;

public record OuterRecord
{
   public class InnerClass
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.OuterRecord+InnerClass");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("OuterRecord");
      result[0].IsReferenceType.Should().BeTrue();
      result[0].IsRecord.Should().BeTrue();
   }

   [Fact]
   public void Returns_empty_list_for_top_level_struct()
   {
      var src = @"
namespace Test;

public struct TopLevelStruct;
";
      var type = GetTypeSymbol(src, "Test.TopLevelStruct");

      var result = type.GetContainingTypes();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Returns_all_containing_types_for_deeply_nested_class_five_levels()
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

      var result = type.GetContainingTypes();

      result.Should().HaveCount(4);
      result[0].Name.Should().Be("Level1");
      result[1].Name.Should().Be("Level2");
      result[2].Name.Should().Be("Level3");
      result[3].Name.Should().Be("Level4");
   }

   [Fact]
   public void Returns_containing_generic_class_with_type_parameters()
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

      var result = type.GetContainingTypes();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Outer");
      result[0].GenericParameters.Should().HaveCount(1);
      result[0].GenericParameters[0].Name.Should().Be("T");
   }

   [Fact]
   public void Returns_containing_generic_class_with_constraints()
   {
      var src = @"
namespace Test;

public class Outer<T> where T : class
{
   public class Inner
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outer`1+Inner");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Outer");
      result[0].GenericParameters.Should().HaveCount(1);
      result[0].GenericParameters[0].Name.Should().Be("T");
      result[0].GenericParameters[0].Constraints.Should().Contain("class");
   }

   [Fact]
   public void Returns_containing_generic_struct()
   {
      var src = @"
namespace Test;

public struct OuterStruct<T>
{
   public class InnerClass
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.OuterStruct`1+InnerClass");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("OuterStruct");
      result[0].IsReferenceType.Should().BeFalse();
      result[0].GenericParameters.Should().HaveCount(1);
      result[0].GenericParameters[0].Name.Should().Be("T");
   }

   [Fact]
   public void Returns_empty_list_for_top_level_record()
   {
      var src = @"
namespace Test;

public record TopLevelRecord;
";
      var type = GetTypeSymbol(src, "Test.TopLevelRecord");

      var result = type.GetContainingTypes();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Returns_containing_types_in_correct_order_outermost_to_innermost()
   {
      var src = @"
namespace Test;

public class Outermost
{
   public class Middle
   {
      public class Innermost
      {
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outermost+Middle+Innermost");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(2);
      // Verify order: outermost first due to Reverse() call in implementation
      result[0].Name.Should().Be("Outermost");
      result[1].Name.Should().Be("Middle");
   }

   [Fact]
   public void Returns_mixed_containing_types_class_struct_record()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   public struct MiddleStruct
   {
      public record InnerRecord
      {
         public class DeepestClass
         {
         }
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.OuterClass+MiddleStruct+InnerRecord+DeepestClass");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(3);
      result[0].Name.Should().Be("OuterClass");
      result[0].IsReferenceType.Should().BeTrue();
      result[0].IsRecord.Should().BeFalse();

      result[1].Name.Should().Be("MiddleStruct");
      result[1].IsReferenceType.Should().BeFalse();
      result[1].IsRecord.Should().BeFalse();

      result[2].Name.Should().Be("InnerRecord");
      result[2].IsReferenceType.Should().BeTrue();
      result[2].IsRecord.Should().BeTrue();
   }

   [Fact]
   public void Returns_multiple_generic_containing_types_at_different_levels()
   {
      var src = @"
namespace Test;

public class Outer<T1, T2> where T1 : class where T2 : struct
{
   public class Middle<T3>
   {
      public class Inner
      {
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.Outer`2+Middle`1+Inner");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(2);

      result[0].Name.Should().Be("Outer");
      result[0].GenericParameters.Should().HaveCount(2);
      result[0].GenericParameters[0].Name.Should().Be("T1");
      result[0].GenericParameters[0].Constraints.Should().Contain("class");
      result[0].GenericParameters[1].Name.Should().Be("T2");
      result[0].GenericParameters[1].Constraints.Should().Contain("struct");

      result[1].Name.Should().Be("Middle");
      result[1].GenericParameters.Should().HaveCount(1);
      result[1].GenericParameters[0].Name.Should().Be("T3");
   }

   [Fact]
   public void Returns_containing_types_preserving_all_properties()
   {
      var src = @"
using System;

namespace Test;

public record OuterRecord<T> where T : IDisposable
{
   public struct MiddleStruct
   {
      public class InnerClass
      {
      }
   }
}
";
      var type = GetTypeSymbol(src, "Test.OuterRecord`1+MiddleStruct+InnerClass");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(2);

      // First level: OuterRecord
      result[0].Name.Should().Be("OuterRecord");
      result[0].IsReferenceType.Should().BeTrue();
      result[0].IsRecord.Should().BeTrue();
      result[0].GenericParameters.Should().HaveCount(1);
      result[0].GenericParameters[0].Name.Should().Be("T");
      result[0].GenericParameters[0].Constraints.Should().Contain("global::System.IDisposable");

      // Second level: MiddleStruct
      result[1].Name.Should().Be("MiddleStruct");
      result[1].IsReferenceType.Should().BeFalse();
      result[1].IsRecord.Should().BeFalse();
      result[1].GenericParameters.Should().BeEmpty();
   }

   [Fact]
   public void Returns_correct_order_for_single_level_nesting()
   {
      var src = @"
namespace Test;

public class Single
{
   public class Nested
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.Single+Nested");

      var result = type.GetContainingTypes();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("Single");
   }

   [Fact]
   public void Returns_containing_interface_for_class_nested_in_interface()
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

      var result = type.GetContainingTypes();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("IOuter");
      result[0].IsReferenceType.Should().BeTrue();
      result[0].IsRecord.Should().BeFalse();
      result[0].GenericParameters.Should().HaveCount(1);
      result[0].GenericParameters[0].Name.Should().Be("T");
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName)
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "GetContainingTypesTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);

      return type ?? throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");
   }
}
