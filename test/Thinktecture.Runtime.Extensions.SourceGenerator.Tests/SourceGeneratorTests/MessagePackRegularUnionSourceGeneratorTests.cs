using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ObjectFactories;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class MessagePackRegularUnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public MessagePackRegularUnionSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 1_000)
   {
   }

   [Fact]
   public async Task Should_generate_attribute_for_regular_union_with_messagepack_factory()
   {
      var source = """
         using System;
         using Thinktecture;

         namespace Thinktecture.Unions
         {
            [Union]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
            public abstract partial record TestUnion
            {
            }
         }
         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(UnionAttribute).Assembly,
                                                                    typeof(Formatters.ThinktectureMessagePackFormatter<,,>).Assembly,
                                                                    typeof(MessagePack.MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }
}
