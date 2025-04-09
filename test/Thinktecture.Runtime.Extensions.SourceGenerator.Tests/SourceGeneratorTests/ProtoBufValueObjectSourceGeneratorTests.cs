using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ValueObjects;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class ProtoBufValueObjectSourceGeneratorTests : SourceGeneratorTestsBase
{
   public ProtoBufValueObjectSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 15_000)
   {
   }

   [Fact]
   public async Task Should_generate_Serializer_and_attribute_for_keyed_class()
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
                                                                  ".ProtoBuf",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ProtoBuf.Serializers.ThinktectureSerializer<,,>).Assembly, typeof(global::ProtoBuf.Serializer).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Serializer_and_attribute_for_keyed_class_without_namespace()
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
                                                                  ".ProtoBuf",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ProtoBuf.Serializers.ThinktectureSerializer<,,>).Assembly, typeof(global::ProtoBuf.Serializer).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Serializer_and_attribute_for_keyed_struct()
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
                                                                  ".ProtoBuf",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ProtoBuf.Serializers.ThinktectureSerializer<,,>).Assembly, typeof(global::ProtoBuf.Serializer).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Serializer_and_attribute_for_complex_class_with_default_order()
   {
      var source = """

         using System;
         using Thinktecture;
         using ProtoBuf;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject]
         	public partial class TestValueObject
         	{
         	   [ProtoMember(1)]
               public readonly string ReferenceField;

               [ProtoMember(2)]
               public int StructProperty { get; }

               [ProtoMember(3)]
               public decimal? NullableStructProperty { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".ProtoBuf",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ProtoBuf.Serializers.ThinktectureSerializer<,,>).Assembly, typeof(global::ProtoBuf.Serializer).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Serializer_and_attribute_for_complex_class_without_ProtoMember()
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
                                                                  ".ProtoBuf",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ProtoBuf.Serializers.ThinktectureSerializer<,,>).Assembly, typeof(global::ProtoBuf.Serializer).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Serializer_and_attribute_for_complex_struct_without_ProtoMember()
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
                                                                  ".ProtoBuf",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ProtoBuf.Serializers.ThinktectureSerializer<,,>).Assembly, typeof(global::ProtoBuf.Serializer).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_Serializer_and_attribute_for_complex_struct_without_namespace()
   {
      var source = """

         using System;
         using Thinktecture;

         [ComplexValueObject]
         public partial struct TestValueObject
         {
            public readonly string ReferenceField;
            public int StructProperty { get; }
            public decimal? NullableStructProperty { get; }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".ProtoBuf",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(ProtoBuf.Serializers.ThinktectureSerializer<,,>).Assembly, typeof(global::ProtoBuf.Serializer).Assembly);

      await VerifyAsync(output);
   }
}
