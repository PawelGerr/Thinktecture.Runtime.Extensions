using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.SmartEnums;
using Xunit.Abstractions;

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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
            public static readonly TestEnum Item1 = new("Item1");
            public static readonly TestEnum Item2 = new("Item2");
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_and_Attribute_for_struct_if_Attribute_is_missing()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [SmartEnum<string>]
         	public partial struct TestEnum
         	{
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
            }
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
         using System.Text.Json.Serialization;

         namespace Thinktecture.Tests
         {
            public class TestEnumJsonConverter : Thinktecture.Text.Json.Serialization.EnumJsonConverter<TestEnum, string>
            {
               public TestEnum_EnumJsonConverter()
                  : this(null)
               {
               }

               public TestEnum_EnumJsonConverter(
                  JsonConverter<string>? keyConverter)
                  : base(TestEnum.Get, keyConverter)
               {
               }
            }

            [SmartEnum<string>]
            [JsonConverter(typeof(TestEnumJsonConverter))]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
            public static readonly TestEnum Item1 = new("Item1");
            public static readonly TestEnum Item2 = new("Item2");
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
            public static readonly TestEnum Item1 = new("Item1");
            public static readonly TestEnum Item2 = new("Item2");
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".Json",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }
}
