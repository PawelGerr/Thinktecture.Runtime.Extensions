using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.SmartEnums;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class NewtonsoftJsonSmartEnumSourceGeneratorTests : SourceGeneratorTestsBase
{
   public NewtonsoftJsonSmartEnumSourceGeneratorTests(ITestOutputHelper output)
      : base(output)
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
                                                                typeof(IEnum<>).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

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
                                                                typeof(IEnum<>).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_NewtonsoftJsonConverter_and_Attribute_for_struct_if_Attribute_is_missing()
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
                                                                ".NewtonsoftJson",
                                                                typeof(IEnum<>).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

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
                      public class TestEnum_ValueObjectNewtonsoftJsonConverter : Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<TestEnum, string>
                      {
                         public TestEnum_ValueObjectNewtonsoftJsonConverter()
                            : base(TestEnum.Get)
                         {
                         }
                      }

                      [SmartEnum<string>]
                      [JsonConverterAttribute(typeof(TestEnum_ValueObjectNewtonsoftJsonConverter))]
                   	public partial class TestEnum
                   	{
                         public static readonly TestEnum Item1 = new("Item1");
                         public static readonly TestEnum Item2 = new("Item2");
                      }
                   }

                   """;
      var output = GetGeneratedOutput<SmartEnumSourceGenerator>(source,
                                                                ".NewtonsoftJson",
                                                                typeof(IEnum<>).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

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
                                                                typeof(IEnum<>).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      output.Should().BeNull();
   }
}
