using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ObjectFactories;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class JsonRegularUnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public JsonRegularUnionSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 1_000)
   {
   }

   [Fact]
   public async Task Should_generate_attribute_for_regular_union_with_json_factory()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Unions
         {
            [Union]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.Json)]
            public abstract partial record TestUnion
            {
            }
         }
         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".Json",
                                                                    typeof(UnionAttribute).Assembly,
                                                                    typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly,
                                                                    typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }
}
