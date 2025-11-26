using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ValueObjects;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class JsonValueObjectCodeGeneratorFactoryTests : SourceGeneratorTestsBase
{
   public JsonValueObjectCodeGeneratorFactoryTests(ITestOutputHelper output)
      : base(output, 10_000)
   {
   }

   [Fact]
   public void Instance_should_not_be_null()
   {
      JsonValueObjectCodeGeneratorFactory.Instance.Should().NotBeNull();
   }

   [Fact]
   public void Instance_should_return_same_instance()
   {
      var instance1 = JsonValueObjectCodeGeneratorFactory.Instance;
      var instance2 = JsonValueObjectCodeGeneratorFactory.Instance;

      instance1.Should().BeSameAs(instance2);
   }

   [Fact]
   public void CodeGeneratorName_should_return_correct_value()
   {
      var instance = JsonValueObjectCodeGeneratorFactory.Instance;

      instance.CodeGeneratorName.Should().Be("SystemTextJson-ValueObject-CodeGenerator");
   }

   [Fact]
   public void Instance_should_be_IValueObjectSerializerCodeGeneratorFactory()
   {
      JsonValueObjectCodeGeneratorFactory.Instance.Should().BeAssignableTo<IValueObjectSerializerCodeGeneratorFactory>();
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_keyed_value_object()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_complex_value_object()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
         	public partial class TestValueObject
         	{
               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               public int IntValue { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_struct_value_object()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
         	public partial struct TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_JsonConverterAttribute_is_present()
   {
      var source = """

         using System;
         using System.Text.Json;
         using System.Text.Json.Serialization;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class TestValueObjectJsonConverter : JsonConverter<TestValueObject>
            {
               public override TestValueObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
               {
                  throw new NotImplementedException();
               }

               public override void Write(Utf8JsonWriter writer, TestValueObject value, JsonSerializerOptions options)
               {
                  throw new NotImplementedException();
               }
            }

            [ValueObject<int>]
            [JsonConverter(typeof(TestValueObjectJsonConverter))]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_SerializationFrameworks_is_None()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SerializationFrameworks = SerializationFrameworks.None)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_SerializationFrameworks_excludes_SystemTextJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SerializationFrameworks = SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_with_SystemTextJson_exists()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_when_ObjectFactory_exists_but_not_for_SystemTextJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_has_SerializationFrameworks_All()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_when_multiple_ObjectFactories_exist_without_SystemTextJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_one_ObjectFactory_uses_SystemTextJson_among_multiple()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            [ObjectFactory<double>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_with_combined_flags_including_SystemTextJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson | SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_when_ObjectFactory_with_combined_flags_excluding_SystemTextJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack | SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_value_object_with_Guid_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<Guid>]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_value_object_with_string_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_nested_value_object()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public partial class OuterClass
            {
               [ValueObject<int>]
               public partial class TestValueObject;
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_generic_complex_value_object()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
         	public partial class Range<T>
               where T : IComparable<T>
         	{
               public T Start { get; }
               public T End { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_both_JsonConverter_and_ObjectFactory_present()
   {
      var source = """

         using System;
         using System.Text.Json;
         using System.Text.Json.Serialization;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class TestValueObjectJsonConverter : JsonConverter<TestValueObject>
            {
               public override TestValueObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
               {
                  throw new NotImplementedException();
               }

               public override void Write(Utf8JsonWriter writer, TestValueObject value, JsonSerializerOptions options)
               {
                  throw new NotImplementedException();
               }
            }

            [ValueObject<int>]
            [JsonConverter(typeof(TestValueObjectJsonConverter))]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_SerializationFrameworks_combined_excludes_SystemTextJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SerializationFrameworks = SerializationFrameworks.MessagePack | SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_when_SerializationFrameworks_includes_SystemTextJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SerializationFrameworks = SerializationFrameworks.SystemTextJson | SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_complex_value_object_with_nullable_members()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
         	public partial class TestValueObject
         	{
               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public string? NullableStringValue { get; }

               public int? NullableIntValue { get; }

               public decimal NonNullableDecimalValue { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_None_but_treats_as_no_SystemTextJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.None)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                  typeof(JsonConverterAttribute).Assembly);

      // This should actually generate because ObjectFactory with None doesn't claim SystemTextJson
      output.Should().NotBeNull();
   }
}
