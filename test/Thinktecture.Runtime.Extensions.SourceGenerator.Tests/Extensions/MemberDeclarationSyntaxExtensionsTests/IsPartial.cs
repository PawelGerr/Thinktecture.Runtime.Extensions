using Microsoft.CodeAnalysis.CSharp.Syntax;

#nullable enable

namespace Thinktecture.Runtime.Tests.MemberDeclarationSyntaxExtensionsTests;

public class IsPartial : CompilationTestBase
{
   [Theory]
   [InlineData("class", true)]
   [InlineData("struct", true)]
   [InlineData("record", true)]
   [InlineData("record struct", true)]
   [InlineData("record class", true)]
   [InlineData("interface", true)]
   [InlineData("class", false)]
   [InlineData("struct", false)]
   [InlineData("record", false)]
   [InlineData("record struct", false)]
   [InlineData("record class", false)]
   [InlineData("interface", false)]
   public void Should_handle_type_kinds(string typeKind, bool isPartial)
   {
      var partial = isPartial ? "partial " : "";
      var src = $@"
namespace Test;

public {partial}{typeKind} MyType
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyType");

      var result = typeDeclaration.IsPartial();

      result.Should().Be(isPartial);
   }

   [Theory]
   [InlineData("partial")]
   [InlineData("public static partial")]
   [InlineData("public partial static")]
   public void Should_return_true_when_partial_is_at_different_positions(string modifiers)
   {
      var src = $@"
namespace Test;

{modifiers} class MyClass
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsPartial();

      result.Should().BeTrue();
   }

   [Theory]
   [InlineData("abstract", "class", true)]
   [InlineData("abstract", "class", false)]
   [InlineData("sealed", "class", true)]
   [InlineData("sealed", "class", false)]
   [InlineData("static", "class", true)]
   [InlineData("static", "class", false)]
   [InlineData("readonly", "struct", true)]
   [InlineData("readonly", "struct", false)]
   [InlineData("ref", "struct", true)]
   [InlineData("ref", "struct", false)]
   [InlineData("unsafe", "class", true)]
   [InlineData("file", "class", true)]
   [InlineData("file", "class", false)]
   public void Should_handle_modifiers(string modifier, string typeKind, bool isPartial)
   {
      var partial = isPartial ? "partial " : "";
      var src = $@"
namespace Test;

public {modifier} {partial}{typeKind} MyType
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyType");

      var result = typeDeclaration.IsPartial();

      result.Should().Be(isPartial);
   }

   [Theory]
   [InlineData("private")]
   [InlineData("internal")]
   [InlineData("protected")]
   public void Should_return_true_for_partial_class_with_accessibility_modifier(string accessibility)
   {
      var outerClass = accessibility == "internal" ? "" : @"
public class OuterClass
{
   ";
      var closingBrace = accessibility == "internal" ? "" : @"
}";

      var src = $@"
namespace Test;
{outerClass}{accessibility} partial class MyClass
{{
}}
{closingBrace}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsPartial();

      result.Should().BeTrue();
   }

   [Theory]
   [InlineData(true)]
   [InlineData(false)]
   public void Should_handle_generic_class(bool isPartial)
   {
      var partial = isPartial ? "partial " : "";
      var src = $@"
namespace Test;

public {partial}class MyClass<T>
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsPartial();

      result.Should().Be(isPartial);
   }

   [Fact]
   public void Should_return_true_for_partial_generic_class_with_multiple_type_parameters()
   {
      var src = @"
namespace Test;

public partial class MyClass<T1, T2, T3>
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsPartial();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_partial_generic_class_with_constraints()
   {
      var src = @"
namespace Test;

public partial class MyClass<T> where T : class, new()
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsPartial();

      result.Should().BeTrue();
   }

   [Theory]
   [InlineData(false, true)]
   [InlineData(false, false)]
   [InlineData(true, true)]
   [InlineData(true, false)]
   public void Should_handle_nested_classes(bool outerPartial, bool innerPartial)
   {
      var outerModifier = outerPartial ? "partial " : "";
      var innerModifier = innerPartial ? "partial " : "";
      var src = $@"
namespace Test;

public {outerModifier}class OuterClass
{{
   public {innerModifier}class InnerClass
   {{
   }}
}}
";
      var typeDeclaration = GetTypeDeclaration(src, "InnerClass");

      var result = typeDeclaration.IsPartial();

      result.Should().Be(innerPartial);
   }

   [Theory]
   [InlineData(true)]
   [InlineData(false)]
   public void Should_handle_class_without_namespace(bool isPartial)
   {
      var partial = isPartial ? "partial " : "";
      var src = $@"
public {partial}class MyClass
{{
}}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsPartial();

      result.Should().Be(isPartial);
   }

   [Fact]
   public void Should_return_true_for_partial_class_in_block_scoped_namespace()
   {
      var src = @"
namespace Test
{
   public partial class MyClass
   {
   }
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsPartial();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_class_with_no_modifiers()
   {
      var src = @"
namespace Test;

class MyClass
{
}
";
      var typeDeclaration = GetTypeDeclaration(src, "MyClass");

      var result = typeDeclaration.IsPartial();

      result.Should().BeFalse();
   }

   [Theory]
   [InlineData("partial void MyMethod();", true)]
   [InlineData("public void MyMethod() { }", false)]
   [InlineData("public partial int MyMethod();", true)]
   [InlineData("partial void MyMethod() { }", true)]
   public void Should_handle_partial_methods(string methodDeclaration, bool expectedResult)
   {
      var src = $@"
namespace Test;

public partial class MyClass
{{
   {methodDeclaration}
}}
";
      var method = GetMemberDeclaration<MethodDeclarationSyntax>(src, "MyMethod");

      var result = method.IsPartial();

      result.Should().Be(expectedResult);
   }

   [Fact]
   public void Should_return_correct_result_for_multiple_types_in_same_file()
   {
      var src = @"
namespace Test;

public partial class PartialClass
{
}

public class NonPartialClass
{
}

public partial class AnotherPartialClass
{
}
";
      var partial1 = GetTypeDeclaration(src, "PartialClass");
      var nonPartial = GetTypeDeclaration(src, "NonPartialClass");
      var partial2 = GetTypeDeclaration(src, "AnotherPartialClass");

      partial1.IsPartial().Should().BeTrue();
      nonPartial.IsPartial().Should().BeFalse();
      partial2.IsPartial().Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_for_partial_nested_in_multiple_levels()
   {
      var src = @"
namespace Test;

public class Outer
{
   public class Middle
   {
      public partial class Inner
      {
      }
   }
}
";
      var typeDeclaration = GetTypeDeclaration(src, "Inner");

      var result = typeDeclaration.IsPartial();

      result.Should().BeTrue();
   }
}
