using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.NamedTypeSymbolExtensionsTests;

public class GetGenericTypeParameters
{
   [Fact]
   public void Returns_empty_list_for_non_generic_class()
   {
      var src = @"
namespace Test;

public class NonGenericClass;
";
      var type = GetTypeSymbol(src, "Test.NonGenericClass");

      var result = type.GetGenericTypeParameters();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Returns_single_parameter_with_no_constraints()
   {
      var src = @"
namespace Test;

public class GenericClass<T>;
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().BeEmpty();
   }

   [Fact]
   public void Returns_multiple_parameters_with_no_constraints()
   {
      var src = @"
namespace Test;

public class GenericClass<T1, T2, T3>;
";
      var type = GetTypeSymbol(src, "Test.GenericClass`3");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(3);
      result[0].Name.Should().Be("T1");
      result[0].Constraints.Should().BeEmpty();
      result[1].Name.Should().Be("T2");
      result[1].Constraints.Should().BeEmpty();
      result[2].Name.Should().Be("T3");
      result[2].Constraints.Should().BeEmpty();
   }

   [Fact]
   public void Returns_parameter_with_class_constraint()
   {
      var src = @"
namespace Test;

public class GenericClass<T> where T : class;
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("class");
   }

   [Fact]
   public void Returns_parameter_with_struct_constraint()
   {
      var src = @"
namespace Test;

public class GenericClass<T> where T : struct;
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("struct");
   }

   [Fact]
   public void Returns_interface_type_parameters_correctly()
   {
      var src = @"
namespace Test;

public interface IGenericInterface<TKey, TValue>;
";
      var type = GetTypeSymbol(src, "Test.IGenericInterface`2");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(2);
      result[0].Name.Should().Be("TKey");
      result[0].Constraints.Should().BeEmpty();
      result[1].Name.Should().Be("TValue");
      result[1].Constraints.Should().BeEmpty();
   }

   [Fact]
   public void Returns_empty_list_for_non_generic_struct()
   {
      var src = @"
namespace Test;

public struct NonGenericStruct;
";
      var type = GetTypeSymbol(src, "Test.NonGenericStruct");

      var result = type.GetGenericTypeParameters();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Returns_record_type_parameters_correctly()
   {
      var src = @"
namespace Test;

public record GenericRecord<T1, T2>(T1 Value1, T2 Value2);
";
      var type = GetTypeSymbol(src, "Test.GenericRecord`2");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(2);
      result[0].Name.Should().Be("T1");
      result[0].Constraints.Should().BeEmpty();
      result[1].Name.Should().Be("T2");
      result[1].Constraints.Should().BeEmpty();
   }

   [Fact]
   public void Returns_nested_generic_class_parameters_only_for_nested_class()
   {
      var src = @"
namespace Test;

public class OuterClass<TOuter>
{
   public class NestedClass<TNested>
   {
   }
}
";
      var type = GetTypeSymbol(src, "Test.OuterClass`1+NestedClass`1");

      var result = type.GetGenericTypeParameters();

      // Should only return the nested class's type parameters, not the outer class's
      result.Should().HaveCount(1);
      result[0].Name.Should().Be("TNested");
      result[0].Constraints.Should().BeEmpty();
   }

   [Fact]
   public void Returns_parameter_with_multiple_interface_constraints()
   {
      var src = @"
using System;
using System.Collections.Generic;

namespace Test;

public class GenericClass<T> where T : IComparable<T>, IEnumerable<T>;
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(2);
      result[0].Constraints[0].Should().Be("global::System.IComparable<T>");
      result[0].Constraints[1].Should().Be("global::System.Collections.Generic.IEnumerable<T>");
   }

   [Fact]
   public void Returns_parameter_with_complex_constraints()
   {
      var src = @"
using System;
using System.Collections.Generic;

namespace Test;

public class GenericClass<T> where T : class, IComparable<T>, new();
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(3);
      result[0].Constraints[0].Should().Be("class");
      result[0].Constraints[1].Should().Be("global::System.IComparable<T>");
      result[0].Constraints[2].Should().Be("new()");
   }

   [Fact]
   public void Returns_covariant_parameter_correctly()
   {
      var src = @"
namespace Test;

public interface ICovariantInterface<out T>;
";
      var type = GetTypeSymbol(src, "Test.ICovariantInterface`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().BeEmpty();
      // Note: Variance is a property of ITypeParameterSymbol but not captured in GenericTypeParameterState
      // This test verifies the method still works correctly with covariant parameters
   }

   [Fact]
   public void Returns_contravariant_parameter_correctly()
   {
      var src = @"
namespace Test;

public interface IContravariantInterface<in T>;
";
      var type = GetTypeSymbol(src, "Test.IContravariantInterface`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().BeEmpty();
      // Note: Variance is a property of ITypeParameterSymbol but not captured in GenericTypeParameterState
      // This test verifies the method still works correctly with contravariant parameters
   }

   [Fact]
   public void Returns_multiple_parameters_with_different_constraints()
   {
      var src = @"
using System;

namespace Test;

public class GenericClass<T1, T2, T3>
   where T1 : class
   where T2 : struct
   where T3 : IDisposable, new();
";
      var type = GetTypeSymbol(src, "Test.GenericClass`3");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(3);

      result[0].Name.Should().Be("T1");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("class");

      result[1].Name.Should().Be("T2");
      result[1].Constraints.Should().HaveCount(1);
      result[1].Constraints[0].Should().Be("struct");

      result[2].Name.Should().Be("T3");
      result[2].Constraints.Should().HaveCount(2);
      result[2].Constraints[0].Should().Be("global::System.IDisposable");
      result[2].Constraints[1].Should().Be("new()");
   }

   [Fact]
   public void Returns_parameter_with_concrete_base_class_constraint()
   {
      var src = @"
using System;

namespace Test;

public class BaseClass { }

public class GenericClass<T> where T : BaseClass;
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(1);
      result[0].Constraints[0].Should().Be("global::Test.BaseClass");
   }

   [Fact]
   public void Returns_parameter_with_another_type_parameter_constraint()
   {
      var src = @"
namespace Test;

public class GenericClass<T1, T2> where T2 : T1;
";
      var type = GetTypeSymbol(src, "Test.GenericClass`2");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(2);
      result[0].Name.Should().Be("T1");
      result[0].Constraints.Should().BeEmpty();
      result[1].Name.Should().Be("T2");
      result[1].Constraints.Should().HaveCount(1);
      result[1].Constraints[0].Should().Be("T1");
   }

   [Fact]
   public void Returns_parameter_with_notnull_constraint()
   {
      var src = @"
namespace Test;

public class GenericClass<T> where T : notnull;
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints[0].Should().Be("notnull");
   }

   [Fact]
   public void Returns_parameter_with_unmanaged_constraint()
   {
      var src = @"
namespace Test;

public class GenericClass<T> where T : unmanaged;
";
      var type = GetTypeSymbol(src, "Test.GenericClass`1");

      var result = type.GetGenericTypeParameters();

      result.Should().HaveCount(1);
      result[0].Name.Should().Be("T");
      result[0].Constraints.Should().HaveCount(2);
      result[0].Constraints.Should().Contain("struct");
      result[0].Constraints.Should().Contain("unmanaged");
   }

   private static INamedTypeSymbol GetTypeSymbol(string source, string metadataName)
   {
      var parseOptions = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Preview);
      var syntaxTree = CSharpSyntaxTree.ParseText(source, parseOptions);
      var references = AppDomain.CurrentDomain.GetAssemblies()
                                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
                                .Select(a => MetadataReference.CreateFromFile(a.Location));

      var compilation = CSharpCompilation.Create(
         "GetGenericTypeParametersTests",
         [syntaxTree],
         references,
         new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release));

      var type = compilation.GetTypeByMetadataName(metadataName);
      return type ?? throw new InvalidOperationException($"Type '{metadataName}' not found. Diagnostics: {string.Join(Environment.NewLine, compilation.GetDiagnostics().Select(d => d.ToString()))}");
   }
}
