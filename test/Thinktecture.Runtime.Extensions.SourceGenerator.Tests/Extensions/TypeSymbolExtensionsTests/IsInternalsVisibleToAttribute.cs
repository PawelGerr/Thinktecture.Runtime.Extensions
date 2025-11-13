using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsInternalsVisibleToAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_InternalsVisibleToAttribute()
   {
      var src = @"
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(""TestAssembly"")]

namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var attributeType = compilation.GetTypeByMetadataName("System.Runtime.CompilerServices.InternalsVisibleToAttribute");

      var result = attributeType.IsInternalsVisibleToAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsInternalsVisibleToAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_error_type()
   {
      var src = @"
namespace Test;

public class MyClass : NonExistentType;
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["The type or namespace name 'NonExistentType' could not be found (are you missing a using directive or an assembly reference?)"]);
      var myClass = GetTypeSymbol(compilation, "Test.MyClass");
      var errorType = myClass.BaseType; // NonExistentType will be an error type

      var result = errorType.IsInternalsVisibleToAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_name_correct_namespace()
   {
      var src = @"
namespace System.Runtime.CompilerServices;

public class WrongAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "System.Runtime.CompilerServices.WrongAttribute");

      var result = type.IsInternalsVisibleToAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_correct_name_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class InternalsVisibleToAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.InternalsVisibleToAttribute");

      var result = type.IsInternalsVisibleToAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_user_defined_type()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.IsInternalsVisibleToAttribute();

      result.Should().BeFalse();
   }
}
