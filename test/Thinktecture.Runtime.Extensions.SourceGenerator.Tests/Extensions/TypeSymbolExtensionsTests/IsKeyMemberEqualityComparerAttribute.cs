using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsKeyMemberEqualityComparerAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_KeyMemberEqualityComparerAttribute()
   {
      var src = @"
using Thinktecture;
using System.Collections.Generic;

namespace Test;

[KeyMemberEqualityComparer<MyEqualityComparer, string>]
public partial class MyType;

public class MyEqualityComparer : Thinktecture.IEqualityComparerAccessor<string>
{
   public static IEqualityComparer<string> EqualityComparer => System.StringComparer.Ordinal;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(KeyMemberEqualityComparerAttribute<,>).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyType");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass.IsKeyMemberEqualityComparerAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      INamedTypeSymbol? type = null;

      var result = type.IsKeyMemberEqualityComparerAttribute();

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

      var result = myClass.BaseType.IsKeyMemberEqualityComparerAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_arity()
   {
      var src = @"
using Thinktecture;

namespace Test;

public class KeyMemberEqualityComparerAttribute<T> : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.KeyMemberEqualityComparerAttribute`1");

      var result = type.IsKeyMemberEqualityComparerAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class KeyMemberEqualityComparerAttribute<T, U, V>;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.KeyMemberEqualityComparerAttribute`3");

      var result = type.IsKeyMemberEqualityComparerAttribute();

      result.Should().BeFalse();
   }
}
