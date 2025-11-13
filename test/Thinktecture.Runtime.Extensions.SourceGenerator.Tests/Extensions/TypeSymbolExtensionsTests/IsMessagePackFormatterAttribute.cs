using System.Linq;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsMessagePackFormatterAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_MessagePack_MessagePackFormatterAttribute()
   {
      var src = @"
namespace Test;

[MessagePack.MessagePackFormatter(typeof(string))]
public class MyClass;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MessagePack.MessagePackFormatterAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsMessagePackFormatterAttribute();

      result.Should().BeTrue();
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

      var result = type.IsMessagePackFormatterAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_correct_name_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class MessagePackFormatterAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.MessagePackFormatterAttribute");

      var result = type.IsMessagePackFormatterAttribute();

      result.Should().BeFalse();
   }
}
