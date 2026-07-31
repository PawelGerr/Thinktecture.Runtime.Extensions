using System.Threading.Tasks;
using Newtonsoft.Json;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.ObjectFactories;
using Thinktecture.Json;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class NewtonsoftJsonObjectFactoryCodeGeneratorFactoryTests : SourceGeneratorTestsBase
{
   public NewtonsoftJsonObjectFactoryCodeGeneratorFactoryTests(ITestOutputHelper output)
      : base(output, 2_000)
   {
   }

   [Fact]
   public void Instance_should_not_be_null()
   {
      NewtonsoftJsonObjectFactoryCodeGeneratorFactory.Instance.Should().NotBeNull();
   }

   [Fact]
   public void Instance_should_return_same_instance()
   {
      var instance1 = NewtonsoftJsonObjectFactoryCodeGeneratorFactory.Instance;
      var instance2 = NewtonsoftJsonObjectFactoryCodeGeneratorFactory.Instance;

      instance1.Should().BeSameAs(instance2);
   }

   [Fact]
   public void CodeGeneratorName_should_return_correct_value()
   {
      var instance = NewtonsoftJsonObjectFactoryCodeGeneratorFactory.Instance;

      instance.CodeGeneratorName.Should().Be("NewtonsoftJson-ObjectFactory-CodeGenerator");
   }

   [Fact]
   public void Instance_should_be_IKeyedSerializerCodeGeneratorFactory()
   {
      NewtonsoftJsonObjectFactoryCodeGeneratorFactory.Instance.Should().BeAssignableTo<IKeyedSerializerCodeGeneratorFactory>();
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_smart_enum_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestEnum
         	{
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestValueObject
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestValueObject? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_when_JsonConverterAttribute_is_present()
   {
      var source = """

         using System;
         using Newtonsoft.Json;

         namespace Thinktecture.Tests
         {
            public class TestEnumJsonConverter : JsonConverter
            {
               public override bool CanConvert(Type objectType)
               {
                  return objectType == typeof(TestEnum);
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

            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            [JsonConverter(typeof(TestEnumJsonConverter))]
         	public partial class TestEnum
         	{
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_multiple_ObjectFactories_with_NewtonsoftJson_enabled()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.MessagePack)]
         	public partial class TestEnum : IConvertible<Guid>, IConvertible<string>
         	{
               public static ValidationError? Validate(Guid value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               Guid IConvertible<Guid>.ToValue() => default!;

               string IConvertible<string>.ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_all_ObjectFactories_exclude_NewtonsoftJson()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestEnum : IConvertible<Guid>, IConvertible<string>
         	{
               public static ValidationError? Validate(Guid value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               Guid IConvertible<Guid>.ToValue() => default!;

               string IConvertible<string>.ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial struct TestValueObject
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestValueObject item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestValueObject
         	{
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestValueObject? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

               [MemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
               public readonly string _stringValue;

               public int IntValue { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ComplexValueObjectAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
            [ObjectFactory<double>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            public partial class TestUnion
            {
               public static ValidationError? Validate(double value, IFormatProvider? provider, out TestUnion? item)
               {
                  item = default;
                  return null;
               }

               public double ToValue() => default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(UnionAttribute<,>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_with_SystemTextJson_only()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         	public partial class TestEnum
         	{
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Json_converter_when_one_ObjectFactory_enables_NewtonsoftJson_among_multiple()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.None)]
            [ObjectFactory<Guid>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            [ObjectFactory<double>(UseForSerialization = SerializationFrameworks.None)]
         	public partial class TestEnum
         	{
               public static ValidationError? Validate(Guid value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public static ValidationError? Validate(double value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public Guid ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestEnum
         	{
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(SmartEnumAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
            [ObjectFactory<double>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            public partial class TestUnion
            {
               public static ValidationError? Validate(double value, IFormatProvider? provider, out TestUnion? item)
               {
                  item = default;
                  return null;
               }

               public double ToValue() => default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(AdHocUnionAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_when_ObjectFactory_with_combination_excluding_NewtonsoftJson()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack | SerializationFrameworks.SystemTextJson)]
         	public partial class TestEnum
         	{
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Json_converter_when_ObjectFactory_with_combined_flags_including_NewtonsoftJson()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson | SerializationFrameworks.MessagePack)]
         	public partial class TestEnum
         	{
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestEnum? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ISmartEnum<>).Assembly,
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

         namespace Thinktecture.Tests
         {
            [ValueObject<Guid>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestValueObject
            {
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestValueObject? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

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
            [ObjectFactory<int>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public partial class TestValueObject
            {
               public static ValidationError? Validate(int value, IFormatProvider? provider, out TestValueObject? item)
               {
                  item = default;
                  return null;
               }

               public int ToValue() => default!;
            }
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Json_converter_for_regular_union_with_ObjectFactory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [Union]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         	public abstract partial class TestUnion
         	{
               public static ValidationError? Validate(string? value, IFormatProvider? provider, out TestUnion? item)
               {
                  item = default;
                  return null;
               }

               public string ToValue() => default!;

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
                                                                    ".NewtonsoftJson",
                                                                    typeof(UnionAttribute).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

#if NET9_0_OR_GREATER
   // A ReadOnlySpan<char> object factory has no Newtonsoft.Json converter path. Even when it is flagged
   // for Newtonsoft.Json serialization, the generator must fall back to the key member instead of emitting
   // ReadOnlySpan<char> as the converter's key type argument, which cannot compile.
   [Fact]
   public async Task Should_fall_back_to_key_member_when_span_ObjectFactory_is_flagged_for_NewtonsoftJson()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
            [ObjectFactory<ReadOnlySpan<char>>(UseForSerialization = SerializationFrameworks.All)]
         	public partial class TestValueObject
            {
               public static ValidationError? Validate(ReadOnlySpan<char> value, IFormatProvider? provider, out TestValueObject? item)
               {
                  item = default;
                  return null;
               }

               public ReadOnlySpan<char> ToValue() => default!;
            }
         }

         """;
      var output = GetGeneratedOutput<Thinktecture.CodeAnalysis.ValueObjects.ValueObjectSourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      await VerifyAsync(output);
   }

   // The exclusion covers every ref struct, not only ReadOnlySpan<char>: no ref struct can be a generic argument
   // of the generated converter. A ReadOnlySpan<byte> factory flagged for Newtonsoft.Json must therefore fall back
   // to the key member as well.
   [Fact]
   public async Task Should_fall_back_to_key_member_when_byte_span_ObjectFactory_is_flagged_for_NewtonsoftJson()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>]
            [KeyMemberEqualityComparer<ComparerAccessors.StringOrdinal, string>]
            [ObjectFactory<ReadOnlySpan<byte>>(UseForSerialization = SerializationFrameworks.All)]
         	public partial class TestValueObject
            {
               public static ValidationError? Validate(ReadOnlySpan<byte> value, IFormatProvider? provider, out TestValueObject? item)
               {
                  item = default;
                  return null;
               }

               public ReadOnlySpan<byte> ToValue() => default!;
            }
         }

         """;
      var output = GetGeneratedOutput<Thinktecture.CodeAnalysis.ValueObjects.ValueObjectSourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(ValueObjectAttribute<>).Assembly,
                                                                    typeof(ObjectFactoryAttribute).Assembly,
                                                                    typeof(ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(JsonConverter).Assembly);

      output.Should().NotContain("ReadOnlySpan<byte>");

      await VerifyAsync(output);
   }
#endif
}
