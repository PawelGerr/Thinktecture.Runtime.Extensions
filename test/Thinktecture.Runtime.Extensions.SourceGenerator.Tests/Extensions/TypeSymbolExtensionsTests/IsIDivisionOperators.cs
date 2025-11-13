using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsIDivisionOperators : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_IDivisionOperators_with_matching_type_parameters()
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

      var result = divisionInterface.IsIDivisionOperators(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_IDivisionOperators_with_wrong_first_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IDivisionOperators<string, MyClass, MyClass>
{
   public static MyClass operator /(MyClass left, MyClass right) => new MyClass();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors:
      [
         "One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it.",
         "The type 'string' cannot be used as type parameter 'TSelf' in the generic type or method 'IDivisionOperators<TSelf, TOther, TResult>'. There is no implicit reference conversion from 'string' to 'System.Numerics.IDivisionOperators<string, Test.MyClass, Test.MyClass>'.",
         "'MyClass' does not implement interface member 'IDivisionOperators<string, MyClass, MyClass>.operator /(string, MyClass)'"
      ]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var divisionInterface = classType.AllInterfaces.First(i => i.Name == "IDivisionOperators");

      var result = divisionInterface.IsIDivisionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IDivisionOperators_with_wrong_second_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IDivisionOperators<MyClass, string, MyClass>
{
   public static MyClass operator /(MyClass left, string right) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var divisionInterface = classType.AllInterfaces.First(i => i.Name == "IDivisionOperators");

      var result = divisionInterface.IsIDivisionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IDivisionOperators_with_wrong_third_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IDivisionOperators<MyClass, MyClass, string>
{
   public static string operator /(MyClass left, MyClass right) => string.Empty;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var divisionInterface = classType.AllInterfaces.First(i => i.Name == "IDivisionOperators");

      var result = divisionInterface.IsIDivisionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_IDivisionOperators_int_from_framework()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);
      var divisionInterface = intType.AllInterfaces.FirstOrDefault(i => i.Name == "IDivisionOperators");

      var result = divisionInterface.IsIDivisionOperators(intType);
      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_name()
   {
      var src = @"
namespace Test;

public interface IMyDivisionOperators<T1, T2, T3>
{
   static abstract T3 operator /(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMyDivisionOperators`3");
      var classType = GetTypeSymbol(compilation, "Test.IMyDivisionOperators`3");

      var result = interfaceType.IsIDivisionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_namespace()
   {
      var src = @"
namespace MyNamespace;

public interface IDivisionOperators<T1, T2, T3>
{
   static abstract T3 operator /(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "MyNamespace.IDivisionOperators`3");
      var classType = GetTypeSymbol(compilation, "MyNamespace.IDivisionOperators`3");

      var result = interfaceType.IsIDivisionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_in_wrong_nested_namespace()
   {
      var src = @"
namespace System.Collections;

public interface IDivisionOperators<T1, T2, T3>
{
   static abstract T3 operator /(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Collections.IDivisionOperators`3");
      var classType = GetTypeSymbol(compilation, "System.Collections.IDivisionOperators`3");

      var result = interfaceType.IsIDivisionOperators(classType);

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

      var result = classType.IsIDivisionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_non_generic_interface()
   {
      var src = @"
namespace System.Numerics;

public interface IDivisionOperators
{
   void Divide();
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.IDivisionOperators");
      var classType = GetTypeSymbol(compilation, "System.Numerics.IDivisionOperators");

      var result = interfaceType.IsIDivisionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_generic_interface_with_wrong_arity()
   {
      var src = @"
namespace System.Numerics;

public interface IDivisionOperators<T1, T2>
{
   static abstract T2 operator /(T1 left, T1 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.IDivisionOperators`2");
      var classType = GetTypeSymbol(compilation, "System.Numerics.IDivisionOperators`2");

      var result = interfaceType.IsIDivisionOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IMultiplyOperators_interface()
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

      var result = multiplyInterface.IsIDivisionOperators(classType);

      result.Should().BeFalse();
   }
}
