using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsSmartEnum : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_keyless_SmartEnum_with_out_parameter()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum]
public partial class MyEnum;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.IsSmartEnum(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
   }

   [Fact]
   public void Returns_true_for_keyed_SmartEnum_with_out_parameter()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.IsSmartEnum(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
   }

   [Fact]
   public void Returns_true_for_keyless_SmartEnum_without_out_parameter()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum]
public partial class MyEnum;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.IsSmartEnum();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_keyed_SmartEnum_without_out_parameter()
   {
      var src = @"
using Thinktecture;

namespace Test;

[SmartEnum<int>]
public partial class MyEnum;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(SmartEnumAttribute<>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = type.IsSmartEnum();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null_with_out_parameter()
   {
      ITypeSymbol? type = null;

      var result = type.IsSmartEnum(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_null_without_out_parameter()
   {
      ITypeSymbol? type = null;

      var result = type.IsSmartEnum();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_special_type()
   {
      var compilation = CreateCompilation("");
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);

      var result = intType.IsSmartEnum(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
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

      var result = type.IsSmartEnum(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }
}
