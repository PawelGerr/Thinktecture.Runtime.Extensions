using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsParsableInterface : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_IParsable_with_matching_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.IParsable<MyClass>
{
   public static MyClass Parse(string s, System.IFormatProvider? provider) => new MyClass();
   public static bool TryParse(string? s, System.IFormatProvider? provider, out MyClass result)
   {
      result = new MyClass();
      return true;
   }
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var parsableInterface = classType.AllInterfaces.First(i => i.Name == "IParsable");

      var result = parsableInterface.IsParsableInterface(classType);

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_IParsable_with_wrong_type_parameter()
   {
      var src = @"
namespace Test;

public class MyClass : System.IParsable<string>
{
   public static string Parse(string s, System.IFormatProvider? provider) => s;
   public static bool TryParse(string? s, System.IFormatProvider? provider, out string result)
   {
      result = s ?? string.Empty;
      return true;
   }
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["The type 'string' cannot be used as type parameter 'TSelf' in the generic type or method 'IParsable<TSelf>'. There is no implicit reference conversion from 'string' to 'System.IParsable<string>'."]);
      var classType = GetTypeSymbol(compilation, "Test.MyClass");
      var parsableInterface = classType.AllInterfaces.First(i => i.Name == "IParsable");

      var result = parsableInterface.IsParsableInterface(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_IParsable_int_from_framework()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);
      var parsableInterface = intType.AllInterfaces.FirstOrDefault(i => i.Name == "IParsable");

      var result = parsableInterface.IsParsableInterface(intType);
      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_non_generic_interface()
   {
      var src = @"
namespace System;

public interface IParsable
{
   void Parse(string s);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "System.IParsable");
      var classType = GetTypeSymbol(compilation, "System.IParsable");

      var result = interfaceType.IsParsableInterface(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_name()
   {
      var src = @"
namespace Test;

public interface IMyParsable<T>
{
   static abstract T Parse(string s, System.IFormatProvider? provider);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Test.IMyParsable`1");
      var classType = GetTypeSymbol(compilation, "Test.IMyParsable`1");

      var result = interfaceType.IsParsableInterface(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_with_wrong_namespace()
   {
      var src = @"
namespace MyNamespace;

public interface IParsable<T>
{
   static abstract T Parse(string s, System.IFormatProvider? provider);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "MyNamespace.IParsable`1");
      var classType = GetTypeSymbol(compilation, "MyNamespace.IParsable`1");

      var result = interfaceType.IsParsableInterface(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_interface_in_nested_namespace()
   {
      var src = @"
namespace System.Text;

public interface IParsable<T>
{
   static abstract T Parse(string s, System.IFormatProvider? provider);
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "System.Text.IParsable`1");
      var classType = GetTypeSymbol(compilation, "System.Text.IParsable`1");

      var result = interfaceType.IsParsableInterface(classType);

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

      var result = classType.IsParsableInterface(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_IFormattable_interface()
   {
      var src = @"
namespace Test;

public class MyClass : System.IFormattable
{
   public string ToString(string? s, System.IFormatProvider? provider) => string.Empty;
}";
      var compilation = CreateCompilation(src);
      var formattableInterface = compilation.GetTypeByMetadataName("System.IFormattable");
      var classType = GetTypeSymbol(compilation, "Test.MyClass");

      var result = formattableInterface!.IsParsableInterface(classType);

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_generic_interface_with_multiple_type_parameters()
   {
      var src = @"
namespace System;

public interface IParsable<T1, T2>
{
   static abstract T1 Parse(string s);
}

public class MyClass : IParsable<MyClass, int>
{
   public static MyClass Parse(string s) => new MyClass();
}
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "System.MyClass");
      var parsableInterface = classType.AllInterfaces.First(i => i.Name == "IParsable");

      var result = parsableInterface.IsParsableInterface(classType);

      result.Should().BeTrue();
   }
}
