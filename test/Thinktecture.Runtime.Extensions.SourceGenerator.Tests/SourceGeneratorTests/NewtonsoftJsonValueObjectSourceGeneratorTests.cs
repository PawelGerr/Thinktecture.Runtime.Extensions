using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ValueObjects;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class NewtonsoftJsonValueObjectSourceGeneratorTests : SourceGeneratorTestsBase
{
   public NewtonsoftJsonValueObjectSourceGeneratorTests(ITestOutputHelper output)
      : base(output)
   {
   }

   [Fact]
   public async Task Should_generate_NewtonsoftJsonConverter_and_attribute_for_keyed_class()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>]
                   	public partial class TestValueObject
                   	{
                   	}
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_NewtonsoftJsonConverter_and_attribute_for_keyed_class_without_namespace()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   [ValueObject<string>]
                   public partial class TestValueObject
                   {
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_NewtonsoftJsonConverter_and_attribute_for_keyed_struct()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ValueObject<string>]
                   	public partial struct TestValueObject
                   	{
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_NewtonsoftJsonConverter_and_attribute_for_complex_class()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                     [ComplexValueObject]
                   	public partial class TestValueObject
                   	{
                         public readonly string ReferenceField;
                         public int StructProperty { get; }
                         public decimal? NullableStructProperty { get; }
                     }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_NewtonsoftJsonConverter_and_attribute_for_complex_class_without_namespace()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   [ComplexValueObject]
                   public partial class TestValueObject
                   {
                      public readonly string ReferenceField;
                      public int StructProperty { get; }
                      public decimal? NullableStructProperty { get; }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_NewtonsoftJsonConverter_and_attribute_for_complex_struct()
   {
      var source = """

                   using System;
                   using Thinktecture;

                   namespace Thinktecture.Tests
                   {
                      [ComplexValueObject]
                   	public partial struct TestValueObject
                   	{
                         public readonly string ReferenceField;
                         public int StructProperty { get; }
                         public decimal? NullableStructProperty { get; }
                      }
                   }

                   """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".NewtonsoftJson",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      await VerifyAsync(output);
   }
}
