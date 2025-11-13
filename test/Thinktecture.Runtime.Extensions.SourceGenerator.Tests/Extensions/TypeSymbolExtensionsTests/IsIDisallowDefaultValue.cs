using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsIDisallowDefaultValue : CompilationTestBase
{
   [Fact]
   public void Should_return_true_for_IDisallowDefaultValue_interface()
   {
      var src = "";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Thinktecture.IDisallowDefaultValue");

      var result = interfaceType.IsIDisallowDefaultValue();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_different_interface_with_same_name_in_different_namespace()
   {
      var src = @"
namespace Test;

public interface IDisallowDefaultValue;
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Test.IDisallowDefaultValue");

      var result = interfaceType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_different_interface_with_different_name()
   {
      var src = @"
namespace Thinktecture;

public interface IMyInterface;
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Thinktecture.IMyInterface");

      var result = interfaceType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_class_type()
   {
      var src = @"
namespace Thinktecture;

public class IDisallowDefaultValue;
";
      var compilation = CreateCompilation(src);
      var classType = GetTypeSymbol(compilation, "Thinktecture.IDisallowDefaultValue");

      var result = classType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_struct_type()
   {
      var src = @"
namespace Thinktecture;

public struct IDisallowDefaultValue;
";
      var compilation = CreateCompilation(src);
      var structType = GetTypeSymbol(compilation, "Thinktecture.IDisallowDefaultValue");

      var result = structType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_enum_type()
   {
      var src = @"
namespace Thinktecture;

public enum IDisallowDefaultValue
{
   Value1,
   Value2
}
";
      var compilation = CreateCompilation(src);
      var enumType = GetTypeSymbol(compilation, "Thinktecture.IDisallowDefaultValue");

      var result = enumType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_null_type()
   {
      ITypeSymbol? type = null;

      var result = type.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_interface_in_nested_namespace()
   {
      var src = @"
namespace Thinktecture.Nested;

public interface IDisallowDefaultValue;
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Thinktecture.Nested.IDisallowDefaultValue");

      var result = interfaceType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_nested_interface_with_correct_name()
   {
      var src = @"
namespace Thinktecture;

public class OuterClass
{
   public interface IDisallowDefaultValue;
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Thinktecture.OuterClass+IDisallowDefaultValue");

      var result = interfaceType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_generic_interface_with_same_name()
   {
      var src = @"
namespace Thinktecture;

public interface IDisallowDefaultValue<T>;
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Thinktecture.IDisallowDefaultValue`1");

      var result = interfaceType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_error_type()
   {
      var src = @"
namespace Test;

public class MyClass : NonExistentType;
";
      var compilation = CreateCompilation(src, expectedCompilerErrors: ["The type or namespace name 'NonExistentType' could not be found (are you missing a using directive or an assembly reference?)"]);
      var myClass = GetTypeSymbol(compilation, "Test.MyClass");
      var errorType = myClass.BaseType; // This will be an error type

      var result = errorType?.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_derived_interface()
   {
      var src = @"
namespace Thinktecture;

public interface IDisallowDefaultValue;

public interface IDerived : IDisallowDefaultValue;
";
      var compilation = CreateCompilation(src);
      var derivedInterface = GetTypeSymbol(compilation, "Thinktecture.IDerived");

      var result = derivedInterface.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_interface_is_in_correct_namespace_with_correct_name()
   {
      var src = @"
namespace Thinktecture;

public interface IDisallowDefaultValue
{
   void SomeMethod();
   int SomeProperty { get; }
}
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Thinktecture.IDisallowDefaultValue");

      var result = interfaceType.IsIDisallowDefaultValue();

      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_for_record_type()
   {
      var src = @"
namespace Thinktecture;

public record IDisallowDefaultValue(string Name);
";
      var compilation = CreateCompilation(src);
      var recordType = GetTypeSymbol(compilation, "Thinktecture.IDisallowDefaultValue");

      var result = recordType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_delegate_type()
   {
      var src = @"
namespace Thinktecture;

public delegate void IDisallowDefaultValue();
";
      var compilation = CreateCompilation(src);
      var delegateType = GetTypeSymbol(compilation, "Thinktecture.IDisallowDefaultValue");

      var result = delegateType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_interface_with_similar_name()
   {
      var src = @"
namespace Thinktecture;

public interface IDisallowDefaultValueExtended;
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Thinktecture.IDisallowDefaultValueExtended");

      var result = interfaceType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_for_interface_with_prefix()
   {
      var src = @"
namespace Thinktecture;

public interface MyIDisallowDefaultValue;
";
      var compilation = CreateCompilation(src);
      var interfaceType = GetTypeSymbol(compilation, "Thinktecture.MyIDisallowDefaultValue");

      var result = interfaceType.IsIDisallowDefaultValue();

      result.Should().BeFalse();
   }
}
