using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsRegularUnionAttribute : CompilationTestBase
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

      var result = attributeType.IsRegularUnionAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_generic_UnionAttribute()
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

      var result = attributeType.IsRegularUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_AdHocUnionAttribute()
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

      var result = attributeType.IsRegularUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsRegularUnionAttribute();

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

      var result = errorType.IsRegularUnionAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_when_not_INamedTypeSymbol()
   {
      var src = @"
namespace Test;

public class MyClass
{
   public int[] ArrayField;
}
";
      var compilation = CreateCompilation(src);
      var myClass = GetTypeSymbol(compilation, "Test.MyClass");
      var field = myClass.GetMembers("ArrayField").First() as IFieldSymbol;
      var arrayType = field!.Type; // This is IArrayTypeSymbol, not INamedTypeSymbol

      var result = arrayType.IsRegularUnionAttribute();

      result.Should().BeFalse();
   }
}
