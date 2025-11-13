using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsComplexValueObjectAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_ComplexValueObjectAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[ComplexValueObject]
public partial class MyValueObject;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(ComplexValueObjectAttribute).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyValueObject");
      var attribute = type.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsComplexValueObjectAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_KeyedValueObjectAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[ValueObject<int>]
public partial class MyValueObject;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(ValueObjectAttribute<>).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyValueObject");
      var attribute = type.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsComplexValueObjectAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsComplexValueObjectAttribute();

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

      var result = errorType.IsComplexValueObjectAttribute();

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

      var result = type.IsComplexValueObjectAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_correct_name_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class ComplexValueObjectAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.ComplexValueObjectAttribute");

      var result = type.IsComplexValueObjectAttribute();

      result.Should().BeFalse();
   }
}
