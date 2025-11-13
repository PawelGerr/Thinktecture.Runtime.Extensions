using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsUnionSwitchMapOverloadAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_UnionSwitchMapOverloadAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[UnionSwitchMapOverload]
public partial class MyClass;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(UnionSwitchMapOverloadAttribute).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsUnionSwitchMapOverloadAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsUnionSwitchMapOverloadAttribute();

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
      var errorType = myClass.BaseType;

      var result = errorType.IsUnionSwitchMapOverloadAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_name_correct_namespace()
   {
      var src = @"
namespace Thinktecture;

public class WrongAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Thinktecture.WrongAttribute");

      var result = type.IsUnionSwitchMapOverloadAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_correct_name_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class UnionSwitchMapOverloadAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.UnionSwitchMapOverloadAttribute");

      var result = type.IsUnionSwitchMapOverloadAttribute();

      result.Should().BeFalse();
   }
}
