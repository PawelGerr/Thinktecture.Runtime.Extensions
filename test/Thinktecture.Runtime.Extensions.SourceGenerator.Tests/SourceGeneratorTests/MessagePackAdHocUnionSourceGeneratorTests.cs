using System.Threading.Tasks;
using MessagePack;
using Thinktecture.CodeAnalysis.AdHocUnions;
using Thinktecture.CodeAnalysis.ObjectFactories;
using Thinktecture.Formatters;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class MessagePackAdHocUnionSourceGeneratorTests : SourceGeneratorTestsBase
{
   public MessagePackAdHocUnionSourceGeneratorTests(ITestOutputHelper output)
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
                                                                    ".MessagePack",
                                                                    typeof(UnionAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

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
                                                                    ".MessagePack",
                                                                    typeof(UnionAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_attribute_if_no_factory()
   {
      var source = """

         using System;
         using System.Text.Json.Serialization;

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
                                                                    ".MessagePack",
                                                                    typeof(UnionAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_attribute_if_attribute_is_present()
   {
      var source = """

         using System;
         using MessagePack;

         namespace Thinktecture.Tests
         {
            public class TestMessagePackFormatter
            {
            }

            [Union<string, int>]
            [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.All)]
            [MessagePackFormatter(typeof(TestMessagePackFormatter))]
            public partial class TestUnion;
         }

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(UnionAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public void Should_not_generate_attribute_when_disabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         [Union<string, int>]
         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.SystemTextJson)]
         public partial class TestUnion;

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(UnionAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_attribute_when_enabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         [Union<string, int>]
         [ObjectFactory<string>(UseForSerialization = SerializationFrameworks.MessagePack)]
         public partial class TestUnion;

         """;
      var output = GetGeneratedOutput<ObjectFactorySourceGenerator>(source,
                                                                    ".MessagePack",
                                                                    typeof(UnionAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }
}
