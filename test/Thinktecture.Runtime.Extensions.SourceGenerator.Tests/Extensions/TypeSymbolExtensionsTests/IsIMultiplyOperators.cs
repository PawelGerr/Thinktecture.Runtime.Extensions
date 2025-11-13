using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsIMultiplyOperators : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_IMultiplyOperators_with_matching_type_parameters()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IMultiplyOperators<MyClass, MyClass, MyClass>
{
   public static MyClass operator *(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var multiplyInterface = classType.AllInterfaces.First(i => i.Name == "IMultiplyOperators");

      var result = multiplyInterface.IsIMultiplyOperators(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_IMultiplyOperators_with_wrong_first_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IMultiplyOperators<string, MyClass, MyClass>
{
   public static MyClass operator *(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors:
      [
         "The type 'string' cannot be used as type parameter 'TSelf' in the generic type or method 'IMultiplyOperators<TSelf, TOther, TResult>'. There is no implicit reference conversion from 'string' to 'System.Numerics.IMultiplyOperators<string, Test.MyClass, Test.MyClass>'.",
         "'MyClass' does not implement interface member 'IMultiplyOperators<string, MyClass, MyClass>.operator *(string, MyClass)'"
      ]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var multiplyInterface = classType.AllInterfaces.First(i => i.Name == "IMultiplyOperators");

      var result = multiplyInterface.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IMultiplyOperators_with_wrong_second_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IMultiplyOperators<MyClass, string, MyClass>
{
   public static MyClass operator *(MyClass left, string right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var multiplyInterface = classType.AllInterfaces.First(i => i.Name == "IMultiplyOperators");

      var result = multiplyInterface.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IMultiplyOperators_with_wrong_third_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IMultiplyOperators<MyClass, MyClass, string>
{
   public static string operator *(MyClass left, MyClass right) => string.Empty;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var multiplyInterface = classType.AllInterfaces.First(i => i.Name == "IMultiplyOperators");

      var result = multiplyInterface.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_IMultiplyOperators_int_from_framework()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);
      var multiplyInterface = intType.AllInterfaces.FirstOrDefault(i => i.Name == "IMultiplyOperators");

      var result = multiplyInterface.IsIMultiplyOperators(intType);
      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_name()
   {
      var src = @"
namespace Test;

public interface IMyMultiplyOperators<T1, T2, T3>
{
   static abstract T3 operator *(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMyMultiplyOperators`3");
      var classType = GetTypeSymbol(compilation, "Test.IMyMultiplyOperators`3");

      var result = interfaceType.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_namespace()
   {
      var src = @"
namespace MyNamespace;

public interface IMultiplyOperators<T1, T2, T3>
{
   static abstract T3 operator *(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "MyNamespace.IMultiplyOperators`3");
      var classType = GetTypeSymbol(compilation, "MyNamespace.IMultiplyOperators`3");

      var result = interfaceType.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_in_wrong_nested_namespace()
   {
      var src = @"
namespace System.Collections;

public interface IMultiplyOperators<T1, T2, T3>
{
   static abstract T3 operator *(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Collections.IMultiplyOperators`3");
      var classType = GetTypeSymbol(compilation, "System.Collections.IMultiplyOperators`3");

      var result = interfaceType.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_non_interface_type()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var result = classType.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_non_generic_interface()
   {
      var src = @"
namespace System.Numerics;

public interface IMultiplyOperators
{
   void Multiply();
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.IMultiplyOperators");
      var classType = GetTypeSymbol(compilation, "System.Numerics.IMultiplyOperators");

      var result = interfaceType.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_generic_interface_with_wrong_arity()
   {
      var src = @"
namespace System.Numerics;

public interface IMultiplyOperators<T1, T2>
{
   static abstract T2 operator *(T1 left, T1 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.IMultiplyOperators`2");
      var classType = GetTypeSymbol(compilation, "System.Numerics.IMultiplyOperators`2");

      var result = interfaceType.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IDivisionOperators_interface()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IDivisionOperators<MyClass, MyClass, MyClass>
{
   public static MyClass operator /(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var divisionInterface = classType.AllInterfaces.First(i => i.Name == "IDivisionOperators");

      var result = divisionInterface.IsIMultiplyOperators(classType);

      result.Should().BeFalse();
   }
}
