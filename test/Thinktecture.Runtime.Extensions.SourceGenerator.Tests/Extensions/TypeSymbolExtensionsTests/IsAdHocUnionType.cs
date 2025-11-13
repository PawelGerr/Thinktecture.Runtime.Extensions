using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsAdHocUnionType : CompilationTestBase
{
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

      var result = type.IsAdHocUnionType(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
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

      var result = type.IsAdHocUnionType(out var attr);

      result.Should().BeTrue();
      attr.Should().NotBeNull();
   }

   [Fact]
   public void Returns_false_for_regular_UnionAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[Union]
public abstract partial class MyUnion;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(UnionAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyUnion");

      var result = type.IsAdHocUnionType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsAdHocUnionType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }

   [Fact]
   public void Returns_false_for_special_type()
   {
      var compilation = CreateCompilation("");
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);

      var result = intType.IsAdHocUnionType(out var attr);

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

      var result = type.IsAdHocUnionType(out var attr);

      result.Should().BeFalse();
      attr.Should().BeNull();
   }
}
