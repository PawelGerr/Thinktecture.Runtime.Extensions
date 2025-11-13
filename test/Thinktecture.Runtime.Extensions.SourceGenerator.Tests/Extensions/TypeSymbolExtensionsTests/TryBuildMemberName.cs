using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class TryBuildMemberName : CompilationTestBase
{
   [Fact]
   public void Returns_true_and_type_name_for_simple_class()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("MyClass");
   }

   [Fact]
   public void Returns_true_and_type_name_for_simple_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyStruct");

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("MyStruct");
   }

   [Fact]
   public void Returns_true_and_type_name_for_simple_interface()
   {
      var src = @"
namespace Test;

public interface IMyInterface;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.IMyInterface");

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("IMyInterface");
   }

   [Fact]
   public void Returns_true_and_type_name_for_simple_record()
   {
      var src = @"
namespace Test;

public record MyRecord(int Value);
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyRecord");

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("MyRecord");
   }

   // ITypeSymbol overload - Generic types
   [Fact]
   public void Returns_true_and_concatenated_name_for_generic_type_with_single_type_argument()
   {
      var src = @"
namespace Test;

public class Container<T>;

public class Usage
{
   public Container<int> Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("ContainerOfInt32");
   }

   [Fact]
   public void Returns_true_and_concatenated_name_for_generic_type_with_multiple_type_arguments()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class Usage
{
   public Dictionary<string, int> Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("DictionaryOfStringInt32");
   }

   [Fact]
   public void Returns_true_and_concatenated_name_for_nested_generic_type()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class Usage
{
   public List<List<int>> Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("ListOfListOfInt32");
   }

   [Fact]
   public void Returns_true_and_concatenated_name_for_deeply_nested_generic_type()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class Usage
{
   public Dictionary<string, List<Dictionary<int, bool>>> Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("DictionaryOfStringListOfDictionaryOfInt32Boolean");
   }

   [Fact]
   public void Returns_true_and_concatenated_name_for_three_type_arguments()
   {
      var src = @"
namespace Test;

public class Container<T1, T2, T3>;

public class Usage
{
   public Container<int, string, bool> Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("ContainerOfInt32StringBoolean");
   }

   [Fact]
   public void Returns_true_and_name_with_array_suffix_for_single_dimension_array()
   {
      var src = @"
namespace Test;

public class Usage
{
   public int[] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("Int32Array");
   }

   [Fact]
   public void Returns_true_and_name_with_array_suffix_for_reference_type_array()
   {
      var src = @"
namespace Test;

public class Usage
{
   public string[] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("StringArray");
   }

   [Fact]
   public void Returns_true_and_name_with_array_suffix_for_custom_class_array()
   {
      var src = @"
namespace Test;

public class MyClass;

public class Usage
{
   public MyClass[] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("MyClassArray");
   }

   [Fact]
   public void Returns_true_and_name_with_array_suffix_for_jagged_array()
   {
      var src = @"
namespace Test;

public class Usage
{
   public int[][] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("Int32ArrayArray");
   }

   [Fact]
   public void Returns_true_and_name_with_array_suffix_for_triple_nested_array()
   {
      var src = @"
namespace Test;

public class Usage
{
   public string[][][] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("StringArrayArrayArray");
   }

   [Fact]
   public void Returns_true_and_name_with_2D_suffix_for_two_dimensional_array()
   {
      var src = @"
namespace Test;

public class Usage
{
   public int[,] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("Int32Array2D");
   }

   [Fact]
   public void Returns_true_and_name_with_3D_suffix_for_three_dimensional_array()
   {
      var src = @"
namespace Test;

public class Usage
{
   public int[,,] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("Int32Array3D");
   }

   [Fact]
   public void Returns_true_and_name_with_rank_suffix_for_higher_dimensional_array()
   {
      var src = @"
namespace Test;

public class Usage
{
   public int[,,,] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("Int32Array4D");
   }

   [Fact]
   public void Returns_true_and_name_with_2D_suffix_for_two_dimensional_string_array()
   {
      var src = @"
namespace Test;

public class Usage
{
   public string[,] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("StringArray2D");
   }

   [Fact]
   public void Returns_true_and_name_for_array_of_generic_type()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class Usage
{
   public List<int>[] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("ListOfInt32Array");
   }

   [Fact]
   public void Returns_true_and_name_for_generic_type_of_array()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class Usage
{
   public List<int[]> Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("ListOfInt32Array");
   }

   [Fact]
   public void Returns_true_and_name_for_complex_nested_generics_with_arrays()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class Usage
{
   public Dictionary<string[], List<int>[]>[] Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("DictionaryOfStringArrayListOfInt32ArrayArray");
   }

   [Fact]
   public void Returns_true_and_name_for_tuple_type()
   {
      var src = @"
using System;

namespace Test;

public class Usage
{
   public Tuple<int, string> Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("TupleOfInt32String");
   }

   [Fact]
   public void Returns_true_and_name_for_nullable_value_type()
   {
      var src = @"
namespace Test;

public class Usage
{
   public int? Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("NullableOfInt32");
   }

   [Fact]
   public void Returns_true_and_name_for_type_parameter()
   {
      var src = @"
namespace Test;

public class Container<T>
{
   public T Field;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.Container`1");
      var field = type.GetMembers().OfType<IFieldSymbol>().First();
      var typeParameter = field.Type;

      var result = typeParameter.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("T");
   }

   [Fact]
   public void Returns_true_and_name_for_generic_type_with_type_parameter_argument()
   {
      var src = @"
using System.Collections.Generic;

namespace Test;

public class Container<T>
{
   public List<T> Field;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.Container`1");
      var field = type.GetMembers().OfType<IFieldSymbol>().First();
      var listOfT = field.Type;

      var result = listOfT.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("ListOfT");
   }

   [Fact]
   public void Returns_false_for_pointer_type()
   {
      var src = @"
namespace Test;

public unsafe class Usage
{
   public int* Field;
}
";
      var compilation = CreateCompilation(src, allowUnsafe: true);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeFalse();
      name.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_function_pointer_type()
   {
      var src = @"
namespace Test;

public unsafe class Usage
{
   public delegate*<int, void> Field;
}
";
      var compilation = CreateCompilation(src, allowUnsafe: true);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeFalse();
      name.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_dynamic_type()
   {
      var src = @"
namespace Test;

public class Usage
{
   public dynamic Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeFalse();
      name.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_error_type()
   {
      var src = @"
namespace Test;

public class Usage
{
   public NonExistentType Field;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["The type or namespace name 'NonExistentType' could not be found (are you missing a using directive or an assembly reference?)"]);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeFalse();
      name.Should().BeNull();
   }

   [Fact]
   public void Returns_true_and_name_for_nested_class()
   {
      var src = @"
namespace Test;

public class Outer
{
   public class Inner;
}

public class Usage
{
   public Outer.Inner Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("Inner");
   }

   [Fact]
   public void Returns_true_and_name_for_generic_nested_in_generic()
   {
      var src = @"
namespace Test;

public class Outer<TOuter>
{
   public class Inner<TInner>
   {
   }
}

public class Usage
{
   public Outer<int>.Inner<string> Field;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var field = usageType.GetMembers().OfType<IFieldSymbol>().First();
      var type = field.Type;

      var result = type.TryBuildMemberName(out var name);

      result.Should().BeTrue();
      name.Should().Be("InnerOfString");
   }

   // ITypeSymbol overload - Built-in types
   [Fact]
   public void Returns_true_and_name_for_all_built_in_numeric_types()
   {
      var src = @"
namespace Test;

public class Usage
{
   public byte ByteField;
   public sbyte SByteField;
   public short ShortField;
   public ushort UShortField;
   public int IntField;
   public uint UIntField;
   public long LongField;
   public ulong ULongField;
   public float FloatField;
   public double DoubleField;
   public decimal DecimalField;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var fields = usageType.GetMembers().OfType<IFieldSymbol>().ToArray();

      var byteResult = fields[0].Type.TryBuildMemberName(out var byteName);
      byteResult.Should().BeTrue();
      byteName.Should().Be("Byte");

      var sbyteResult = fields[1].Type.TryBuildMemberName(out var sbyteName);
      sbyteResult.Should().BeTrue();
      sbyteName.Should().Be("SByte");

      var shortResult = fields[2].Type.TryBuildMemberName(out var shortName);
      shortResult.Should().BeTrue();
      shortName.Should().Be("Int16");

      var ushortResult = fields[3].Type.TryBuildMemberName(out var ushortName);
      ushortResult.Should().BeTrue();
      ushortName.Should().Be("UInt16");

      var intResult = fields[4].Type.TryBuildMemberName(out var intName);
      intResult.Should().BeTrue();
      intName.Should().Be("Int32");

      var uintResult = fields[5].Type.TryBuildMemberName(out var uintName);
      uintResult.Should().BeTrue();
      uintName.Should().Be("UInt32");

      var longResult = fields[6].Type.TryBuildMemberName(out var longName);
      longResult.Should().BeTrue();
      longName.Should().Be("Int64");

      var ulongResult = fields[7].Type.TryBuildMemberName(out var ulongName);
      ulongResult.Should().BeTrue();
      ulongName.Should().Be("UInt64");

      var floatResult = fields[8].Type.TryBuildMemberName(out var floatName);
      floatResult.Should().BeTrue();
      floatName.Should().Be("Single");

      var doubleResult = fields[9].Type.TryBuildMemberName(out var doubleName);
      doubleResult.Should().BeTrue();
      doubleName.Should().Be("Double");

      var decimalResult = fields[10].Type.TryBuildMemberName(out var decimalName);
      decimalResult.Should().BeTrue();
      decimalName.Should().Be("Decimal");
   }

   [Fact]
   public void Returns_true_and_name_for_char_and_bool()
   {
      var src = @"
namespace Test;

public class Usage
{
   public char CharField;
   public bool BoolField;
}
";
      var compilation = CreateCompilation(src);
      var usageType = GetTypeSymbol(compilation, "Test.Usage");
      var fields = usageType.GetMembers().OfType<IFieldSymbol>().ToArray();

      var charResult = fields[0].Type.TryBuildMemberName(out var charName);
      charResult.Should().BeTrue();
      charName.Should().Be("Char");

      var boolResult = fields[1].Type.TryBuildMemberName(out var boolName);
      boolResult.Should().BeTrue();
      boolName.Should().Be("Boolean");
   }
}
