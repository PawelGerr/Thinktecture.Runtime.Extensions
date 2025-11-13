using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsJsonIgnoreAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_System_Text_Json_JsonIgnoreAttribute()
   {
      var src = @"
namespace Test;

public class MyClass
{
   [System.Text.Json.Serialization.JsonIgnore]
   public string Name { get; set; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(System.Text.Json.Serialization.JsonIgnoreAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var property = type.GetMembers("Name").First();
      var attribute = property.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsJsonIgnoreAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsJsonIgnoreAttribute();

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

      var result = errorType.IsJsonIgnoreAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace_depth()
   {
      var src = @"
namespace System.Json.Serialization;

public class JsonIgnoreAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "System.Json.Serialization.JsonIgnoreAttribute");

      var result = type.IsJsonIgnoreAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_correct_name_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class JsonIgnoreAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.JsonIgnoreAttribute");

      var result = type.IsJsonIgnoreAttribute();

      result.Should().BeFalse();
   }
}
