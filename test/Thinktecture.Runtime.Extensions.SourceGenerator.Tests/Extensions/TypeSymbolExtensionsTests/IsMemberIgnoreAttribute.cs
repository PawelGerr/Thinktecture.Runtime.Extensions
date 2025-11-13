using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsMemberIgnoreAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_IgnoreMemberAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyType
{
   [IgnoreMember]
   public string Name { get; }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(IgnoreMemberAttribute).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyType");
      var property = type.GetMembers("Name").First();
      var attribute = property.GetAttributes().First();
      var attributeType = attribute.AttributeClass;

      var result = attributeType.IsMemberIgnoreAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsMemberIgnoreAttribute();

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

      var result = errorType.IsMemberIgnoreAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace;

public class IgnoreMemberAttribute;
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "WrongNamespace.IgnoreMemberAttribute");

      var result = type.IsMemberIgnoreAttribute();

      result.Should().BeFalse();
   }
}
