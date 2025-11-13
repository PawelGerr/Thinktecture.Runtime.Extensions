using System.Linq;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsObjectFactoryAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_ObjectFactoryAttribute_with_one_type_argument()
   {
      var src = @"
using Thinktecture;

namespace Test;

[ObjectFactory<string>]
public partial class MyClass;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(ObjectFactoryAttribute<>).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsObjectFactoryAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_abstract_ObjectFactoryAttribute()
   {
      var src = @"
namespace Test;

[Thinktecture.ObjectFactory]
public class MyClass;
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(ObjectFactoryAttribute<>).Assembly.Location],
         expectedCompilerErrors: ["Cannot apply attribute class 'ObjectFactoryAttribute' because it is abstract"]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsObjectFactoryAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace
{
   public class ObjectFactoryAttribute<T> : System.Attribute;
}

namespace Test
{
   [WrongNamespace.ObjectFactory<string>]
   public class MyClass;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsObjectFactoryAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_type_argument_count()
   {
      var src = @"
namespace Thinktecture
{
   public class ObjectFactoryAttribute<T1, T2> : System.Attribute;
}


namespace Test
{
   [Thinktecture.ObjectFactory<string, int>]
   public class MyClass;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsObjectFactoryAttribute();

      result.Should().BeFalse();
   }
}
