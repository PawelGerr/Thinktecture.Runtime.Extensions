using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ObjectFactories;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class NewtonsoftJsonAdHocUnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public NewtonsoftJsonAdHocUnionSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 1_000)
   {
   }

   [Fact]
   public async Task Should_generate_attribute_if_attribute_is_missing()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            [Union<string, int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            public partial class TestUnion;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(UnionAttribute).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_attribute_for_type_without_namespace()
   {
      var source = """

         using System;
         using Thinktecture;

         [Union<string, int>]
         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
         public partial class TestUnion;

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(UnionAttribute).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_attribute_if_no_factory()
   {
      var source = """

         using System;

         namespace Thinktecture.Tests
         {
            public class TestJsonConverter
            {
            }

            [Union<string, int>]
            public partial class TestUnion;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(UnionAttribute).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_attribute_if_attribute_is_present()
   {
      var source = """

         using System;
         using Newtonsoft.Json;

         namespace Thinktecture.Tests
         {
            public class TestJsonConverter
            {
            }

            [Union<string, int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            [JsonConverterAttribute(typeof(TestJsonConverter))]
            public partial class TestUnion;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(UnionAttribute).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_attribute_when_disabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         [Union<string, int>]
         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         public partial class TestUnion;

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(UnionAttribute).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_attribute_when_enabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         [Union<string, int>]
         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
         public partial class TestUnion;

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(UnionAttribute).Assembly, typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }
}
