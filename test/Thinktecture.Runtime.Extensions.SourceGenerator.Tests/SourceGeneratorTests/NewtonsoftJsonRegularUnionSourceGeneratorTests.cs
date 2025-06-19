using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ObjectFactories;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class NewtonsoftJsonRegularUnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public NewtonsoftJsonRegularUnionSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 1_000)
   {
   }

   [Fact]
   public async Task Should_generate_attribute_for_regular_union_with_newtonsoft_json_factory()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Unions
         {
            [Union]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.NewtonsoftJson)]
            public abstract partial record TestUnion
            {
            }
         }
         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".NewtonsoftJson",
                                                                    typeof(UnionAttribute).Assembly,
                                                                    typeof(Json.ThinktectureNewtonsoftJsonConverterFactory).Assembly,
                                                                    typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }
}
