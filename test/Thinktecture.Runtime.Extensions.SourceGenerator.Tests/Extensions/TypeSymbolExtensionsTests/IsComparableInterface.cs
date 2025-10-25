using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsComparableInterface : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_non_generic_IComparable_interface()
   {
      var src = @"
namespace Test;

public class MyClass : System.IComparable
{
   public int CompareTo(object? obj) => 0;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var comparableInterface = classType.AllInterfaces.First(i => i is { Name: "IComparable", IsGenericType: false });
      var genericTypeParameter = classType;

      var result = comparableInterface.IsComparableInterface(genericTypeParameter);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_generic_IComparable_with_matching_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.IComparable<MyClass>
{
   public int CompareTo(MyClass? other) => 0;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var comparableInterface = classType.AllInterfaces.First(i => i is { Name: "IComparable", IsGenericType: true });

      var result = comparableInterface.IsComparableInterface(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_generic_IComparable_with_wrong_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.IComparable<string>
{
   public int CompareTo(string? other) => 0;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var comparableInterface = classType.AllInterfaces.First(i => i is { Name: "IComparable", IsGenericType: true });

      var result = comparableInterface.IsComparableInterface(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_name()
   {
      var src = @"
namespace Test;

public interface IMyComparable
{
   int CompareTo(object? obj);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMyComparable");
      var genericTypeParameter = interfaceType;

      var result = interfaceType.IsComparableInterface(genericTypeParameter);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_namespace()
   {
      var src = @"
namespace MyNamespace;

public interface IComparable
{
   int CompareTo(object? obj);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "MyNamespace.IComparable");
      var genericTypeParameter = interfaceType;

      var result = interfaceType.IsComparableInterface(genericTypeParameter);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_in_nested_namespace()
   {
      var src = @"
namespace System.Collections;

public interface IComparable
{
   int CompareTo(object? obj);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "System.Collections.IComparable");
      var genericTypeParameter = interfaceType;

      var result = interfaceType.IsComparableInterface(genericTypeParameter);

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

      var result = classType.IsComparableInterface(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_IComparable_from_framework()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var comparableInterface = compilation.GetTypeByMetadataName("System.IComparable");
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var result = comparableInterface!.IsComparableInterface(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_generic_IComparable_int_from_framework()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);
      var comparableInterface = intType.AllInterfaces.First(i => i is { Name: "IComparable", IsGenericType: true });

      var result = comparableInterface.IsComparableInterface(intType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_IFormattable_interface()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var formattableInterface = compilation.GetTypeByMetadataName("System.IFormattable");
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var result = formattableInterface!.IsComparableInterface(classType);

      result.Should().BeFalse();
   }
}
