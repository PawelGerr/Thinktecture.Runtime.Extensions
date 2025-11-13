using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsMessagePackKeyAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_MessagePack_KeyAttribute()
   {
      var src = @"
namespace Test;

public class MyClass
{
   [MessagePack.Key(0)]
   public string Name { get; set; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.KeyAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Name").First();
      var attribute = property.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsMessagePackKeyAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsMessagePackKeyAttribute();

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

      var result = errorType.IsMessagePackKeyAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_name_correct_namespace()
   {
      var src = @"
namespace MessagePack;

public class WrongAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "MessagePack.WrongAttribute");

      var result = type.IsMessagePackKeyAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_correct_name_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class KeyAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.KeyAttribute");

      var result = type.IsMessagePackKeyAttribute();

      result.Should().BeFalse();
   }
}
