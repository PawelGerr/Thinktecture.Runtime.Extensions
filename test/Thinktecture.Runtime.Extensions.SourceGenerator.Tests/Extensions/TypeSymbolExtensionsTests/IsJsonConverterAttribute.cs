using System.Linq;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsJsonConverterAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_System_Text_Json_JsonConverterAttribute()
   {
      var src = @"
namespace Test;

[System.Text.Json.Serialization.JsonConverter(typeof(string))]
public class MyClass;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(System.Text.Json.Serialization.JsonConverterAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsJsonConverterAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_Newtonsoft_Json_JsonConverterAttribute()
   {
      var src = @"
namespace Test;

[Newtonsoft.Json.JsonConverter(typeof(string))]
public class MyClass;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(Newtonsoft.Json.JsonConverterAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsJsonConverterAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace_depth()
   {
      var src = @"
namespace System.Json.Serialization;

public class JsonConverterAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "System.Json.Serialization.JsonConverterAttribute");

      var result = type.IsJsonConverterAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_correct_name_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class JsonConverterAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.JsonConverterAttribute");

      var result = type.IsJsonConverterAttribute();

      result.Should().BeFalse();
   }
}
