using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.SmartEnums;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class NewtonsoftJsonSmartEnumSourceGeneratorTests : SourceGeneratorTestsBase
{
   public NewtonsoftJsonSmartEnumSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 1_000)
   {
   }

   [Fact]
   public async Task Should_generate_NewtonsoftJsonConverter_and_Attribute_if_Attribute_is_missing()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_NewtonsoftJsonConverter_for_enum_without_namespace()
   {
      var source = """

         using System;
         using Thinktecture;

            [SmartEnum<string>]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
            public static readonly TestEnum Item2 = default!;
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_NewtonsoftJsonConverter_and_attribute_if_Attribute_is_present()
   {
      var source = """

         using System;
         using Newtonsoft.Json;

         namespace Thinktecture.Tests
         {
            public class TestEnum_ThinktectureNewtonsoftJsonConverter : JsonConverter<TestEnum>
            {
               public override void WriteJson(JsonWriter writer, TestEnum value, JsonSerializer serializer)
               {
                  throw new NotImplementedException();
               }

               public override TestEnum ReadJson(JsonReader reader, Type objectType, TestEnum existingValue, bool hasExistingValue, JsonSerializer serializer)
               {
                  throw new NotImplementedException();
               }
            }

            [SmartEnum<string>]
            [JsonConverter(typeof(TestEnum_ThinktectureNewtonsoftJsonConverter))]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_NewtonsoftJsonConverter_for_keyless_enum()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_JsonConverter_when_disabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         [SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.MessagePack)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
            public static readonly TestEnum Item2 = default!;
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      output.Should().BeNull();
   }

   // Key Type Variations Tests

   [Fact]
   public async Task Should_generate_converter_for_int_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            public partial class StatusCode
            {
               public static readonly StatusCode Ok = default!;
               public static readonly StatusCode NotFound = default!;
               public static readonly StatusCode ServerError = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_guid_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<Guid>]
            public partial class EntityId
            {
               public static readonly EntityId First = default!;
               public static readonly EntityId Second = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_decimal_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<decimal>]
            public partial class PriceLevel
            {
               public static readonly PriceLevel Low = default!;
               public static readonly PriceLevel Medium = default!;
               public static readonly PriceLevel High = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_long_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<long>]
            public partial class BigId
            {
               public static readonly BigId Item1 = default!;
               public static readonly BigId Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_byte_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<byte>]
            public partial class TinyEnum
            {
               public static readonly TinyEnum Min = default!;
               public static readonly TinyEnum Max = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_double_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<double>]
            public partial class Measurement
            {
               public static readonly Measurement Zero = default!;
               public static readonly Measurement One = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_enum_as_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public enum UnderlyingEnum
            {
               Value1 = 1,
               Value2 = 2
            }

            [SmartEnum<UnderlyingEnum>]
            public partial class SmartEnumWithEnumKey
            {
               public static readonly SmartEnumWithEnumKey First = default!;
               public static readonly SmartEnumWithEnumKey Second = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   // Structural Variations Tests

   [Fact]
   public async Task Should_generate_converter_for_nested_enum()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class OuterClass
            {
               [SmartEnum<string>]
               public partial class NestedEnum
               {
                  public static readonly NestedEnum Item1 = default!;
                  public static readonly NestedEnum Item2 = default!;
               }
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_deeply_nested_enum()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class Level1
            {
               public class Level2
               {
                  [SmartEnum<int>]
                  public partial class DeeplyNestedEnum
                  {
                     public static readonly DeeplyNestedEnum Item1 = default!;
                     public static readonly DeeplyNestedEnum Item2 = default!;
                  }
               }
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_enum_with_derived_types()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            public abstract partial class BaseEnum
            {
               public static readonly DerivedEnum1 Item1 = default!;
               public static readonly DerivedEnum2 Item2 = default!;
            }

            public partial class DerivedEnum1 : BaseEnum
            {
            }

            public partial class DerivedEnum2 : BaseEnum
            {
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   // Configuration Variations Tests

   [Fact]
   public async Task Should_generate_converter_for_enum_with_field_key_member()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberKind = MemberKind.Field, KeyMemberAccessModifier = AccessModifier.Public, KeyMemberName = "Key")]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_enum_with_custom_key_member_name()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>(KeyMemberName = "Identifier")]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_enum_with_private_key_member()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberAccessModifier = AccessModifier.Private)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_with_multiple_serialization_frameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.NewtonsoftJson | SerializationFrameworks.SystemTextJson)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_converter_for_multiple_enums_in_same_file()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public partial class FirstEnum
            {
               public static readonly FirstEnum Item1 = default!;
               public static readonly FirstEnum Item2 = default!;
            }

            [SmartEnum<int>]
            public partial class SecondEnum
            {
               public static readonly SecondEnum Item1 = default!;
               public static readonly SecondEnum Item2 = default!;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.FirstEnum.NewtonsoftJson.g.cs",
                        "Thinktecture.Tests.SecondEnum.NewtonsoftJson.g.cs");
   }

   [Fact]
   public async Task Should_generate_converter_for_enum_with_single_item()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public partial class SingleItemEnum
            {
               public static readonly SingleItemEnum OnlyItem = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_when_enabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         [SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.NewtonsoftJson)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
            public static readonly TestEnum Item2 = default!;
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }
}
