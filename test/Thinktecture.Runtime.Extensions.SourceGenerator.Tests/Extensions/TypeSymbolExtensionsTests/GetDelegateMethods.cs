using System.Linq;
using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class GetDelegateMethods : CompilationTestBase
{
   [Fact]
   public void Returns_method_with_UseDelegateFromConstructorAttribute()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyClass
{
   [UseDelegateFromConstructor]
   public partial void MyMethod();
}
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location],
         expectedCompilerErrors: ["Partial method 'MyClass.MyMethod()' must have an implementation part because it has accessibility modifiers."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().HaveCount(1);
      result[0].MethodName.Should().Be("MyMethod");
   }

   [Fact]
   public void Returns_empty_when_no_methods_have_attribute()
   {
      var src = @"
namespace Test;

public partial class MyClass
{
   public partial void MyMethod();
}
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["Partial method 'MyClass.MyMethod()' must have an implementation part because it has accessibility modifiers."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Excludes_non_partial_methods()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyClass
{
   [UseDelegateFromConstructor]
   public void NonPartialMethod() { }
}
";
      var compilation = CreateCompilation(src, additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Excludes_generic_methods()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyClass
{
   [UseDelegateFromConstructor]
   public partial void GenericMethod<T>();
}
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location],
         expectedCompilerErrors: ["Partial method 'MyClass.GenericMethod<T>()' must have an implementation part because it has accessibility modifiers."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().BeEmpty();
   }

   [Fact]
   public void Captures_return_type_correctly()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyClass
{
   [UseDelegateFromConstructor]
   public partial string GetValue();
}
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location],
         expectedCompilerErrors: ["Partial method 'MyClass.GetValue()' must have an implementation part because it has accessibility modifiers."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().HaveCount(1);
      result[0].ReturnType.Should().Be("string");
   }

   [Fact]
   public void Returns_null_for_void_return_type()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyClass
{
   [UseDelegateFromConstructor]
   public partial void DoSomething();
}
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location],
         expectedCompilerErrors: ["Partial method 'MyClass.DoSomething()' must have an implementation part because it has accessibility modifiers."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().HaveCount(1);
      result[0].ReturnType.Should().BeNull();
   }

   [Fact]
   public void Captures_parameters_correctly()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyClass
{
   [UseDelegateFromConstructor]
   public partial void ProcessData(int count, string name);
}
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location],
         expectedCompilerErrors: ["Partial method 'MyClass.ProcessData(int, string)' must have an implementation part because it has accessibility modifiers."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().HaveCount(1);
      result[0].Parameters.Should().HaveCount(2);
      result[0].Parameters[0].Name.Should().Be("count");
      result[0].Parameters[0].Type.Should().Be("int");
      result[0].Parameters[1].Name.Should().Be("name");
      result[0].Parameters[1].Type.Should().Be("string");
   }

   [Fact]
   public void Captures_ref_parameters_correctly()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyClass
{
   [UseDelegateFromConstructor]
   public partial void ModifyValue(ref int value);
}
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location],
         expectedCompilerErrors: ["Partial method 'MyClass.ModifyValue(ref int)' must have an implementation part because it has accessibility modifiers."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().HaveCount(1);
      result[0].Parameters.Should().HaveCount(1);
      result[0].Parameters[0].RefKind.Should().Be(RefKind.Ref);
   }

   [Fact]
   public void Returns_multiple_delegate_methods()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyClass
{
   [UseDelegateFromConstructor]
   public partial void Method1();

   [UseDelegateFromConstructor]
   public partial int Method2();

   [UseDelegateFromConstructor]
   public partial string Method3(int x);
}
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location],
         expectedCompilerErrors:
         [
            "Partial method 'MyClass.Method1()' must have an implementation part because it has accessibility modifiers.",
            "Partial method 'MyClass.Method2()' must have an implementation part because it has accessibility modifiers.",
            "Partial method 'MyClass.Method3(int)' must have an implementation part because it has accessibility modifiers."
         ]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().HaveCount(3);
      result.Select(m => m.MethodName).Should().BeEquivalentTo("Method1", "Method2", "Method3");
   }

   [Fact]
   public void Returns_empty_for_methods_without_parameters()
   {
      var src = @"
using Thinktecture;

namespace Test;

public partial class MyClass
{
   [UseDelegateFromConstructor]
   public partial void NoParams();
}
";
      var compilation = CreateCompilation(
         src,
         additionalReferences: [typeof(UseDelegateFromConstructorAttribute).Assembly.Location],
         expectedCompilerErrors: ["Partial method 'MyClass.NoParams()' must have an implementation part because it has accessibility modifiers."]);
      var type = GetTypeSymbol(compilation, "Test.MyClass");

      var result = type.GetDelegateMethods();

      result.Should().HaveCount(1);
      result[0].Parameters.Should().BeEmpty();
   }
}
