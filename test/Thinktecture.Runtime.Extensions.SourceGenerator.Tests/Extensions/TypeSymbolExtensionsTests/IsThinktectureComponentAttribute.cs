using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsThinktectureComponentAttribute : CompilationTestBase
{
   [Theory]
   [InlineData("SmartEnum")]
   [InlineData("SmartEnum<int>")]
   [InlineData("ValueObject<int>")]
   [InlineData("ComplexValueObject")]
   [InlineData("Union")]
   [InlineData("Union<int, string>")]
   [InlineData("AdHocUnion(typeof(int), typeof(string))")]
   public void Returns_true_for_Thinktecture_component_attributes(string attributeUsage)
   {
      var src = $@"
using Thinktecture;

namespace Test;

[{attributeUsage}]
public partial class MyType;
";
      var compilation = CreateCompilation(src, additionalReferences:
      [
         typeof(SmartEnumAttribute).Assembly.Location,
         typeof(SmartEnumAttribute<>).Assembly.Location,
         typeof(ValueObjectAttribute<>).Assembly.Location,
         typeof(ComplexValueObjectAttribute).Assembly.Location,
         typeof(UnionAttribute).Assembly.Location,
         typeof(UnionAttribute<,>).Assembly.Location,
         typeof(AdHocUnionAttribute).Assembly.Location
      ]);

      var type = GetTypeSymbol(compilation, "Test.MyType");
      var attribute = type.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsThinktectureComponentAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsThinktectureComponentAttribute();

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

      var result = errorType.IsThinktectureComponentAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_non_Thinktecture_attribute()
   {
      var src = @"
namespace Test;

public class MyAttribute : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyAttribute");

      var result = type.IsThinktectureComponentAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class SmartEnumAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.SmartEnumAttribute");

      var result = type.IsThinktectureComponentAttribute();

      result.Should().BeFalse();
   }
}
