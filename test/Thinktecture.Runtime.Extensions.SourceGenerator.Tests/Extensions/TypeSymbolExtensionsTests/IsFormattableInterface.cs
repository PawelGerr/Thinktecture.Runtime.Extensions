using System.Linq;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsFormattableInterface : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_actual_IFormattable_interface()
   {
      var src = @"
namespace Test;

public class MyClass : System.IFormattable
{
   public string ToString(string? format, System.IFormatProvider? formatProvider) => string.Empty;
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var formattableInterface = classType.AllInterfaces.First(i => i.Name == "IFormattable");

      var result = formattableInterface.IsFormattableInterface();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_IFormattable_from_framework()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var formattableInterface = compilation.GetTypeByMetadataName("System.IFormattable");

      var result = formattableInterface!.IsFormattableInterface();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_name()
   {
      var src = @"
namespace Test;

public interface IMyFormattable
{
   string ToString(string? format, System.IFormatProvider? formatProvider);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMyFormattable");

      var result = interfaceType.IsFormattableInterface();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_namespace()
   {
      var src = @"
namespace MyNamespace;

public interface IFormattable
{
   string ToString(string? format, System.IFormatProvider? formatProvider);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "MyNamespace.IFormattable");

      var result = interfaceType.IsFormattableInterface();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_in_nested_namespace()
   {
      var src = @"
namespace System.Text;

public interface IFormattable
{
   string ToString(string? format, System.IFormatProvider? formatProvider);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "System.Text.IFormattable");

      var result = interfaceType.IsFormattableInterface();

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

      var result = classType.IsFormattableInterface();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IComparable_interface()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var comparableInterface = compilation.GetTypeByMetadataName("System.IComparable");

      var result = comparableInterface!.IsFormattableInterface();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_generic_interface()
   {
      var src = @"
namespace Test;

public interface IFormattable<T>
{
   string ToString(T value);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Test.IFormattable`1");

      var result = interfaceType.IsFormattableInterface();

      result.Should().BeFalse();
   }
}
