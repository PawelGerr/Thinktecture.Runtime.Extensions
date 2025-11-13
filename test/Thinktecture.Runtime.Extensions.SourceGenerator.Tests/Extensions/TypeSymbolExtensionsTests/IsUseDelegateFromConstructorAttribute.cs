using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsUseDelegateFromConstructorAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_UseDelegateFromConstructorAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyType
{
   [UseDelegateFromConstructor]
   public partial void MyMethod();
}
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location],
         expectedCompilerErrors: ["Partial method 'MyType.MyMethod()' must have an implementation part because it has accessibility modifiers."]);

      var type = GetTypeSymbol(compilation, "Test.MyType");
      var method = type.GetMembers("MyMethod").First();
      var attribute = method.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsUseDelegateFromConstructorAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsUseDelegateFromConstructorAttribute();

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

      var result = errorType.IsUseDelegateFromConstructorAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_name()
   {
      var src = @"
namespace Thinktecture;

public class WrongAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Thinktecture.WrongAttribute");

      var result = type.IsUseDelegateFromConstructorAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class UseDelegateFromConstructorAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.UseDelegateFromConstructorAttribute");

      var result = type.IsUseDelegateFromConstructorAttribute();

      result.Should().BeFalse();
   }
}
