using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.AttributeInfoTests;

public class Equals : CompilationTestBase
{
   [Fact]
   public void Should_return_true_when_both_instances_have_default_values()
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

      AttributeInfo.TryCreate(testClassSymbol, out var info1);
      AttributeInfo.TryCreate(testClassSymbol, out var info2);

      // Act
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_true_when_both_instances_have_same_HasStructLayoutAttribute()
   {
      // Arrange
      var source = """
         namespace TestNamespace
         {
            using System.Runtime.InteropServices;

            [StructLayout(LayoutKind.Sequential)]
            public class TestClass
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      AttributeInfo.TryCreate(testClassSymbol, out var info1);
      AttributeInfo.TryCreate(testClassSymbol, out var info2);

      // Act
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_HasStructLayoutAttribute_differs()
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
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_HasJsonConverterAttribute_differs()
   {
      // Arrange
      var sourceWithAttribute = """
         namespace TestNamespace
         {
            using System.Text.Json.Serialization;

            [JsonConverter(typeof(TestConverter))]
            public class TestClass1
            {
            }

            public class TestConverter : JsonConverter<TestClass1>
            {
               public override TestClass1 Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
                  => throw new System.NotImplementedException();

               public override void Write(System.Text.Json.Utf8JsonWriter writer, TestClass1 value, System.Text.Json.JsonSerializerOptions options)
                  => throw new System.NotImplementedException();
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

      var compilation = CreateCompilation(sourceWithAttribute + "\n" + sourceWithoutAttribute,
                                          additionalReferences: [typeof(System.Text.Json.Serialization.JsonConverterAttribute).Assembly.Location]);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_HasNewtonsoftJsonConverterAttribute_differs()
   {
      // Arrange
      var sourceWithAttribute = """
         namespace TestNamespace
         {
            using Newtonsoft.Json;

            [JsonConverter(typeof(TestConverter))]
            public class TestClass1
            {
            }

            public class TestConverter : JsonConverter
            {
               public override bool CanConvert(System.Type objectType) => throw new System.NotImplementedException();
               public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) => throw new System.NotImplementedException();
               public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new System.NotImplementedException();
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

      var compilation = CreateCompilation(sourceWithAttribute + "\n" + sourceWithoutAttribute, additionalReferences: [typeof(Newtonsoft.Json.JsonConverterAttribute).Assembly.Location]);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_HasMessagePackFormatterAttribute_differs()
   {
      // Arrange
      var sourceWithAttribute = """
         namespace TestNamespace
         {
            using MessagePack;
            using MessagePack.Formatters;

            [MessagePackFormatter(typeof(TestFormatter))]
            public class TestClass1
            {
            }

            public class TestFormatter : IMessagePackFormatter<TestClass1>
            {
               public void Serialize(ref MessagePackWriter writer, TestClass1 value, MessagePackSerializerOptions options)
                  => throw new System.NotImplementedException();

               public TestClass1 Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
                  => throw new System.NotImplementedException();
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

      var compilation = CreateCompilation(sourceWithAttribute + "\n" + sourceWithoutAttribute,
                                          additionalReferences:
                                          [
                                             typeof(MessagePack.Formatters.IMessagePackFormatter).Assembly.Location,
                                             typeof(MessagePack.MessagePackFormatterAttribute).Assembly.Location
                                          ]);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_ObjectFactories_differ_in_count()
   {
      // Arrange
      var sourceWithOneFactory = """
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

      var sourceWithTwoFactories = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ObjectFactory<string>]
            [ObjectFactory<int>]
            [ValueObject<decimal>]
            public partial class TestClass2
            {
               private TestClass2(string value) { }
               private TestClass2(int value) { }
            }
         }
         """;

      var compilation = CreateCompilation(sourceWithOneFactory + "\n" + sourceWithTwoFactories);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_ObjectFactories_differ_in_content()
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
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_ObjectFactories_have_same_content()
   {
      // Arrange
      var source = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ObjectFactory<string>]
            [ValueObject<int>]
            public partial class TestClass
            {
               private TestClass(string value) { }
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      AttributeInfo.TryCreate(testClassSymbol, out var info1);
      AttributeInfo.TryCreate(testClassSymbol, out var info2);

      // Act
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_ValidationError_differs()
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
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_ValidationError_is_same()
   {
      // Arrange
      var source = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ValidationError<MyValidationError>]
            [ValueObject<int>]
            public partial class TestClass
            {
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
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_return_false_when_KeyMemberComparerAccessor_differs()
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
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_KeyMemberEqualityComparerAccessor_differs()
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
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_comparing_with_null_object()
   {
      // Arrange
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
      var result = info.Equals(null);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_false_when_comparing_with_different_type()
   {
      // Arrange
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
      var result = info.Equals("not an AttributeInfo");

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_return_true_when_all_properties_match()
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
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Should_handle_null_KeyMemberComparerAccessor_in_equality()
   {
      // Arrange
      var sourceWithoutComparer = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ValueObject<int>]
            public partial class TestClass1
            {
            }
         }
         """;

      var sourceWithComparer = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberComparer<MyComparer, int>]
            [ValueObject<int>]
            public partial class TestClass2
            {
            }

            public class MyComparer : Thinktecture.IComparerAccessor<int>
            {
               public static System.Collections.Generic.IComparer<int> Comparer => System.Collections.Generic.Comparer<int>.Default;
            }
         }
         """;

      var compilation = CreateCompilation(sourceWithoutComparer + "\n" + sourceWithComparer);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Should_handle_null_KeyMemberEqualityComparerAccessor_in_equality()
   {
      // Arrange
      var sourceWithoutComparer = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ValueObject<int>]
            public partial class TestClass1
            {
            }
         }
         """;

      var sourceWithComparer = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberEqualityComparer<MyEqualityComparer, int>]
            [ValueObject<int>]
            public partial class TestClass2
            {
            }

            public class MyEqualityComparer : Thinktecture.IEqualityComparerAccessor<int>
            {
               public static System.Collections.Generic.IEqualityComparer<int> EqualityComparer => System.Collections.Generic.EqualityComparer<int>.Default;
            }
         }
         """;

      var compilation = CreateCompilation(sourceWithoutComparer + "\n" + sourceWithComparer);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var result = info1.Equals(info2);

      // Assert
      result.Should().BeFalse();
   }
}
