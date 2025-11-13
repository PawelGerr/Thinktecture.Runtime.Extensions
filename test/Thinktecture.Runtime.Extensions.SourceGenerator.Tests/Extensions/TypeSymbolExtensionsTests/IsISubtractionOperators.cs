using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsISubtractionOperators : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_ISubtractionOperators_with_matching_type_parameters()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.ISubtractionOperators<MyClass, MyClass, MyClass>
{
   public static MyClass operator -(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var subtractionInterface = classType.AllInterfaces.First(i => i.Name == "ISubtractionOperators");

      var result = subtractionInterface.IsISubtractionOperators(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_ISubtractionOperators_with_wrong_first_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.ISubtractionOperators<string, MyClass, MyClass>
{
   public static MyClass operator -(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors:
      [
         "The type 'string' cannot be used as type parameter 'TSelf' in the generic type or method 'ISubtractionOperators<TSelf, TOther, TResult>'. There is no implicit reference conversion from 'string' to 'System.Numerics.ISubtractionOperators<string, Test.MyClass, Test.MyClass>'.",
         "'MyClass' does not implement interface member 'ISubtractionOperators<string, MyClass, MyClass>.operator -(string, MyClass)'"
      ]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var subtractionInterface = classType.AllInterfaces.First(i => i.Name == "ISubtractionOperators");

      var result = subtractionInterface.IsISubtractionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_ISubtractionOperators_with_wrong_second_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.ISubtractionOperators<MyClass, string, MyClass>
{
   public static MyClass operator -(MyClass left, string right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var subtractionInterface = classType.AllInterfaces.First(i => i.Name == "ISubtractionOperators");

      var result = subtractionInterface.IsISubtractionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_ISubtractionOperators_with_wrong_third_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.ISubtractionOperators<MyClass, MyClass, string>
{
   public static string operator -(MyClass left, MyClass right) => string.Empty;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var subtractionInterface = classType.AllInterfaces.First(i => i.Name == "ISubtractionOperators");

      var result = subtractionInterface.IsISubtractionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_ISubtractionOperators_int_from_framework()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);
      var subtractionInterface = intType.AllInterfaces.FirstOrDefault(i => i.Name == "ISubtractionOperators");

      var result = subtractionInterface.IsISubtractionOperators(intType);
      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_name()
   {
      var src = @"
namespace Test;

public interface IMySubtractionOperators<T1, T2, T3>
{
   static abstract T3 operator -(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMySubtractionOperators`3");
      var classType = GetTypeSymbol(compilation, "Test.IMySubtractionOperators`3");

      var result = interfaceType.IsISubtractionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_namespace()
   {
      var src = @"
namespace MyNamespace;

public interface ISubtractionOperators<T1, T2, T3>
{
   static abstract T3 operator -(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "MyNamespace.ISubtractionOperators`3");
      var classType = GetTypeSymbol(compilation, "MyNamespace.ISubtractionOperators`3");

      var result = interfaceType.IsISubtractionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_in_wrong_nested_namespace()
   {
      var src = @"
namespace System.Collections;

public interface ISubtractionOperators<T1, T2, T3>
{
   static abstract T3 operator -(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Collections.ISubtractionOperators`3");
      var classType = GetTypeSymbol(compilation, "System.Collections.ISubtractionOperators`3");

      var result = interfaceType.IsISubtractionOperators(classType);

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

      var result = classType.IsISubtractionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_non_generic_interface()
   {
      var src = @"
namespace System.Numerics;

public interface ISubtractionOperators
{
   void Subtract();
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.ISubtractionOperators");
      var classType = GetTypeSymbol(compilation, "System.Numerics.ISubtractionOperators");

      var result = interfaceType.IsISubtractionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_generic_interface_with_wrong_arity()
   {
      var src = @"
namespace System.Numerics;

public interface ISubtractionOperators<T1, T2>
{
   static abstract T2 operator -(T1 left, T1 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.ISubtractionOperators`2");
      var classType = GetTypeSymbol(compilation, "System.Numerics.ISubtractionOperators`2");

      var result = interfaceType.IsISubtractionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IAdditionOperators_interface()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IAdditionOperators<MyClass, MyClass, MyClass>
{
   public static MyClass operator +(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var additionInterface = classType.AllInterfaces.First(i => i.Name == "IAdditionOperators");

      var result = additionInterface.IsISubtractionOperators(classType);

      result.Should().BeFalse();
   }
}
