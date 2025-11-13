using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.MethodSymbolExtensionsTests;

public class IsUserDefinedArithmeticOperator : CompilationTestBase
{
   [Fact]
   public void Returns_false_for_regular_static_method_with_matching_signature()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass Add(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("Add").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse(); // Not a user-defined operator
   }

   [Fact]
   public void Returns_true_for_operator_plus()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_operator_minus()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator -(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Subtraction").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_operator_multiply()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator *(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Multiply").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_operator_divide()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator /(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Division").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_method_with_wrong_return_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static int operator +(MyClass left, MyClass right) => 42;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_unary_operator_with_no_second_parameter()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(MyClass value) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_UnaryPlus").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_unary_operator_with_one_parameter()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator -(MyClass value) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_UnaryNegation").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_three_parameters()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(MyClass left, MyClass right, MyClass other) => new MyClass();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Overloaded binary operator '+' takes two parameters"]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      // Not a user-defined operator
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_wrong_first_parameter_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(int left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_wrong_second_parameter_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(MyClass left, int right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_both_wrong_parameter_types()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(int left, string right) => new MyClass();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type"]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_only_first_parameter_correct()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static int operator +(MyClass left, int right) => 42;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_only_second_parameter_correct()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static int operator +(int left, MyClass right) => 42;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_only_return_type_correct()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(int left, string right) => new MyClass();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type"]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_struct_type()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public static MyStruct operator +(MyStruct left, MyStruct right) => new MyStruct();
}
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");
      var operatorMethod = structType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(structType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_record_type()
   {
      var src = @"
namespace Test;

public record MyRecord
{
   public static MyRecord operator +(MyRecord left, MyRecord right) => new MyRecord();
}
";
      var compilation = CreateCompilation(src);
      var recordType = GetTypeSymbol(compilation, "Test.MyRecord");
      var operatorMethod = recordType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(recordType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_void_return_type()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static void operator +(MyClass left, MyClass right) { }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["User-defined operators cannot return void"]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_when_all_nullable_reference_types_match()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public static MyClass? operator +(MyClass? left, MyClass? right) => null;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();
      // Get the nullable type from the method's return type
      var nullableReturnType = operatorMethod.ReturnType;

      var result = operatorMethod.IsUserDefinedArithmeticOperator(nullableReturnType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_checking_non_nullable_class_against_nullable_parameters()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public static MyClass? operator +(MyClass? left, MyClass? right) => null;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_when_return_type_not_nullable_class_but_parameters_are()
   {
      var src = @"
#nullable enable
namespace Test;

public class MyClass
{
   public static MyClass operator +(MyClass? left, MyClass? right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var operatorMethod = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_nullable_value_type_when_types_match()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public static MyStruct? operator +(MyStruct? left, MyStruct? right) => null;
}
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");
      var operatorMethod = structType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();
      // Get the nullable struct type from the operator's return type
      var nullableStructType = operatorMethod.ReturnType;

      var result = operatorMethod.IsUserDefinedArithmeticOperator(nullableStructType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_when_checking_non_nullable_struct_against_nullable_return()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public static MyStruct? operator +(MyStruct left, MyStruct right) => null;
}
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");
      var operatorMethod = structType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(structType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_int_addition_operator()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);
      var addMethod = intType.GetMembers().Where(m => m.Name.Contains("op_Addition")).OfType<IMethodSymbol>().First();

      // Not a user-defined operator
      var result = addMethod.IsUserDefinedArithmeticOperator(intType);
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_instance_method_with_matching_signature()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public MyClass operator +(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["User-defined operator 'MyClass.operator +(MyClass, MyClass)' must be declared static and public"]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      // Not static and not a user-defined operator
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_ref_parameters()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(ref MyClass left, ref MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["ref and out are not valid in this context"]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_out_parameters()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static MyClass operator +(out MyClass left, out MyClass right)
   {
      left = new MyClass();
      right = new MyClass();
      return new MyClass();
   }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["ref and out are not valid in this context"]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_method_with_in_parameters()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public static MyStruct operator +(in MyStruct left, in MyStruct right) => new MyStruct();
}
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Test.MyStruct");
      var method = structType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(structType);
      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_comparison_operator()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public static bool operator ==(MyClass left, MyClass right) => true;
   public static bool operator !=(MyClass left, MyClass right) => false;
   public override bool Equals(object obj) => true;
   public override int GetHashCode() => 0;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("op_Equality").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_generic_class_with_matching_types()
   {
      var src = @"
namespace Test;

public class MyClass<T>
{
   public static MyClass<T> operator +(MyClass<T> left, MyClass<T> right) => new MyClass<T>();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass`1");
      var constructedType = classType.Construct(compilation.GetSpecialType(SpecialType.System_Int32));
      var operatorMethod = constructedType.GetMembers("op_Addition").OfType<IMethodSymbol>().First();

      var result = operatorMethod.IsUserDefinedArithmeticOperator(constructedType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_async_method()
   {
      var src = @"
using System.Threading.Tasks;
namespace Test;

public class MyClass
{
   public static async Task<MyClass> Add(MyClass left, MyClass right) => await Task.FromResult(new MyClass());
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var method = classType.GetMembers("Add").OfType<IMethodSymbol>().First();

      var result = method.IsUserDefinedArithmeticOperator(classType);

      result.Should().BeFalse();
   }
}
