using System.Threading.Tasks;
using Newtonsoft.Json;
using Thinktecture.CodeAnalysis.ValueObjects;
using Thinktecture.Json;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class NewtonsoftJsonValueObjectCodeGeneratorFactoryTests : SourceGeneratorTestsBase
{
   public NewtonsoftJsonValueObjectCodeGeneratorFactoryTests(ITestOutputHelper output)
      : base(output, 10_000)
   {
   }

   [Fact]
   public void Instance_should_not_be_null()
   {
      NewtonsoftJsonValueObjectCodeGeneratorFactory.Instance.Should().NotBeNull();
   }

   [Fact]
   public void Instance_should_return_same_instance()
   {
      var instance1 = NewtonsoftJsonValueObjectCodeGeneratorFactory.Instance;
      var instance2 = NewtonsoftJsonValueObjectCodeGeneratorFactory.Instance;

      instance1.Should().BeSameAs(instance2);
   }

   [Fact]
   public void CodeGeneratorName_should_return_correct_value()
   {
      var instance = NewtonsoftJsonValueObjectCodeGeneratorFactory.Instance;

      instance.CodeGeneratorName.Should().Be("NewtonsoftJson-ValueObject-CodeGenerator");
   }

   [Fact]
   public void Instance_should_be_IValueObjectSerializerCodeGeneratorFactory()
   {
      NewtonsoftJsonValueObjectCodeGeneratorFactory.Instance.Should().BeAssignableTo<IValueObjectSerializerCodeGeneratorFactory>();
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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_JsonConverterAttribute_is_present()
   {
      var source = """

         using System;
         using Newtonsoft.Json;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class TestValueObjectJsonConverter : JsonConverter
            {
               public override bool CanConvert(Type objectType)
               {
                  return objectType == typeof(TestValueObject);
               }

               public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
               {
                  throw new NotImplementedException();
               }

               public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_SerializationFrameworks_excludes_NewtonsoftJson()
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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_with_NewtonsoftJson_exists()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_when_ObjectFactory_exists_but_not_for_NewtonsoftJson()
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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_when_multiple_ObjectFactories_exist_without_NewtonsoftJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_one_ObjectFactory_uses_NewtonsoftJson_among_multiple()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            [ObjectFactory<double>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_with_combined_flags_including_NewtonsoftJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson | SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_when_ObjectFactory_with_combined_flags_excluding_NewtonsoftJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack | SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_both_JsonConverter_and_ObjectFactory_present()
   {
      var source = """

         using System;
         using Newtonsoft.Json;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class TestValueObjectJsonConverter : JsonConverter
            {
               public override bool CanConvert(Type objectType)
               {
                  return objectType == typeof(TestValueObject);
               }

               public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
               {
                  throw new NotImplementedException();
               }

               public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_SerializationFrameworks_combined_excludes_NewtonsoftJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SerializationFrameworks = SerializationFrameworks.MessagePack | SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_when_SerializationFrameworks_includes_NewtonsoftJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SerializationFrameworks = SerializationFrameworks.NewtonsoftJson | SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_when_ObjectFactory_None_but_treats_as_no_NewtonsoftJson()
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
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      // This should generate because ObjectFactory with None doesn't claim NewtonsoftJson
      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_value_object_with_decimal_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<decimal>]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_complex_value_object_struct()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
         	public partial struct TestValueObject
         	{
               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               public int IntValue { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_SerializationFrameworks_SystemTextJson_only()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SerializationFrameworks = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_when_SerializationFrameworks_is_All()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>(SerializationFrameworks = SerializationFrameworks.All)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_complex_value_object_with_only_fields()
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

               public readonly int _intValue;
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_value_object_with_long_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<long>]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_multiple_ObjectFactories_all_include_NewtonsoftJson()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.NewtonsoftJson | SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute<>).Assembly,
                                                                  typeof(ObjectFactoryAttribute).Assembly,
                                                                  typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                  typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }
}
