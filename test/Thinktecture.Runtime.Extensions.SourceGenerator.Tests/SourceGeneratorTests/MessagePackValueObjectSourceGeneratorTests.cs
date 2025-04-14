using System.Threading.Tasks;
using MessagePack;
using Thinktecture.CodeAnalysis.ValueObjects;
using Thinktecture.Formatters;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class MessagePackValueObjectSourceGeneratorTests : SourceGeneratorTestsBase
{
   public MessagePackValueObjectSourceGeneratorTests(ITestOutputHelper output)
      : base(output)
   {
   }

   [Fact]
   public async Task Should_generate_MessagePackFormatter_and_attribute_for_keyed_class()
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
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePackFormatter_and_attribute_for_keyed_class_without_namespace()
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
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePackFormatter_and_attribute_for_keyed_struct()
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
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePackFormatter_and_attribute_for_complex_class()
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
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePackFormatter_and_attribute_for_complex_class_without_namespace()
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
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePackFormatter_and_attribute_for_complex_struct()
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
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_MessagePackFormatter_for_complex_value_object_with_partial_keys()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
            public partial class ComplexValueObjectWithPartialKeys
            {
               public decimal Property1 { get; }

               [MessagePack.Key(5)]
               public string Property2 { get; }

               public int Property3 { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ValueObjectMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }
}
