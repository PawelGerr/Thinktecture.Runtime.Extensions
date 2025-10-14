using System.Threading.Tasks;
using Thinktecture.CodeAnalysis.ValueObjects;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class JsonValueObjectSourceGeneratorTests : SourceGeneratorTestsBase
{
   public JsonValueObjectSourceGeneratorTests(ITestOutputHelper output)
      : base(output, 24_000)
   {
   }

   [Fact]
   public async Task Should_generate_JsonConverter_and_attribute_for_keyed_class()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_and_attribute_for_keyed_class_without_namespace()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_and_attribute_for_keyed_struct()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_and_attribute_for_complex_class()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_and_attribute_for_complex_struct()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_and_attribute_for_complex_struct_with_AllowDefaultStructs()
   {
      var source = """

         using System;
         using Thinktecture;

         namespace Thinktecture.Tests
         {
            [ComplexValueObject(AllowDefaultStructs = true)]
         	public partial struct TestValueObject
         	{
               public readonly string ReferenceField;
               public int StructProperty { get; }
               public decimal? NullableStructProperty { get; }
            }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_complex_class_if_Attribute_is_missing()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_and_attribute_for_complex_struct_without_namespace()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_for_complex_value_object_with_non_nullable_property()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable enable

         namespace Thinktecture.Tests;

         [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
         public partial struct ComplexValueObjectWithNonNullProperty
         {
            public string Property { get; }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_for_complex_value_object_with_property_without_Nullable_Annotations()
   {
      var source = """

         using System;
         using Thinktecture;

         #nullable disable

         namespace Thinktecture.Tests;

         [ComplexValueObject(DefaultStringComparison = StringComparison.OrdinalIgnoreCase)]
         public partial struct ComplexValueObjectWithNonNullProperty
         {
            public string Property { get; }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_JsonConverter_for_keyed_when_disabled_via_SerializationFrameworks()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_JsonConverter_for_keyed_when_enabled_via_SerializationFrameworks()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public void Should_not_generate_JsonConverter_for_complex_when_disabled_via_SerializationFrameworks()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      output.Should().BeNull();
   }

   [Fact]
   public async Task Should_generate_JsonConverter_for_complex_when_enabled_via_SerializationFrameworks()
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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

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
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_for_complex_JsonIgnoreAttribute()
   {
      var source = """

         using System;
         using Thinktecture;
         using System.Text.Json.Serialization;

         #nullable disable

         namespace Thinktecture.Tests;

         [ComplexValueObject]
         public partial class ComplexValueObjectWithJsonIgnore
         {
            [JsonIgnore]
            public string? StringProperty_Ignore { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public string? StringProperty_Ignore_Always { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public string? StringProperty_Ignore_WhenWritingDefault { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string? StringProperty_Ignore_WhenWritingNull { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public string? StringProperty_Ignore_Never { get; }

            public string? StringProperty { get; }

            [JsonIgnore]
            public int IntProperty_Ignore { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public int IntProperty_Ignore_Always { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public int IntProperty_Ignore_WhenWritingDefault { get; }

            // WhenWritingNull on structs is not valid
            // [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            // public int IntProperty_Ignore_WhenWritingNull { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public int IntProperty_Ignore_Never { get; }

            public int IntProperty { get; }

            [JsonIgnore]
            public int? NullableIntProperty_Ignore { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public int? NullableIntProperty_Ignore_Always { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
            public int? NullableIntProperty_Ignore_WhenWritingDefault { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? NullableIntProperty_Ignore_WhenWritingNull { get; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            public int? NullableIntProperty_Ignore_Never { get; }

            public int? NullableIntProperty { get; }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }

   [Fact]
   public async Task Should_generate_JsonConverter_for_generic_complex_value_object()
   {
      var source = """

         using System;
         using Thinktecture;
         using System.Text.Json.Serialization;

         #nullable disable

         namespace Thinktecture.Tests;

         [ComplexValueObject]
         public partial class ComplexValueObject<T1, T2, T3>
            where T1 : class, IEquatable<T1>
            where T3 : struct
         {
            public T1 Property { get; }
            public T2? NullableProperty { get; }
            public T3 StructProperty { get; }
            public T3? NullableStructProperty { get; }
         }

         """;
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ComplexValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ThinktectureJsonConverter<,,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      await VerifyAsync(output);
   }
}
