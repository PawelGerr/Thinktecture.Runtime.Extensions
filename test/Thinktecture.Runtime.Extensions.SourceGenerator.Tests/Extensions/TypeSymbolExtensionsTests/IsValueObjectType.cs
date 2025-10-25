using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsValueObjectType : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_keyed_ValueObject()
   {
      var src = @"
using Thinktecture;

namespace Test;

[ValueObject<int>]
public partial class MyVo;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(ValueObjectAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyVo");

      var result = type.IsValueObjectType(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
   }

   [Fact]
   public void Returns_true_for_complex_ValueObject()
   {
      var src = @"
using Thinktecture;

namespace Test;

[ComplexValueObject]
public partial class MyVo
{
   public string Name { get; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(ComplexValueObjectAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyVo");

      var result = type.IsValueObjectType(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
   }

   [Fact]
   public void Returns_false_for_non_annotated_type()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.IsValueObjectType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_special_framework_type()
   {
      var compilation = CreateCompilation("");
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);

      var result = intType.IsValueObjectType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsValueObjectType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }
}
