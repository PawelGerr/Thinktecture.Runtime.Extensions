using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class JsonSmartEnumSourceGeneratorTests : SourceGeneratorTestsBase
{
   public JsonSmartEnumSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 1_000)
   {
   }

   [Fact]
   public async Task Should_generate_JsonConverter_and_Attribute_if_Attribute_is_missing()
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
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_for_enum_without_namespace()
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
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_JsonConverter_and_attribute_if_Attribute_is_present()
   {
      var source = """

         using System;
         using System.Text.Json;
         using System.Text.Json.Serialization;

         namespace Thinktecture.Tests
         {
            public class TestEnumJsonConverter : JsonConverter<TestEnum>
            {
               public override TestEnum Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
               {
                  throw new NotImplementedException();
               }

               public override void Write(Utf8JsonWriter writer, TestEnum value, JsonSerializerOptions options)
               {
                  throw new NotImplementedException();
               }
            }

            [SmartEnum<string>]
            [JsonConverter(typeof(TestEnumJsonConverter))]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_JsonConverter_for_keyless_enum()
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
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

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
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_JsonConverter_when_enabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         [SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.SystemTextJson)]
         public partial class TestEnum
         {
            public static readonly TestEnum Item1 = default!;
            public static readonly TestEnum Item2 = default!;
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converter_for_int_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converter_for_guid_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<Guid>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converter_with_public_property_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberKind = MemberKind.Property, KeyMemberAccessModifier = AccessModifier.Public)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converter_with_private_field_key()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberKind = MemberKind.Field, KeyMemberAccessModifier = AccessModifier.Private)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converter_with_custom_key_member_name()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(KeyMemberName = "Identifier")]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_not_generate_json_converter_for_record_type()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public partial record TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_json_converter_for_struct_type()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<int>]
            public partial struct TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                [typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly],
                                                                ["Attribute 'SmartEnum<>' is not valid on this declaration type. It is only valid on 'class' declarations."]);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_json_converter_for_internal_enum()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            internal partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converter_for_nested_type()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class OuterClass
            {
               [SmartEnum<string>]
               public partial class TestEnum
               {
                  public static readonly TestEnum Item1 = default!;
                  public static readonly TestEnum Item2 = default!;
               }
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converters_for_multiple_enums_in_same_file()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public partial class TestEnum1
            {
               public static readonly TestEnum1 Item1 = default!;
               public static readonly TestEnum1 Item2 = default!;
            }

            [SmartEnum<int>]
            public partial class TestEnum2
            {
               public static readonly TestEnum2 Item1 = default!;
               public static readonly TestEnum2 Item2 = default!;
            }
         }

         """;
      var outputs = GetGeneratedOutputs<SmartEnumSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(outputs,
                        "Thinktecture.Tests.TestEnum1.Json.g.cs",
                        "Thinktecture.Tests.TestEnum2.Json.g.cs");
   }

   [Fact]
   public async Task Should_generate_json_converter_for_derived_smart_enum()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            public class BaseClass
            {
            }

            [SmartEnum<string>]
            public partial class DerivedEnum : BaseClass
            {
               public static readonly DerivedEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converter_with_multiple_frameworks_enabled()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.SystemTextJson | SerializationFrameworks.MessagePack)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_json_converter_when_disabled_with_other_frameworks_enabled()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(SerializationFrameworks = SerializationFrameworks.MessagePack)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_json_converter_for_enum_in_global_namespace()
   {
      var source = """

         using System;
         using Thinktecture;

         [SmartEnum<string>]
         public partial class TestEnumGlobal
         {
            public static readonly TestEnumGlobal Item1 = default!;
            public static readonly TestEnumGlobal Item2 = default!;
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converter_for_deeply_nested_namespace()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace A.B.C.D.E.F
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
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_json_converter_without_span_when_DisableSpanBasedJsonConversion_is_true()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>(DisableSpanBasedJsonConversion = true)]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
               public static readonly TestEnum Item2 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   // NICE-TO-HAVE TESTS - Multiple Items Edge Cases

   [Fact]
   public async Task Should_generate_json_converter_for_enum_with_single_item()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
            public partial class TestEnum
            {
               public static readonly TestEnum Item1 = default!;
            }
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_json_converter_for_keyless_enum()
   {
      var source = """

         using System;
         using Thinktecture;

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
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      output.Should().BeNull();
   }
}
