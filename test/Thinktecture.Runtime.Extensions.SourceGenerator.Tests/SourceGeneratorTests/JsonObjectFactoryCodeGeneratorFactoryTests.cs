using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.ObjectFactories;
using Thinktecture.Text.Json.Serialization;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class JsonObjectFactoryCodeGeneratorFactoryTests : SourceGeneratorTestsBase
{
   public JsonObjectFactoryCodeGeneratorFactoryTests(ITestOutputHelper output)
      : base(output, 2_000)
   {
   }

   [Fact]
   public void Instance_should_not_be_null()
   {
      JsonObjectFactoryCodeGeneratorFactory.Instance.Should().NotBeNull();
   }

   [Fact]
   public void Instance_should_return_same_instance()
   {
      var instance1 = JsonObjectFactoryCodeGeneratorFactory.Instance;
      var instance2 = JsonObjectFactoryCodeGeneratorFactory.Instance;

      instance1.Should().BeSameAs(instance2);
   }

   [Fact]
   public void CodeGeneratorName_should_return_correct_value()
   {
      var instance = JsonObjectFactoryCodeGeneratorFactory.Instance;

      instance.CodeGeneratorName.Should().Be("SystemTextJson-ObjectFactory-CodeGenerator");
   }

   [Fact]
   public void Instance_should_be_IKeyedSerializerCodeGeneratorFactory()
   {
      JsonObjectFactoryCodeGeneratorFactory.Instance.Should().BeAssignableTo<IKeyedSerializerCodeGeneratorFactory>();
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_smart_enum_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_value_object_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_ObjectFactory_with_SerializationFrameworks_All()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_Json_converter_for_ObjectFactory_with_SerializationFrameworks_None()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.None)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_Json_converter_for_ObjectFactory_with_SerializationFrameworks_MessagePack_only()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_JsonConverterAttribute_is_present()
   {
      var source = """

         using System;
         using System.Text.Json;
         using System.Text.Json.Serialization;

         namespace Thinktecture.Tests
         {
            public class TestEnumJsonConverter : JsonConverter<TestEnum>
            {
               public override TestEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
               {
                  throw new NotImplementedException();
               }

               public override void Write(Utf8JsonWriter writer, TestEnum value, JsonSerializerOptions options)
               {
                  throw new NotImplementedException();
               }
            }

            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            [JsonConverter(typeof(TestEnumJsonConverter))]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_multiple_ObjectFactories_with_SystemTextJson_enabled()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.MessagePack)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_all_ObjectFactories_exclude_SystemTextJson()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_struct_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial struct TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_complex_value_object_with_ObjectFactory()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject
         	{
               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               public int IntValue { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ComplexValueObjectAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_union_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [Union<string, int>]
            [ObjectFactory<double>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            public partial class TestUnion;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(UnionAttribute<,>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_with_NewtonsoftJson_only()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Json_converter_when_one_ObjectFactory_enables_SystemTextJson_among_multiple()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.None)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            [ObjectFactory<double>(UseForSerialization = SerializationFrameworks.None)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_keyless_smart_enum_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(SmartEnumAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_ad_hoc_union_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [AdHocUnion(typeof(string), typeof(int))]
            [ObjectFactory<double>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
            public partial class TestUnion;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(AdHocUnionAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_with_combination_excluding_SystemTextJson()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack | SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Json_converter_when_ObjectFactory_with_combined_flags_including_SystemTextJson()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson | SerializationFrameworks.MessagePack)]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ISmartEnum<>).Assembly,
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

         namespace Thinktecture.Tests
         {
            [ValueObject<Guid>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_value_object_with_string_key()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

#if NET9_0_OR_GREATER
   [Fact]
   public async Task Should_generate_Json_converter_for_value_object_with_ReadOnlySpan_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
            [ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }

#endif

#if NET9_0_OR_GREATER
   [Fact]
   public async Task Should_generate_Json_converter_for_complex_value_object_with_ReadOnlySpan_ObjectFactory()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests
         {
            [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
            [ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject
         	{
               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               public int IntValue { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(ComplexValueObjectAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }
#endif

   [Fact]
   public async Task Should_generate_Json_converter_for_regular_union_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [Union]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public abstract partial class TestUnion
         	{
               public sealed partial class TypeA : TestUnion
               {
                  public string Value { get; }
               }

               public sealed partial class TypeB : TestUnion
               {
                  public int Number { get; }
               }
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(UnionAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(JsonConverterAttribute).Assembly);

      await VerifyAsync(output);
   }
}
