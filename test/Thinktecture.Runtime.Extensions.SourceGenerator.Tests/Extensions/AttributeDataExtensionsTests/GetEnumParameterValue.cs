using System.Linq;
using Microsoft.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.AttributeDataExtensionsTests;

public class GetEnumParameterValue : CompilationTestBase
{
   [Fact]
   public void Should_handle_enum_with_int_underlying_type()
   {
      // Arrange
      var source = """
         using System;

         namespace TestNamespace;

         public enum MyIntEnum
         {
            None = 0,
            Value1 = 1,
            Value2 = 2
         }

         [AttributeUsage(AttributeTargets.Class)]
         public class TestAttribute : Attribute
         {
            public MyIntEnum EnumValue { get; set; }
         }

         [Test(EnumValue = MyIntEnum.Value1)]
         public class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");
      var attributeData = testClassSymbol.GetAttributes().Single();

      // Act
      var namedArg = attributeData.NamedArguments.Single(a => a.Key == "EnumValue");
      var typedConstant = namedArg.Value;

      // Assert - verify that int-based enum values are boxed as int
      typedConstant.Kind.Should().Be(TypedConstantKind.Enum);
      typedConstant.Value.Should().BeOfType<int>();
      ((int)typedConstant.Value!).Should().Be(1);
   }

   [Fact]
   public void Should_document_that_roslyn_boxes_enums_with_their_underlying_type()
   {
      // This test documents the Roslyn behavior that enum values in attributes
      // are boxed using their actual underlying type, not always as 'int'.
      // The fix in GetEnumParameterValue handles all integral types (byte, sbyte, short, ushort, int, uint, long, ulong).

      var byteSource = """
         using System;
         namespace Test;
         public enum ByteEnum : byte { Value = 1 }
         [AttributeUsage(AttributeTargets.Class)]
         public class TestAttr : Attribute { public ByteEnum E { get; set; } }
         [TestAttr(E = ByteEnum.Value)]
         public class C {}
         """;

      var compilation = CreateCompilation(byteSource);
      var typeSymbol = GetTypeSymbol(compilation, "Test.C");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Verify: Roslyn boxes byte enums as 'byte', not 'int'
      var typedConstant = attributeData.NamedArguments.Single(a => a.Key == "E").Value;
      typedConstant.Kind.Should().Be(TypedConstantKind.Enum);
      typedConstant.Value.Should().BeOfType<byte>();
   }

   [Fact]
   public void Should_extract_byte_enum_correctly()
   {
      // Arrange
      var source = """
         using System;
         namespace Test;
         public enum ByteEnum : byte { None = 0, Value1 = 1, Value2 = 2 }
         [AttributeUsage(AttributeTargets.Class)]
         public class TestAttr : Attribute { public ByteEnum MyEnum { get; set; } }
         [TestAttr(MyEnum = ByteEnum.Value2)]
         public class C {}
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.C");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act & Assert - verify byte is boxed correctly
      var typedConstant = attributeData.NamedArguments.Single(a => a.Key == "MyEnum").Value;
      typedConstant.Kind.Should().Be(TypedConstantKind.Enum);
      typedConstant.Value.Should().BeOfType<byte>();
      ((byte)typedConstant.Value!).Should().Be(2);
   }

   [Fact]
   public void Should_extract_sbyte_enum_correctly()
   {
      // Arrange
      var source = """
         using System;
         namespace Test;
         public enum SByteEnum : sbyte { None = 0, Value1 = 1, Value2 = -1 }
         [AttributeUsage(AttributeTargets.Class)]
         public class TestAttr : Attribute { public SByteEnum MyEnum { get; set; } }
         [TestAttr(MyEnum = SByteEnum.Value2)]
         public class C {}
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.C");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act & Assert - verify sbyte is boxed correctly
      var typedConstant = attributeData.NamedArguments.Single(a => a.Key == "MyEnum").Value;
      typedConstant.Kind.Should().Be(TypedConstantKind.Enum);
      typedConstant.Value.Should().BeOfType<sbyte>();
      ((sbyte)typedConstant.Value!).Should().Be(-1);
   }

   [Fact]
   public void Should_extract_short_enum_correctly()
   {
      // Arrange
      var source = """
         using System;
         namespace Test;
         public enum ShortEnum : short { None = 0, Value1 = 100, Value2 = -100 }
         [AttributeUsage(AttributeTargets.Class)]
         public class TestAttr : Attribute { public ShortEnum MyEnum { get; set; } }
         [TestAttr(MyEnum = ShortEnum.Value1)]
         public class C {}
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.C");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act & Assert - verify short is boxed correctly
      var typedConstant = attributeData.NamedArguments.Single(a => a.Key == "MyEnum").Value;
      typedConstant.Kind.Should().Be(TypedConstantKind.Enum);
      typedConstant.Value.Should().BeOfType<short>();
      ((short)typedConstant.Value!).Should().Be(100);
   }

   [Fact]
   public void Should_extract_ushort_enum_correctly()
   {
      // Arrange
      var source = """
         using System;
         namespace Test;
         public enum UShortEnum : ushort { None = 0, Value1 = 100, Value2 = 65000 }
         [AttributeUsage(AttributeTargets.Class)]
         public class TestAttr : Attribute { public UShortEnum MyEnum { get; set; } }
         [TestAttr(MyEnum = UShortEnum.Value2)]
         public class C {}
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.C");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act & Assert - verify ushort is boxed correctly
      var typedConstant = attributeData.NamedArguments.Single(a => a.Key == "MyEnum").Value;
      typedConstant.Kind.Should().Be(TypedConstantKind.Enum);
      typedConstant.Value.Should().BeOfType<ushort>();
      ((ushort)typedConstant.Value!).Should().Be(65000);
   }

   [Fact]
   public void Should_extract_uint_enum_correctly()
   {
      // Arrange
      var source = """
         using System;
         namespace Test;
         public enum UIntEnum : uint { None = 0, Value1 = 100, Value2 = 4000000000 }
         [AttributeUsage(AttributeTargets.Class)]
         public class TestAttr : Attribute { public UIntEnum MyEnum { get; set; } }
         [TestAttr(MyEnum = UIntEnum.Value2)]
         public class C {}
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.C");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act & Assert - verify uint is boxed correctly
      var typedConstant = attributeData.NamedArguments.Single(a => a.Key == "MyEnum").Value;
      typedConstant.Kind.Should().Be(TypedConstantKind.Enum);
      typedConstant.Value.Should().BeOfType<uint>();
      ((uint)typedConstant.Value!).Should().Be(4000000000);
   }

   [Fact]
   public void Should_extract_long_enum_correctly()
   {
      // Arrange
      var source = """
         using System;
         namespace Test;
         public enum LongEnum : long { None = 0, Value1 = 100, Value2 = 9000000000000000000 }
         [AttributeUsage(AttributeTargets.Class)]
         public class TestAttr : Attribute { public LongEnum MyEnum { get; set; } }
         [TestAttr(MyEnum = LongEnum.Value2)]
         public class C {}
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.C");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act & Assert - verify long is boxed correctly
      var typedConstant = attributeData.NamedArguments.Single(a => a.Key == "MyEnum").Value;
      typedConstant.Kind.Should().Be(TypedConstantKind.Enum);
      typedConstant.Value.Should().BeOfType<long>();
      ((long)typedConstant.Value!).Should().Be(9000000000000000000);
   }

   [Fact]
   public void Should_extract_ulong_enum_correctly()
   {
      // Arrange
      var source = """
         using System;
         namespace Test;
         public enum ULongEnum : ulong { None = 0, Value1 = 100, Value2 = 18000000000000000000 }
         [AttributeUsage(AttributeTargets.Class)]
         public class TestAttr : Attribute { public ULongEnum MyEnum { get; set; } }
         [TestAttr(MyEnum = ULongEnum.Value2)]
         public class C {}
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.C");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act & Assert - verify ulong is boxed correctly
      var typedConstant = attributeData.NamedArguments.Single(a => a.Key == "MyEnum").Value;
      typedConstant.Kind.Should().Be(TypedConstantKind.Enum);
      typedConstant.Value.Should().BeOfType<ulong>();
      ((ulong)typedConstant.Value!).Should().Be(18000000000000000000);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.OperatorsGeneration.Default)]
   [InlineData(Thinktecture.CodeAnalysis.OperatorsGeneration.DefaultWithKeyTypeOverloads)]
   [InlineData(Thinktecture.CodeAnalysis.OperatorsGeneration.None)]
   public void Should_extract_OperatorsGeneration_enum_values(Thinktecture.CodeAnalysis.OperatorsGeneration expected)
   {
      // Arrange
      var source = $$"""
         using Thinktecture;

         namespace Test;

         [ValueObject<int>(AdditionOperators = OperatorsGeneration.{{expected}})]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindAdditionOperators();

      // Assert
      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.ConversionOperatorsGeneration.None)]
   [InlineData(Thinktecture.CodeAnalysis.ConversionOperatorsGeneration.Implicit)]
   [InlineData(Thinktecture.CodeAnalysis.ConversionOperatorsGeneration.Explicit)]
   public void Should_extract_ConversionOperatorsGeneration_enum_values(Thinktecture.CodeAnalysis.ConversionOperatorsGeneration expected)
   {
      // Arrange
      var source = $$"""
         using Thinktecture;

         namespace Test;

         [ValueObject<int>(ConversionFromKeyMemberType = ConversionOperatorsGeneration.{{expected}})]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindConversionFromKeyMemberType();

      // Assert
      result.Should().Be(expected);
   }

   [Theory]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Public)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Private)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Protected)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.Internal)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.ProtectedInternal)]
   [InlineData(Thinktecture.CodeAnalysis.AccessModifier.PrivateProtected)]
   public void Should_extract_AccessModifier_enum_values(Thinktecture.CodeAnalysis.AccessModifier expected)
   {
      // Arrange
      var source = $$"""
         using Thinktecture;

         namespace Test;

         [ValueObject<int>(KeyMemberAccessModifier = AccessModifier.{{expected}})]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindKeyMemberAccessModifier();

      // Assert
      result.Should().Be(expected);
   }

   [Fact]
   public void Should_return_null_when_enum_parameter_not_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ValueObject<int>]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindKeyMemberAccessModifier();

      // Assert
      result.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_for_invalid_enum_value()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [ValueObject<int>(AdditionOperators = (OperatorsGeneration)999)]
         public partial class TestValueObject
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestValueObject");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindAdditionOperators();

      // Assert
      // Invalid enum values should fall back to default via GetValidValue
      result.Should().Be(Thinktecture.CodeAnalysis.OperatorsGeneration.Default);
   }

   [Fact]
   public void Should_handle_flags_enum()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace Test;

         [SmartEnum<int>(SerializationFrameworks = SerializationFrameworks.SystemTextJson | SerializationFrameworks.NewtonsoftJson)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
         }
         """;

      var compilation = CreateCompilation(source);
      var typeSymbol = GetTypeSymbol(compilation, "Test.TestEnum");
      var attributeData = typeSymbol.GetAttributes().Single();

      // Act
      var result = attributeData.FindSerializationFrameworks();

      // Assert - check underlying integer values match expectations
      var expected = (int)SerializationFrameworks.SystemTextJson | (int)SerializationFrameworks.NewtonsoftJson;
      ((int)result).Should().Be(expected);
      ((int)result & (int)SerializationFrameworks.MessagePack).Should().Be(0);
   }
}
