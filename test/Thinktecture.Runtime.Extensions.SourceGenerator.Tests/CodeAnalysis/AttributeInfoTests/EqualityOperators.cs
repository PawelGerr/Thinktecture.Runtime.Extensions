using System.Text.Json.Serialization;
using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.AttributeInfoTests;

public class EqualityOperators : CompilationTestBase
{
   [Fact]
   public void Operator_Equals_Should_return_true_for_equal_instances()
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
      var result = info1 == info2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Operator_Equals_Should_return_false_for_different_instances()
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
      var result = info1 == info2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Operator_NotEquals_Should_return_false_for_equal_instances()
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
      var result = info1 != info2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Operator_NotEquals_Should_return_true_for_different_instances()
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
      var result = info1 != info2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Operator_Equals_Should_return_true_for_instances_with_all_same_properties()
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
      var result = info1 == info2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Operator_NotEquals_Should_return_false_for_instances_with_all_same_properties()
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
      var result = info1 != info2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Operator_Equals_Should_work_with_default_instances()
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
      var result = info1 == info2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Operator_NotEquals_Should_work_with_default_instances()
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
      var result = info1 != info2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Operator_Equals_Should_return_false_when_HasJsonConverterAttribute_differs()
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

      var compilation = CreateCompilation(sourceWithAttribute + "\n" + sourceWithoutAttribute, additionalReferences: [typeof(JsonConverterAttribute).Assembly.Location]);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var result = info1 == info2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Operator_NotEquals_Should_return_true_when_ObjectFactories_differ()
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
      var result = info1 != info2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Operator_Equals_Should_return_false_when_ValidationError_differs()
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
      var result = info1 == info2;

      // Assert
      result.Should().BeFalse();
   }

   [Fact]
   public void Operator_NotEquals_Should_return_true_when_KeyMemberComparerAccessor_differs()
   {
      // Arrange
      var sourceWithComparer = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberComparer<MyComparer, int>]
            [ValueObject<int>]
            public partial class TestClass1
            {
            }

            public class MyComparer : Thinktecture.IComparerAccessor<int>
            {
               public static System.Collections.Generic.IComparer<int> Comparer => System.Collections.Generic.Comparer<int>.Default;
            }
         }
         """;

      var sourceWithoutComparer = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ValueObject<int>]
            public partial class TestClass2
            {
            }
         }
         """;

      var compilation = CreateCompilation(sourceWithComparer + "\n" + sourceWithoutComparer);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var result = info1 != info2;

      // Assert
      result.Should().BeTrue();
   }

   [Fact]
   public void Operator_Equals_Should_return_false_when_KeyMemberEqualityComparerAccessor_differs()
   {
      // Arrange
      var sourceWithComparer = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberEqualityComparer<MyEqualityComparer, int>]
            [ValueObject<int>]
            public partial class TestClass1
            {
            }

            public class MyEqualityComparer : Thinktecture.IEqualityComparerAccessor<int>
            {
               public static System.Collections.Generic.IEqualityComparer<int> EqualityComparer => System.Collections.Generic.EqualityComparer<int>.Default;
            }
         }
         """;

      var sourceWithoutComparer = """
         namespace TestNamespace
         {
            using Thinktecture;

            [ValueObject<int>]
            public partial class TestClass2
            {
            }
         }
         """;

      var compilation = CreateCompilation(sourceWithComparer + "\n" + sourceWithoutComparer);
      var testClass1Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass1");
      var testClass2Symbol = GetTypeSymbol(compilation, "TestNamespace.TestClass2");

      AttributeInfo.TryCreate(testClass1Symbol, out var info1);
      AttributeInfo.TryCreate(testClass2Symbol, out var info2);

      // Act
      var result = info1 == info2;

      // Assert
      result.Should().BeFalse();
   }
}
