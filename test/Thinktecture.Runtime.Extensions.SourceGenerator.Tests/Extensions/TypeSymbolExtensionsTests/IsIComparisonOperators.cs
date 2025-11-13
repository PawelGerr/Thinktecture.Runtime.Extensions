using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsIComparisonOperators : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_IComparisonOperators_with_matching_type_parameters_and_bool_result()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IComparisonOperators<MyClass, MyClass, bool>
{
   public static bool operator <(MyClass left, MyClass right) => false;
   public static bool operator >(MyClass left, MyClass right) => false;
   public static bool operator <=(MyClass left, MyClass right) => false;
   public static bool operator >=(MyClass left, MyClass right) => false;
   public static bool operator ==(MyClass left, MyClass right) => false;
   public static bool operator !=(MyClass left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var comparisonInterface = classType.AllInterfaces.First(i => i.Name == "IComparisonOperators");

      var result = comparisonInterface.IsIComparisonOperators(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_IComparisonOperators_with_wrong_first_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IComparisonOperators<string, MyClass, bool>
{
   public static bool operator <(MyClass left, MyClass right) => false;
   public static bool operator >(MyClass left, MyClass right) => false;
   public static bool operator <=(MyClass left, MyClass right) => false;
   public static bool operator >=(MyClass left, MyClass right) => false;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors:
      [
         "The type 'string' cannot be used as type parameter 'TSelf' in the generic type or method 'IComparisonOperators<TSelf, TOther, TResult>'. There is no implicit reference conversion from 'string' to 'System.Numerics.IComparisonOperators<string, Test.MyClass, bool>'.",
         "The type 'string' cannot be used as type parameter 'TSelf' in the generic type or method 'IEqualityOperators<TSelf, TOther, TResult>'. There is no implicit reference conversion from 'string' to 'System.Numerics.IEqualityOperators<string, Test.MyClass, bool>'.",
         "'MyClass' does not implement interface member 'IComparisonOperators<string, MyClass, bool>.operator <(string, MyClass)'",
         "'MyClass' does not implement interface member 'IComparisonOperators<string, MyClass, bool>.operator <=(string, MyClass)'",
         "'MyClass' does not implement interface member 'IComparisonOperators<string, MyClass, bool>.operator >(string, MyClass)'",
         "'MyClass' does not implement interface member 'IComparisonOperators<string, MyClass, bool>.operator >=(string, MyClass)'",
         "'MyClass' does not implement interface member 'IEqualityOperators<string, MyClass, bool>.operator ==(string?, MyClass?)'",
         "'MyClass' does not implement interface member 'IEqualityOperators<string, MyClass, bool>.operator !=(string?, MyClass?)'"
      ]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var comparisonInterface = classType.AllInterfaces.First(i => i.Name == "IComparisonOperators");

      var result = comparisonInterface.IsIComparisonOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IComparisonOperators_with_wrong_second_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IComparisonOperators<MyClass, string, bool>
{
   public static bool operator <(MyClass left, string right) => false;
   public static bool operator >(MyClass left, string right) => false;
   public static bool operator <=(MyClass left, string right) => false;
   public static bool operator >=(MyClass left, string right) => false;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors:
      [
         "'MyClass' does not implement interface member 'IEqualityOperators<MyClass, string, bool>.operator ==(MyClass?, string?)'",
         "'MyClass' does not implement interface member 'IEqualityOperators<MyClass, string, bool>.operator !=(MyClass?, string?)'"
      ]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var comparisonInterface = classType.AllInterfaces.First(i => i.Name == "IComparisonOperators");

      var result = comparisonInterface.IsIComparisonOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IComparisonOperators_with_non_bool_third_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.Numerics.IComparisonOperators<MyClass, MyClass, int>
{
   public static int operator <(MyClass left, MyClass right) => 0;
   public static int operator >(MyClass left, MyClass right) => 0;
   public static int operator <=(MyClass left, MyClass right) => 0;
   public static int operator >=(MyClass left, MyClass right) => 0;
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors:
      [
         "'MyClass' does not implement interface member 'IEqualityOperators<MyClass, MyClass, int>.operator ==(MyClass?, MyClass?)'",
         "'MyClass' does not implement interface member 'IEqualityOperators<MyClass, MyClass, int>.operator !=(MyClass?, MyClass?)'"
      ]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var comparisonInterface = classType.AllInterfaces.First(i => i.Name == "IComparisonOperators");

      var result = comparisonInterface.IsIComparisonOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_IComparisonOperators_int_from_framework()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);
      var comparisonInterface = intType.AllInterfaces.FirstOrDefault(i => i.Name == "IComparisonOperators");

      var result = comparisonInterface.IsIComparisonOperators(intType);
      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_name()
   {
      var src = @"
namespace Test;

public interface IMyComparisonOperators<T1, T2, T3>
{
   static abstract T3 operator <(T1 left, T2 right);
   static abstract T3 operator >(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMyComparisonOperators`3");
      var classType = GetTypeSymbol(compilation, "Test.IMyComparisonOperators`3");

      var result = interfaceType.IsIComparisonOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_namespace()
   {
      var src = @"
namespace MyNamespace;

public interface IComparisonOperators<T1, T2, T3>
{
   static abstract T3 operator <(T1 left, T2 right);
   static abstract T3 operator >(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "MyNamespace.IComparisonOperators`3");
      var classType = GetTypeSymbol(compilation, "MyNamespace.IComparisonOperators`3");

      var result = interfaceType.IsIComparisonOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_in_wrong_nested_namespace()
   {
      var src = @"
namespace System.Collections;

public interface IComparisonOperators<T1, T2, T3>
{
   static abstract T3 operator <(T1 left, T2 right);
   static abstract T3 operator >(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Collections.IComparisonOperators`3");
      var classType = GetTypeSymbol(compilation, "System.Collections.IComparisonOperators`3");

      var result = interfaceType.IsIComparisonOperators(classType);

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

      var result = classType.IsIComparisonOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_non_generic_interface()
   {
      var src = @"
namespace System.Numerics;

public interface IComparisonOperators
{
   void Compare();
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.IComparisonOperators");
      var classType = GetTypeSymbol(compilation, "System.Numerics.IComparisonOperators");

      var result = interfaceType.IsIComparisonOperators(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_generic_interface_with_wrong_arity()
   {
      var src = @"
namespace System.Numerics;

public interface IComparisonOperators<T1, T2>
{
   static abstract bool operator <(T1 left, T2 right);
   static abstract bool operator >(T1 left, T2 right);
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["One of the parameters of a binary operator must be the containing type, or its type parameter constrained to it."]);
      var interfaceType = GetTypeSymbol(compilation, "System.Numerics.IComparisonOperators`2");
      var classType = GetTypeSymbol(compilation, "System.Numerics.IComparisonOperators`2");

      var result = interfaceType.IsIComparisonOperators(classType);

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

      var result = additionInterface.IsIComparisonOperators(classType);

      result.Should().BeFalse();
   }
}
