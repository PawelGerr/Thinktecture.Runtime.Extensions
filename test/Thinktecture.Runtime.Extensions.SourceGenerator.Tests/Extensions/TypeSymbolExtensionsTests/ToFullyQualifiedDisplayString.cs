using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class ToFullyQualifiedDisplayString : CompilationTestBase
{
   [Fact]
   public void Returns_fully_qualified_name_for_simple_class()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyClass");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_simple_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyStruct");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyStruct");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.IMyInterface");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.IMyInterface");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_record()
   {
      var src = @"
namespace Test;

public record MyRecord(string Name);
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyRecord");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyRecord");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_record_struct()
   {
      var src = @"
namespace Test;

public record struct MyRecordStruct(int Value);
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyRecordStruct");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyRecordStruct");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_enum()
   {
      var src = @"
namespace Test;

public enum MyEnum
{
   Value1,
   Value2
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyEnum");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_delegate()
   {
      var src = @"
namespace Test;

public delegate void MyDelegate(int value);
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyDelegate");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyDelegate");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_generic_class_with_one_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass<T>;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass`1");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyClass<T>");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_generic_class_with_multiple_type_parameters()
   {
      var src = @"
namespace Test;

public class MyClass<T1, T2, T3>;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass`3");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyClass<T1, T2, T3>");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_constructed_generic_type()
   {
      var src = @"
namespace Test;

public class MyClass<T>
{
   public MyClass<int> Field;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass`1");
      var constructedType = type.Construct(compilation.GetSpecialType(SpecialType.System_Int32));

      var result = constructedType.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyClass<int>");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_constructed_generic_type_with_nullable_reference_type()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass<T>
{
   public MyClass<string?> Field;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass`1");
      var field = type.GetMembers("Field").OfType<IFieldSymbol>().First();
      var constructedType = field.Type;

      var result = constructedType.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.MyClass<string?>");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_nested_class()
   {
      var src = @"
namespace Test;

public class OuterClass
{
   public class NestedClass
   {
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.OuterClass+NestedClass");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.OuterClass.NestedClass");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_deeply_nested_class()
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
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.Level1+Level2+Level3");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.Level1.Level2.Level3");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_nested_generic_class()
   {
      var src = @"
namespace Test;

public class Outer<TOuter>
{
   public class Inner<TInner>
   {
   }
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.Outer`1+Inner`1");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Test.Outer<TOuter>.Inner<TInner>");
   }

   // Nullable types
   [Fact]
   public void Returns_fully_qualified_name_for_nullable_value_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int? Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("int?");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_nullable_reference_type()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public string? Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("string?");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_non_nullable_reference_type_in_nullable_context()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public string Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("string");
   }

   // Array types
   [Fact]
   public void Returns_fully_qualified_name_for_single_dimensional_array()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int[] Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("int[]");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_multi_dimensional_array()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int[,] Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("int[,]");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_jagged_array()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int[][] Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("int[][]");
   }

   // Edge cases
   [Fact]
   public void Returns_fully_qualified_name_for_type_in_global_namespace()
   {
      var src = @"
public class GlobalClass;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "GlobalClass");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::GlobalClass");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_type_with_multiple_namespace_levels()
   {
      var src = @"
namespace Level1.Level2.Level3;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Level1.Level2.Level3.MyClass");

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::Level1.Level2.Level3.MyClass");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_error_type()
   {
      var src = @"
namespace Test;

public class MyClass : NonExistentBase;
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["The type or namespace name 'NonExistentBase' could not be found (are you missing a using directive or an assembly reference?)"]);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var baseType = myClassType.BaseType; // This will be an error type

      var result = baseType!.ToFullyQualifiedDisplayString();

      result.Should().Be("NonExistentBase");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_pointer_type()
   {
      var src = @"
namespace Test;

public unsafe class MyClass
{
   public int* Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("int*");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_built_in_types()
   {
      var compilation = CreateCompilation("");

      var intType = compilation.GetSpecialType(SpecialType.System_Int32);
      intType.ToFullyQualifiedDisplayString().Should().Be("int");

      var stringType = compilation.GetSpecialType(SpecialType.System_String);
      stringType.ToFullyQualifiedDisplayString().Should().Be("string");

      var objectType = compilation.GetSpecialType(SpecialType.System_Object);
      objectType.ToFullyQualifiedDisplayString().Should().Be("object");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass<T>
{
   public T Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass`1");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("T");
   }

   // Complex scenarios
   [Fact]
   public void Returns_fully_qualified_name_for_complex_generic_type_with_nested_generics()
   {
      var src = @"
namespace Test;

public class Outer<T>
{
   public System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<T>> Field;
}
";
      var compilation = CreateCompilation(src);
      var outerType = GetTypeSymbol(compilation, "Test.Outer`1");
      var field = outerType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::System.Collections.Generic.Dictionary<string, global::System.Collections.Generic.List<T>>");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_tuple_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public (int, string) Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::System.ValueTuple<int, string>");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_named_tuple_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public (int Id, string Name) Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("global::System.ValueTuple<int, string>");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_function_pointer_type()
   {
      var src = @"
namespace Test;

public unsafe class MyClass
{
   public delegate*<int, string, void> Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Contain("delegate*");
   }

   [Fact]
   public void Returns_fully_qualified_name_for_dynamic_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public dynamic Field;
}
";
      var compilation = CreateCompilation(src);
      var myClassType = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClassType.GetMembers("Field").OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.ToFullyQualifiedDisplayString();

      result.Should().Be("dynamic");
   }

   private CSharpCompilation CreateCompilation(string source, string assemblyName = "TestAssembly", params MetadataReference[] additionalReferences)
   {
      return CreateCompilation(source, assemblyName, allowUnsafe: true, additionalReferences: additionalReferences);
   }
}
