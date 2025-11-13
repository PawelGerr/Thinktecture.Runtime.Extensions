using System.Linq;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsValidationErrorAttribute : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_ValidationErrorAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

[ValidationError<ValidationError>]
public partial class MyClass;
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(ValidationErrorAttribute<>).Assembly.Location]);

      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsValidationErrorAttribute();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_wrong_arity()
   {
      var src = @"
namespace Test;

[Thinktecture.ValidationError]
public class MyClass;
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["'ValidationError' is not an attribute class"]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsValidationErrorAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_namespace()
   {
      var src = @"
namespace WrongNamespace
{
   public class ValidationErrorAttribute<T> : System.Attribute;
}

namespace Test
{
   [WrongNamespace.ValidationError<string>]
   public class MyClass;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsValidationErrorAttribute();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_wrong_type_argument_count()
   {
      var src = @"
namespace Thinktecture
{
   public class ValidationErrorAttribute<T1, T2> : System.Attribute;
}



namespace Test
{
   [Thinktecture.ValidationError<string, int>]
   public class MyClass;
}
";
      var compilation = CreateCompilation(src);
      var type = GetTypeSymbol(compilation, "Test.MyClass");
      var attribute = type.GetAttributes().First();

      var result = attribute.AttributeClass?.IsValidationErrorAttribute();

      result.Should().BeFalse();
   }
}
