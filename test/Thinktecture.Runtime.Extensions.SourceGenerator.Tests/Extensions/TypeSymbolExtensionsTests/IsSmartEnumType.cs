using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsSmartEnumType : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_keyless_SmartEnum()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum]
public partial class MyEnum;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.IsSmartEnumType(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
   }

   [Fact]
   public void Returns_true_for_keyed_SmartEnum()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.IsSmartEnumType(out var attr);

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

      var result = type.IsSmartEnumType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_special_framework_type()
   {
      var compilation = CreateCompilation("");
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);

      var result = intType.IsSmartEnumType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsSmartEnumType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }
}
