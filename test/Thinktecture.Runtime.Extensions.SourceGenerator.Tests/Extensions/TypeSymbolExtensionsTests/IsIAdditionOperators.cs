using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsIAdditionOperators : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_IAdditionOperators_with_matching_type_parameters()
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

      var result = additionInterface.IsIAdditionOperators(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_IAdditionOperators_with_wrong_first_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IAdditionOperators<string, MyClass, MyClass>
{
   public static MyClass operator +(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors:
      [
         "The type 'string' cannot be used as type parameter 'TSelf' in the generic type or method 'IAdditionOperators<TSelf, TOther, TResult>'. There is no implicit reference conversion from 'string' to 'System.Numerics.IAdditionOperators<string, Test.MyClass, Test.MyClass>'.",
         "'MyClass' does not implement interface member 'IAdditionOperators<string, MyClass, MyClass>.operator +(string, MyClass)'"
      ]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var additionInterface = classType.AllInterfaces.First(i => i.Name == "IAdditionOperators");

      var result = additionInterface.IsIAdditionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IAdditionOperators_with_wrong_second_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IAdditionOperators<MyClass, string, MyClass>
{
   public static MyClass operator +(MyClass left, string right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var additionInterface = classType.AllInterfaces.First(i => i.Name == "IAdditionOperators");

      var result = additionInterface.IsIAdditionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IAdditionOperators_with_wrong_third_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IAdditionOperators<MyClass, MyClass, string>
{
   public static string operator +(MyClass left, MyClass right) => string.Empty;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var additionInterface = classType.AllInterfaces.First(i => i.Name == "IAdditionOperators");

      var result = additionInterface.IsIAdditionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_IAdditionOperators_int_from_framework()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);
      var additionInterface = intType.AllInterfaces.FirstOrDefault(i => i.Name == "IAdditionOperators");

      var result = additionInterface.IsIAdditionOperators(intType);
      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_name()
   {
      var src = @"
namespace Test;

public interface IMyAdditionOperators<T1, T2, T3>
{
   static abstract T3 operator +(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMyAdditionOperators`3");
      var classType = GetTypeSymbol(compilation, "Test.IMyAdditionOperators`3");

      var result = interfaceType.IsIAdditionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_namespace()
   {
      var src = @"
namespace MyNamespace;

public interface IAdditionOperators<T1, T2, T3>
{
   static abstract T3 operator +(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "MyNamespace.IAdditionOperators`3");
      var classType = GetTypeSymbol(compilation, "MyNamespace.IAdditionOperators`3");

      var result = interfaceType.IsIAdditionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_in_wrong_nested_namespace()
   {
      var src = @"
namespace System.Collections;

public interface IAdditionOperators<T1, T2, T3>
{
   static abstract T3 operator +(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Collections.IAdditionOperators`3");
      var classType = GetTypeSymbol(compilation, "System.Collections.IAdditionOperators`3");

      var result = interfaceType.IsIAdditionOperators(classType);

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

      var result = classType.IsIAdditionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_non_generic_interface()
   {
      var src = @"
namespace System.Numerics;

public interface IAdditionOperators
{
   void Add();
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.IAdditionOperators");
      var classType = GetTypeSymbol(compilation, "System.Numerics.IAdditionOperators");

      var result = interfaceType.IsIAdditionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_generic_interface_with_wrong_arity()
   {
      var src = @"
namespace System.Numerics;

public interface IAdditionOperators<T1, T2>
{
   static abstract T2 operator +(T1 left, T1 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.IAdditionOperators`2");
      var classType = GetTypeSymbol(compilation, "System.Numerics.IAdditionOperators`2");

      var result = interfaceType.IsIAdditionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_ISubtractionOperators_interface()
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

      var result = subtractionInterface.IsIAdditionOperators(classType);

      result.Should().BeFalse();
   }
}
