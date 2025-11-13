using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsAnyUnionAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_non_generic_UnionAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union]
public abstract partial class MyUnion;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(UnionAttribute).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyUnion");
      var attribute = type.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsAnyUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_generic_UnionAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union<int, string>]
public partial class MyUnion;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(UnionAttribute<,>).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyUnion");
      var attribute = type.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsAnyUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_AdHocUnionAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[AdHocUnion(typeof(int), typeof(string))]
public partial class MyUnion;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(AdHocUnionAttribute).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyUnion");
      var attribute = type.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsAnyUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsAnyUnionAttribute();

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

      var result = errorType.IsAnyUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_non_union_attribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum]
public partial class MyEnum;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyEnum");
      var attribute = type.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsAnyUnionAttribute();

      result.Should().BeFalse();
   }
}
