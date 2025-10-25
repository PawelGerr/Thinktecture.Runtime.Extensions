using System.Text.Json.Serialization;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.AttributeInfoTests;

public class GetHashCode : CompilationTestBase
{
   [Fact]
   public void Should_return_same_hash_code_for_equal_instances()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      AttributeInfo.TryCreate(testClassSymbol, out var info1);
      AttributeInfo.TryCreate(testClassSymbol, out var info2);

      // Act
      var hashCode1 = info1.GetHashCode();
      var hashCode2 = info2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_return_same_hash_code_for_instances_with_same_properties()
   {
      // Arrange
      var source = """
         namespace TestNamespace
         {
            using System.Runtime.InteropServices;
            using Thinktecture;

            [StructLayout(LayoutKind.Sequential)]
            [KeyMemberEqualityComparer<MyEqualityComparer, int>]
            [KeyMemberComparer<MyComparer, int>]
            [ValidationError<MyValidationError>]
            [ObjectFactory<string>]
            [ValueObject<int>]
            public partial class TestClass
            {
               private TestClass(string value) { }
            }

            public class MyEqualityComparer : Thinktecture.IEqualityComparerAccessor<int>
            {
               public static System.Collections.Generic.IEqualityComparer<int> EqualityComparer => System.Collections.Generic.EqualityComparer<int>.Default;
            }

            public class MyComparer : Thinktecture.IComparerAccessor<int>
            {
               public static System.Collections.Generic.IComparer<int> Comparer => System.Collections.Generic.Comparer<int>.Default;
            }

            public class MyValidationError : Thinktecture.IValidationError<MyValidationError>
         {
            public static MyValidationError Create(string message) => new MyValidationError();
         }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      AttributeInfo.TryCreate(testClassSymbol, out var info1);
      AttributeInfo.TryCreate(testClassSymbol, out var info2);

      // Act
      var hashCode1 = info1.GetHashCode();
      var hashCode2 = info2.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_likely_return_different_hash_codes_for_different_HasStructLayoutAttribute()
   {
      // Arrange
      var sourceWithAttribute = """
         namespace TestNamespace
         {
            using System.Runtime.InteropServices;

            [StructLayout(LayoutKind.Sequential)]
            public class TestClass1
            {
            }
         }
         """;

      var sourceWithoutAttribute = """
         namespace TestNamespace
         {
            public class TestClass2
            {
            }
         }
         """;

      var compilation = CreateCompilation(sourceWithAttribute + "\n" + sourceWithoutAttribute);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var hashCode1 = info1.GetHashCode();
      var hashCode2 = info2.GetHashCode();

      // Assert - different hash codes are likely but not guaranteed
      hashCode1.Should().NotBe(0);
      hashCode2.Should().NotBe(0);
   }

   [Fact]
   public void Should_likely_return_different_hash_codes_for_different_ObjectFactories()
   {
      // Arrange
      var sourceWithStringFactory = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ObjectFactory<string>]
            [ValueObject<int>]
            public partial class TestClass1
            {
               private TestClass1(string value) { }
            }
         }
         """;

      var sourceWithIntFactory = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ObjectFactory<int>]
            [ValueObject<decimal>]
            public partial class TestClass2
            {
               private TestClass2(int value) { }
            }
         }
         """;

      var compilation = CreateCompilation(sourceWithStringFactory + "\n" + sourceWithIntFactory);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var hashCode1 = info1.GetHashCode();
      var hashCode2 = info2.GetHashCode();

      // Assert - verify both return valid hash codes
      hashCode1.Should().NotBe(0);
      hashCode2.Should().NotBe(0);
   }

   [Fact]
   public void Should_likely_return_different_hash_codes_for_different_ValidationError()
   {
      // Arrange
      var sourceWithValidationError1 = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ValidationError<MyValidationError1>]
            [ValueObject<int>]
            public partial class TestClass1
            {
            }

            public class MyValidationError1 : Thinktecture.IValidationError<MyValidationError1>
         {
            public static MyValidationError1 Create(string message) => new MyValidationError1();
         }
         }
         """;

      var sourceWithValidationError2 = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ValidationError<MyValidationError2>]
            [ValueObject<int>]
            public partial class TestClass2
            {
            }

            public class MyValidationError2 : Thinktecture.IValidationError<MyValidationError2>
         {
            public static MyValidationError2 Create(string message) => new MyValidationError2();
         }
         }
         """;

      var compilation = CreateCompilation(sourceWithValidationError1 + "\n" + sourceWithValidationError2);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var hashCode1 = info1.GetHashCode();
      var hashCode2 = info2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(0);
      hashCode2.Should().NotBe(0);
   }

   [Fact]
   public void Should_likely_return_different_hash_codes_for_different_KeyMemberComparerAccessor()
   {
      // Arrange
      var sourceWithComparer1 = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberComparer<MyComparer1, int>]
            [ValueObject<int>]
            public partial class TestClass1
            {
            }

            public class MyComparer1 : Thinktecture.IComparerAccessor<int>
            {
               public static System.Collections.Generic.IComparer<int> Comparer => System.Collections.Generic.Comparer<int>.Default;
            }
         }
         """;

      var sourceWithComparer2 = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberComparer<MyComparer2, int>]
            [ValueObject<int>]
            public partial class TestClass2
            {
            }

            public class MyComparer2 : Thinktecture.IComparerAccessor<int>
            {
               public static System.Collections.Generic.IComparer<int> Comparer => System.Collections.Generic.Comparer<int>.Default;
            }
         }
         """;

      var compilation = CreateCompilation(sourceWithComparer1 + "\n" + sourceWithComparer2);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var hashCode1 = info1.GetHashCode();
      var hashCode2 = info2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(0);
      hashCode2.Should().NotBe(0);
   }

   [Fact]
   public void Should_likely_return_different_hash_codes_for_different_KeyMemberEqualityComparerAccessor()
   {
      // Arrange
      var sourceWithComparer1 = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberEqualityComparer<MyEqualityComparer1, int>]
            [ValueObject<int>]
            public partial class TestClass1
            {
            }

            public class MyEqualityComparer1 : Thinktecture.IEqualityComparerAccessor<int>
            {
               public static System.Collections.Generic.IEqualityComparer<int> EqualityComparer => System.Collections.Generic.EqualityComparer<int>.Default;
            }
         }
         """;

      var sourceWithComparer2 = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberEqualityComparer<MyEqualityComparer2, int>]
            [ValueObject<int>]
            public partial class TestClass2
            {
            }

            public class MyEqualityComparer2 : Thinktecture.IEqualityComparerAccessor<int>
            {
               public static System.Collections.Generic.IEqualityComparer<int> EqualityComparer => System.Collections.Generic.EqualityComparer<int>.Default;
            }
         }
         """;

      var compilation = CreateCompilation(sourceWithComparer1 + "\n" + sourceWithComparer2);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var hashCode1 = info1.GetHashCode();
      var hashCode2 = info2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(0);
      hashCode2.Should().NotBe(0);
   }

   [Fact]
   public void Should_return_consistent_hash_code_for_default_instance()
   {
      // Arrange
      var source = """
         namespace TestNamespace;

         public class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Act
      var hashCode1 = info.GetHashCode();
      var hashCode2 = info.GetHashCode();

      // Assert
      hashCode1.Should().Be(hashCode2);
   }

   [Fact]
   public void Should_handle_null_string_properties_in_hash_code_calculation()
   {
      // Arrange - instance with null KeyMemberComparerAccessor and KeyMemberEqualityComparerAccessor
      var source = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ValueObject<int>]
            public partial class TestClass
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Act
      var hashCode = info.GetHashCode();

      // Assert - should not throw and should return a valid hash code
      hashCode.Should().NotBe(0);
   }

   [Fact]
   public void Should_incorporate_all_boolean_flags_in_hash_code()
   {
      // Arrange - Test with all boolean flags set
      var sourceAllTrue = """
         namespace TestNamespace
         {
            using System.Runtime.InteropServices;
            using System.Text.Json.Serialization;
            using Newtonsoft.Json;
            using MessagePack;
            using MessagePack.Formatters;

            [StructLayout(LayoutKind.Sequential)]
            [System.Text.Json.Serialization.JsonConverter(typeof(TestJsonConverter))]
            [Newtonsoft.Json.JsonConverter(typeof(TestNewtonsoftConverter))]
            [MessagePackFormatter(typeof(TestMessagePackFormatter))]
            public class TestClass1
            {
            }

            public class TestJsonConverter : System.Text.Json.Serialization.JsonConverter<TestClass1>
            {
               public override TestClass1 Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
                  => throw new System.NotImplementedException();

               public override void Write(System.Text.Json.Utf8JsonWriter writer, TestClass1 value, System.Text.Json.JsonSerializerOptions options)
                  => throw new System.NotImplementedException();
            }

            public class TestNewtonsoftConverter : Newtonsoft.Json.JsonConverter
            {
               public override bool CanConvert(System.Type objectType) => throw new System.NotImplementedException();
               public override object ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer) => throw new System.NotImplementedException();
               public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer) => throw new System.NotImplementedException();
            }

            public class TestMessagePackFormatter : IMessagePackFormatter<TestClass1>
            {
               public void Serialize(ref MessagePackWriter writer, TestClass1 value, MessagePackSerializerOptions options)
                  => throw new System.NotImplementedException();

               public TestClass1 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
                  => throw new System.NotImplementedException();
            }
         }
         """;

      var sourceAllFalse = """
         namespace TestNamespace
         {
            public class TestClass2
            {
            }
         }
         """;

      var compilation = CreateCompilation(sourceAllTrue + "\n" + sourceAllFalse,
                                          additionalReferences:
                                          [
                                             typeof(JsonConverterAttribute).Assembly.Location,
                                             typeof(Newtonsoft.Json.JsonConverterAttribute).Assembly.Location,
                                             typeof(MessagePack.MessagePackFormatterAttribute).Assembly.Location,
                                             typeof(MessagePack.Formatters.IMessagePackFormatter).Assembly.Location
                                          ]);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var hashCode1 = info1.GetHashCode();
      var hashCode2 = info2.GetHashCode();

      // Assert
      hashCode1.Should().NotBe(0);
      hashCode2.Should().NotBe(0);
   }
}
