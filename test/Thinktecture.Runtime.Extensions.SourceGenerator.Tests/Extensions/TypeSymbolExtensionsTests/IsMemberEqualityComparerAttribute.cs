using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsMemberEqualityComparerAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_MemberEqualityComparerAttribute()
   {
      var src = @"
using Thinktecture;
using System.Collections.Generic;

namespace Test;

public partial class MyType
{
   [MemberEqualityComparer<MyEqualityComparer, string>]
   public string Name { get; }
}

public class MyEqualityComparer : Thinktecture.IEqualityComparerAccessor<string>
{
   public static IEqualityComparer<string> EqualityComparer => System.StringComparer.Ordinal;
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(MemberEqualityComparerAttribute<,>).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyType");
      var property = type.GetMembers("Name").First();
      var attribute = property.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsMemberEqualityComparerAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsMemberEqualityComparerAttribute();

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

      var result = errorType.IsMemberEqualityComparerAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_name()
   {
      var src = @"
namespace Thinktecture;

public class WrongAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Thinktecture.WrongAttribute");

      var result = type.IsMemberEqualityComparerAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class MemberEqualityComparerAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.MemberEqualityComparerAttribute");

      var result = type.IsMemberEqualityComparerAttribute();

      result.Should().BeFalse();
   }
}
