using Microsoft.CodeAnalysis;

#nullable enable

namespace Thinktecture.Runtime.Tests.TypeSymbolExtensionsTests;

public class IsNullOrDotnetBaseType : CompilationTestBase
{
   [Fact]
   public void Returns_true_for_null()
   {
      ITypeSymbol? type = null;

      var result = type.IsNullOrDotnetBaseType();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_System_Object()
   {
      var compilation = CreateCompilation("");
      var objectType = compilation.GetSpecialType(SpecialType.System_Object);

      var result = objectType.IsNullOrDotnetBaseType();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_value_type_like_Int32()
   {
      var compilation = CreateCompilation("");
      var intType = compilation.GetSpecialType(SpecialType.System_Int32);

      var result = intType.IsNullOrDotnetBaseType();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_user_defined_reference_type()
   {
      var src = @"
namespace Test;

public class MyClass;
";
      var compilation = CreateCompilation(src);
      var myClass = GetTypeSymbol(compilation, "Test.MyClass");

      var result = myClass.IsNullOrDotnetBaseType();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_true_for_System_ValueType()
   {
      var compilation = CreateCompilation("");
      var valueType = compilation.GetSpecialType(SpecialType.System_ValueType);

      var result = valueType.IsNullOrDotnetBaseType();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_System_Enum()
   {
      var compilation = CreateCompilation("");
      var enumType = compilation.GetSpecialType(SpecialType.System_Enum);

      var result = enumType.IsNullOrDotnetBaseType();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_true_for_System_MulticastDelegate()
   {
      var compilation = CreateCompilation("");
      var delegateType = compilation.GetSpecialType(SpecialType.System_MulticastDelegate);

      var result = delegateType.IsNullOrDotnetBaseType();

      result.Should().BeTrue();
   }

   [Fact]
   public void Returns_false_for_user_defined_struct()
   {
      var src = @"
namespace Test;

public struct MyStruct
{
   public int Value { get; set; }
}
";
      var compilation = CreateCompilation(src);
      var myStruct = GetTypeSymbol(compilation, "Test.MyStruct");

      var result = myStruct.IsNullOrDotnetBaseType();

      result.Should().BeFalse();
   }

   [Fact]
   public void Returns_false_for_user_defined_enum()
   {
      var src = @"
namespace Test;

public enum MyEnum
{
   Value1,
   Value2
}
";
      var compilation = CreateCompilation(src);
      var myEnum = GetTypeSymbol(compilation, "Test.MyEnum");

      var result = myEnum.IsNullOrDotnetBaseType();

      result.Should().BeFalse();
   }

}
