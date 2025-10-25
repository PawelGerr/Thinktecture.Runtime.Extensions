using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsAnyUnionType : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_regular_union_type()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union]
public partial class MyUnion;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(UnionAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyUnion");

      var result = type.IsAnyUnionType(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
   }

   [Fact]
   public void Returns_true_for_generic_ad_hoc_union_type()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union<string, int>]
public partial class MyUnion;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(UnionAttribute<,>).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyUnion");

      var result = type.IsAnyUnionType(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
   }

   [Fact]
   public void Returns_true_for_non_generic_ad_hoc_union_type()
   {
      var src = @"
using Thinktecture;

namespace Test;

[AdHocUnion(typeof(string), typeof(int))]
public partial class MyUnion;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(AdHocUnionAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyUnion");

      var result = type.IsAnyUnionType(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
   }

   [Fact]
   public void Returns_false_for_non_union_type()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.IsAnyUnionType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_special_framework_type()
   {
      var compilation = CreateCompilation("");
      var objType = compilation.GetSpecialType(SpecialType.System_Object);

      var result = objType.IsAnyUnionType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsAnyUnionType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }
}
