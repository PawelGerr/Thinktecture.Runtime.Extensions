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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
            public static readonly TestEnum Item1 = new("Item1");
            public static readonly TestEnum Item2 = new("Item2");
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
            public class TestEnum_ThinktectureNewtonsoftJsonConverter : Thinktecture.Json.ThinktectureNewtonsoftJsonConverter<TestEnum, string>
            {
               public TestEnum_ThinktectureNewtonsoftJsonConverter()
                  : base(TestEnum.Get)
               {
               }
            }

            [SmartEnum<string>]
            [JsonConverterAttribute(typeof(TestEnum_ThinktectureNewtonsoftJsonConverter))]
         	public partial class TestEnum
         	{
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
               public static readonly TestEnum Item1 = new("Item1");
               public static readonly TestEnum Item2 = new("Item2");
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
            public static readonly TestEnum Item1 = new("Item1");
            public static readonly TestEnum Item2 = new("Item2");
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      output.Should().BeNull();
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
            public static readonly TestEnum Item1 = new("Item1");
            public static readonly TestEnum Item2 = new("Item2");
         }

         """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(ISmartEnum<>).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }
}
