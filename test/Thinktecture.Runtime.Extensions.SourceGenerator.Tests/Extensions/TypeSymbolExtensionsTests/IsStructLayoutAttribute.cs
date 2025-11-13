using System.Linq;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsStructLayoutAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_System_Runtime_InteropServices_StructLayoutAttribute()
   {
      var src = @"
using System.Runtime.InteropServices;

namespace Test;

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct
{
   public int Value;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyStruct");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsStructLayoutAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace_depth()
   {
      var src = @"
namespace System.InteropServices;

public class StructLayoutAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "System.InteropServices.StructLayoutAttribute");

      var result = type.IsStructLayoutAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_correct_name_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class StructLayoutAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.StructLayoutAttribute");

      var result = type.IsStructLayoutAttribute();

      result.Should().BeFalse();
   }
}
