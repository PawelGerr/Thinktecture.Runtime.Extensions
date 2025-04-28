using System.Threading.Tasks;
using MessagePack;
using Thinktecture.CodeAnalysis.ValueObjects;
using Thinktecture.Formatters;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class MessagePackValueObjectSourceGeneratorTests : SourceGeneratorTestsBase
{
   public MessagePackValueObjectSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 4_000)
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
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

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
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

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
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

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
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

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
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

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
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

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
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_Formatter_for_keyed_when_disabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>(SerializationFrameworks = SerializationFrameworks.SystemTextJson)]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Formatter_for_keyed_when_enabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ValueObject<string>(SerializationFrameworks = SerializationFrameworks.MessagePack)]
         	public partial class TestValueObject
         	{
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_Formatter_for_complex_when_disabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable disable

         namespace Thinktecture.Tests;

         [ComplexValueObject(SerializationFrameworks = SerializationFrameworks.SystemTextJson)]
         public partial class ComplexValueObjectWithNonNullProperty
         {
            public int Property { get; }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_Formatter_for_complex_when_enabled_via_SerializationFrameworks()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable disable

         namespace Thinktecture.Tests;

         [ComplexValueObject(SerializationFrameworks = SerializationFrameworks.MessagePack)]
         public partial class ComplexValueObjectWithNonNullProperty
         {
            public int Property { get; }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_for_complex_with_properties_disallowing_default_values()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable disable

         namespace Thinktecture.Tests;

         public class ClassDisallowingDefaultValues : IDisallowDefaultValue;
         public struct StructDisallowingDefaultValues : IDisallowDefaultValue;

         [ComplexValueObject]
         public partial class TestValueObject
         {
            public readonly ClassDisallowingDefaultValues _nonNullableReferenceType;
            public readonly ClassDisallowingDefaultValues? _nullableReferenceType;
            public readonly StructDisallowingDefaultValues _nonNullableStruct;
            public readonly StructDisallowingDefaultValues? _nullableStruct;
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".MessagePack",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ThinktectureMessagePackFormatter<,,>).Assembly, typeof(MessagePackFormatterAttribute).Assembly);

      await VerifyAsync(output);
   }
}
