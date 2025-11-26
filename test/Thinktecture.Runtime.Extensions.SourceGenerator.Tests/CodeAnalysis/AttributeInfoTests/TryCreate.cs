using Thinktecture.CodeAnalysis;

namespace Thinktecture.Runtime.Tests.CodeAnalysis.AttributeInfoTests;

public class TryCreate : CompilationTestBase
{
   [Fact]
   public void Should_return_null_and_default_info_when_type_has_no_attributes()
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

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.HasStructLayoutAttribute.Should().BeFalse();
      info.HasJsonConverterAttribute.Should().BeFalse();
      info.HasNewtonsoftJsonConverterAttribute.Should().BeFalse();
      info.HasMessagePackFormatterAttribute.Should().BeFalse();
      info.ObjectFactories.Should().BeEmpty();
      info.ValidationError.Should().Be(ValidationErrorState.Default);
      info.KeyMemberComparerAccessor.Should().BeNull();
      info.KeyMemberEqualityComparerAccessor.Should().BeNull();
   }

   [Fact]
   public void Should_detect_StructLayoutAttribute()
   {
      // Arrange
      var source = """
         using System.Runtime.InteropServices;

         namespace TestNamespace;

         [StructLayout(LayoutKind.Sequential)]
         public class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.HasStructLayoutAttribute.Should().BeTrue();
   }

   [Fact]
   public void Should_detect_JsonConverterAttribute()
   {
      // Arrange
      var source = """
         using System.Text.Json.Serialization;

         namespace TestNamespace;

         [JsonConverter(typeof(TestConverter))]
         public class TestClass
         {
         }

         public class TestConverter : JsonConverter<TestClass>
         {
            public override TestClass Read(ref System.Text.Json.Utf8JsonReader reader, System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
               => throw new System.NotImplementedException();

            public override void Write(System.Text.Json.Utf8JsonWriter writer, TestClass value, System.Text.Json.JsonSerializerOptions options)
               => throw new System.NotImplementedException();
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(System.Text.Json.Serialization.JsonConstructorAttribute).Assembly.Location]);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.HasJsonConverterAttribute.Should().BeTrue();
   }

   [Fact]
   public void Should_detect_NewtonsoftJsonConverterAttribute()
   {
      // Arrange
      var source = """
         using Newtonsoft.Json;

         namespace TestNamespace;

         [JsonConverter(typeof(TestConverter))]
         public class TestClass
         {
         }

         public class TestConverter : JsonConverter
         {
            public override bool CanConvert(System.Type objectType) => throw new System.NotImplementedException();
            public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer) => throw new System.NotImplementedException();
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new System.NotImplementedException();
         }
         """;

      var compilation = CreateCompilation(source, additionalReferences: [typeof(Newtonsoft.Json.JsonConverterAttribute).Assembly.Location]);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.HasNewtonsoftJsonConverterAttribute.Should().BeTrue();
   }

   [Fact]
   public void Should_detect_MessagePackFormatterAttribute()
   {
      // Arrange
      var source = """
         using MessagePack;
         using MessagePack.Formatters;

         namespace TestNamespace;

         [MessagePackFormatter(typeof(TestFormatter))]
         public class TestClass
         {
         }

         public class TestFormatter : IMessagePackFormatter<TestClass>
         {
            public void Serialize(ref MessagePackWriter writer, TestClass value, MessagePackSerializerOptions options)
               => throw new System.NotImplementedException();

            public TestClass Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
               => throw new System.NotImplementedException();
         }
         """;

      var compilation = CreateCompilation(source,
                                          additionalReferences:
                                          [
                                             typeof(MessagePack.MessagePackFormatterAttribute).Assembly.Location,
                                             typeof(MessagePack.Formatters.IMessagePackFormatter).Assembly.Location
                                          ]);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.HasMessagePackFormatterAttribute.Should().BeTrue();
   }

   [Fact]
   public void Should_detect_ValidationErrorAttribute()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValidationError<MyValidationError>]
         [ValueObject<int>]
         public partial class TestClass
         {
         }

         public class MyValidationError : Thinktecture.IValidationError<MyValidationError>
         {
            public static MyValidationError Create(string message) => new MyValidationError();
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.ValidationError.TypeFullyQualified.Should().Be("global::TestNamespace.MyValidationError");
   }

   [Fact]
   public void Should_detect_KeyMemberComparerAttribute()
   {
      // Arrange
      var source = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberComparer<MyComparer, int>]
            [ValueObject<int>]
            public partial class TestClass
            {
            }

            public class MyComparer : Thinktecture.IComparerAccessor<int>
            {
               public static System.Collections.Generic.IComparer<int> Comparer => System.Collections.Generic.Comparer<int>.Default;
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.KeyMemberComparerAccessor.Should().Be("global::TestNamespace.MyComparer");
   }

   [Fact]
   public void Should_detect_KeyMemberEqualityComparerAttribute()
   {
      // Arrange
      var source = """
         namespace TestNamespace
         {
            using Thinktecture;

            [KeyMemberEqualityComparer<MyEqualityComparer, int>]
            [ValueObject<int>]
            public partial class TestClass
            {
            }

            public class MyEqualityComparer : Thinktecture.IEqualityComparerAccessor<int>
            {
               public static System.Collections.Generic.IEqualityComparer<int> EqualityComparer => System.Collections.Generic.EqualityComparer<int>.Default;
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.KeyMemberEqualityComparerAccessor.Should().Be("global::TestNamespace.MyEqualityComparer");
   }

   [Fact]
   public void Should_add_ObjectFactory_to_list()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>]
         [ValueObject<int>]
         public partial class TestClass
         {
            private TestClass(string value)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.ObjectFactories.Should().HaveCount(1);
      info.ObjectFactories[0].TypeFullyQualified.Should().Be("string");
      info.ObjectFactories[0].UseForSerialization.Should().Be(Thinktecture.CodeAnalysis.SerializationFrameworks.None);
      info.ObjectFactories[0].UseWithEntityFramework.Should().BeFalse();
      info.ObjectFactories[0].UseForModelBinding.Should().BeFalse();
      info.ObjectFactories[0].HasCorrespondingConstructor.Should().BeFalse();
   }

   [Fact]
   public void Should_configure_ObjectFactory_with_UseForSerialization()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         [ValueObject<int>]
         public partial class TestClass
         {
            private TestClass(string value)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.ObjectFactories.Should().HaveCount(1);
      info.ObjectFactories[0].UseForSerialization.Should().Be(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson);
   }

   [Fact]
   public void Should_configure_ObjectFactory_with_UseWithEntityFramework()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(UseWithEntityFramework = true)]
         [ValueObject<int>]
         public partial class TestClass
         {
            private TestClass(string value)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.ObjectFactories.Should().HaveCount(1);
      info.ObjectFactories[0].UseWithEntityFramework.Should().BeTrue();
   }

   [Fact]
   public void Should_configure_ObjectFactory_with_UseForModelBinding()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(UseForModelBinding = true)]
         [ValueObject<int>]
         public partial class TestClass
         {
            private TestClass(string value)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.ObjectFactories.Should().HaveCount(1);
      info.ObjectFactories[0].UseForModelBinding.Should().BeTrue();
   }

   [Fact]
   public void Should_configure_ObjectFactory_with_HasCorrespondingConstructor()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(HasCorrespondingConstructor = true)]
         [ValueObject<int>]
         public partial class TestClass
         {
            private TestClass(string value)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.ObjectFactories.Should().HaveCount(1);
      info.ObjectFactories[0].HasCorrespondingConstructor.Should().BeTrue();
   }

   [Fact]
   public void Should_replace_duplicate_ObjectFactory_with_same_type()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         [ValueObject<int>]
         public partial class TestClass
         {
            private TestClass(string value)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.ObjectFactories.Should().HaveCount(1);
      // The last one should win
      info.ObjectFactories[0].UseForSerialization.Should().Be(Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack);
   }

   [Fact]
   public void Should_keep_multiple_ObjectFactories_with_different_types()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>]
         [ObjectFactory<int>]
         [ValueObject<decimal>]
         public partial class TestClass
         {
            private TestClass(string value)
            {
            }

            private TestClass(int value)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.ObjectFactories.Should().HaveCount(2);
      info.ObjectFactories.Should().Contain(f => f.TypeFullyQualified == "string");
      info.ObjectFactories.Should().Contain(f => f.TypeFullyQualified == "int");
   }

   [Fact]
   public void Should_detect_single_ThinktectureComponentAttribute()
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

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out _, out var componentAttribute);

      // Assert
      error.Should().BeNull();
      componentAttribute.Should().NotBeNull();
      componentAttribute!.AttributeClass!.Name.Should().Contain("ValueObjectAttribute");
   }

   [Fact]
   public void Should_return_error_when_multiple_ThinktectureComponentAttributes_are_present()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject<int>]
         [SmartEnum<int>]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out _);

      // Assert
      error.Should().NotBeNull();
      error!.Value.Descriptor.Title.ToString().Should().Be("Type must not have more than one ValueObject/SmartEnum/Union-attribute");
      error.Value.Args.Should().BeEquivalentTo(["TestClass"]);
   }

   [Fact]
   public void Should_handle_multiple_different_attributes_at_once()
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
               private TestClass(string value)
               {
               }
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

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.HasStructLayoutAttribute.Should().BeTrue();
      info.KeyMemberEqualityComparerAccessor.Should().Be("global::TestNamespace.MyEqualityComparer");
      info.KeyMemberComparerAccessor.Should().Be("global::TestNamespace.MyComparer");
      info.ValidationError.TypeFullyQualified.Should().Be("global::TestNamespace.MyValidationError");
      info.ObjectFactories.Should().HaveCount(1);
   }

   [Fact]
   public void Should_skip_attribute_when_AttributeClass_is_erroneous()
   {
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ValueObject2<int>]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source, expectedCompilerErrors:
      [
         "The type or namespace name 'ValueObject2Attribute<>' could not be found (are you missing a using directive or an assembly reference?)",
         "The type or namespace name 'ValueObject2<>' could not be found (are you missing a using directive or an assembly reference?)"
      ]);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out _);

      // Assert
      error.Should().BeNull();
   }

   [Fact]
   public void Should_return_null_componentAttribute_when_no_ThinktectureComponentAttribute_present()
   {
      // Arrange
      var source = """
         using System.Runtime.InteropServices;

         namespace TestNamespace;

         [StructLayout(LayoutKind.Sequential)]
         public class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info, out var componentAttribute);

      // Assert
      error.Should().BeNull();
      componentAttribute.Should().BeNull();
      info.HasStructLayoutAttribute.Should().BeTrue();
   }

   [Theory]
   [InlineData("ValueObject<int>")]
   [InlineData("SmartEnum<int>")]
   [InlineData("ComplexValueObject")]
   [InlineData("Union")]
   public void Should_detect_different_ThinktectureComponentAttributes(string attributeName)
   {
      // Arrange
      var source = $$"""
         using Thinktecture;

         namespace TestNamespace;

         [{{attributeName}}]
         public partial class TestClass
         {
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out _, out var componentAttribute);

      // Assert
      error.Should().BeNull();
      componentAttribute.Should().NotBeNull();
   }

   [Fact]
   public void Should_handle_ObjectFactory_with_all_properties_set()
   {
      // Arrange
      var source = """
         using Thinktecture;

         namespace TestNamespace;

         [ObjectFactory<string>(
            UseForSerialization = SerializationFrameworks.SystemTextJson | SerializationFrameworks.MessagePack,
            UseWithEntityFramework = true,
            UseForModelBinding = true,
            HasCorrespondingConstructor = true)]
         [ValueObject<int>]
         public partial class TestClass
         {
            private TestClass(string value)
            {
            }
         }
         """;

      var compilation = CreateCompilation(source);
      var testClassSymbol = GetTypeSymbol(compilation, "TestNamespace.TestClass");

      // Act
      var error = AttributeInfo.TryCreate(testClassSymbol, out var info);

      // Assert
      error.Should().BeNull();
      info.ObjectFactories.Should().HaveCount(1);
      var factory = info.ObjectFactories[0];
      factory.TypeFullyQualified.Should().Be("string");
      factory.UseForSerialization.Should().Be(Thinktecture.CodeAnalysis.SerializationFrameworks.SystemTextJson | Thinktecture.CodeAnalysis.SerializationFrameworks.MessagePack);
      factory.UseWithEntityFramework.Should().BeTrue();
      factory.UseForModelBinding.Should().BeTrue();
      factory.HasCorrespondingConstructor.Should().BeTrue();
   }
}
