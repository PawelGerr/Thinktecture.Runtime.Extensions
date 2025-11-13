using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsKeyMemberComparerAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_KeyMemberComparerAttribute()
   {
      var src = @"
using Thinktecture;
using System.Collections.Generic;

namespace Test;

[KeyMemberComparer<MyComparer, string>]
public partial class MyType;

public class MyComparer : Thinktecture.IComparerAccessor<string>
{
   public static IComparer<string> Comparer => System.StringComparer.Ordinal;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(KeyMemberComparerAttribute<,>).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyType");
      var attribute = type.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsKeyMemberComparerAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      INamedTypeSymbol? type = null;

      var result = type.IsKeyMemberComparerAttribute();

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

      var result = myClass.BaseType.IsKeyMemberComparerAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_arity()
   {
      var src = @"
using Thinktecture;

namespace Test;

public class KeyMemberComparerAttribute<T> : System.Attribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.KeyMemberComparerAttribute`1");

      var result = type.IsKeyMemberComparerAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class KeyMemberComparerAttribute<T, U, V>;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.KeyMemberComparerAttribute`3");

      var result = type.IsKeyMemberComparerAttribute();

      result.Should().BeFalse();
   }
}
