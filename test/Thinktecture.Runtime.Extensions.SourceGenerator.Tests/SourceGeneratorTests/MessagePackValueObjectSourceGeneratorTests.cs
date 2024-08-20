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
   public void Should_generate_MessagePackFormatter_and_attribute_for_keyed_class()
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

      AssertOutput(output, """
                           // <auto-generated />
                           #nullable enable

                           namespace Thinktecture.Tests;

                           [global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.ValueObjectMessagePackFormatter<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>))]
                           partial class TestValueObject
                           {
                           }

                           """);
   }

   [Fact]
   public void Should_generate_MessagePackFormatter_and_attribute_for_keyed_class_without_namespace()
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

      AssertOutput(output, """
                           // <auto-generated />
                           #nullable enable

                           [global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.ValueObjectMessagePackFormatter<global::TestValueObject, string, global::Thinktecture.ValidationError>))]
                           partial class TestValueObject
                           {
                           }

                           """);
   }

   [Fact]
   public void Should_generate_MessagePackFormatter_and_attribute_for_keyed_struct()
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

      AssertOutput(output, """
                           // <auto-generated />
                           #nullable enable

                           namespace Thinktecture.Tests;

                           [global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.StructValueObjectMessagePackFormatter<global::Thinktecture.Tests.TestValueObject, string, global::Thinktecture.ValidationError>))]
                           partial struct TestValueObject
                           {
                           }

                           """);
   }

   [Fact]
   public void Should_generate_MessagePackFormatter_and_attribute_for_complex_class()
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

      AssertOutput(output, """
                           // <auto-generated />
                           #nullable enable

                           namespace Thinktecture.Tests;

                           [global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
                           partial class TestValueObject
                           {
                              public sealed class ValueObjectMessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Thinktecture.Tests.TestValueObject?>
                              {
                                 /// <inheritdoc />
                                 public global::Thinktecture.Tests.TestValueObject? Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
                                 {
                                    if (reader.TryReadNil())
                                       return default;

                                    var count = reader.ReadArrayHeader();

                                    if (count != 3)
                                       throw new global::MessagePack.MessagePackSerializationException($"Invalid member count. Expected 3 but found {count} field/property values.");

                                    global::MessagePack.IFormatterResolver resolver = options.Resolver;
                                    options.Security.DepthStep(ref reader);

                                    try
                                    {

                                       var referenceField = reader.ReadString()!;
                                       var structProperty = reader.ReadInt32()!;
                                       var nullableStructProperty = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<decimal?>(resolver).Deserialize(ref reader, options)!;

                                       var validationError = global::Thinktecture.Tests.TestValueObject.Validate(
                                                                  referenceField,
                                                                  structProperty,
                                                                  nullableStructProperty,
                                                                  out var obj);

                                       if (validationError is not null)
                                          throw new global::MessagePack.MessagePackSerializationException(validationError.ToString() ?? "Unable to deserialize \"TestValueObject\".");

                                       return obj;
                                    }
                                    finally
                                    {
                                      reader.Depth--;
                                    }
                                 }

                                 /// <inheritdoc />
                                 public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Thinktecture.Tests.TestValueObject? value, global::MessagePack.MessagePackSerializerOptions options)
                                 {
                                    if(value is null)
                                    {
                                       writer.WriteNil();
                                       return;
                                    }

                                    writer.WriteArrayHeader(3);

                                    var resolver = options.Resolver;
                                    writer.Write(value.ReferenceField);
                                    writer.Write(value.StructProperty);
                                    global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<decimal?>(resolver).Serialize(ref writer, value.NullableStructProperty, options);
                                 }
                              }
                           }

                           """);
   }

   [Fact]
   public void Should_generate_MessagePackFormatter_and_attribute_for_complex_class_without_namespace()
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

      AssertOutput(output, """
                           // <auto-generated />
                           #nullable enable

                           [global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
                           partial class TestValueObject
                           {
                              public sealed class ValueObjectMessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::TestValueObject?>
                              {
                                 /// <inheritdoc />
                                 public global::TestValueObject? Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
                                 {
                                    if (reader.TryReadNil())
                                       return default;

                                    var count = reader.ReadArrayHeader();

                                    if (count != 3)
                                       throw new global::MessagePack.MessagePackSerializationException($"Invalid member count. Expected 3 but found {count} field/property values.");

                                    global::MessagePack.IFormatterResolver resolver = options.Resolver;
                                    options.Security.DepthStep(ref reader);

                                    try
                                    {

                                       var referenceField = reader.ReadString()!;
                                       var structProperty = reader.ReadInt32()!;
                                       var nullableStructProperty = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<decimal?>(resolver).Deserialize(ref reader, options)!;

                                       var validationError = global::TestValueObject.Validate(
                                                                  referenceField,
                                                                  structProperty,
                                                                  nullableStructProperty,
                                                                  out var obj);

                                       if (validationError is not null)
                                          throw new global::MessagePack.MessagePackSerializationException(validationError.ToString() ?? "Unable to deserialize \"TestValueObject\".");

                                       return obj;
                                    }
                                    finally
                                    {
                                      reader.Depth--;
                                    }
                                 }

                                 /// <inheritdoc />
                                 public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::TestValueObject? value, global::MessagePack.MessagePackSerializerOptions options)
                                 {
                                    if(value is null)
                                    {
                                       writer.WriteNil();
                                       return;
                                    }

                                    writer.WriteArrayHeader(3);

                                    var resolver = options.Resolver;
                                    writer.Write(value.ReferenceField);
                                    writer.Write(value.StructProperty);
                                    global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<decimal?>(resolver).Serialize(ref writer, value.NullableStructProperty, options);
                                 }
                              }
                           }

                           """);
   }

   [Fact]
   public void Should_generate_MessagePackFormatter_and_attribute_for_complex_struct()
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

      AssertOutput(output, """
                           // <auto-generated />
                           #nullable enable

                           namespace Thinktecture.Tests;

                           [global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
                           partial struct TestValueObject
                           {
                              public sealed class ValueObjectMessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<global::Thinktecture.Tests.TestValueObject>
                              {
                                 /// <inheritdoc />
                                 public global::Thinktecture.Tests.TestValueObject Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
                                 {
                                    if (reader.TryReadNil())
                                       return default;

                                    var count = reader.ReadArrayHeader();

                                    if (count != 3)
                                       throw new global::MessagePack.MessagePackSerializationException($"Invalid member count. Expected 3 but found {count} field/property values.");

                                    global::MessagePack.IFormatterResolver resolver = options.Resolver;
                                    options.Security.DepthStep(ref reader);

                                    try
                                    {

                                       var referenceField = reader.ReadString()!;
                                       var structProperty = reader.ReadInt32()!;
                                       var nullableStructProperty = global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<decimal?>(resolver).Deserialize(ref reader, options)!;

                                       var validationError = global::Thinktecture.Tests.TestValueObject.Validate(
                                                                  referenceField,
                                                                  structProperty,
                                                                  nullableStructProperty,
                                                                  out var obj);

                                       if (validationError is not null)
                                          throw new global::MessagePack.MessagePackSerializationException(validationError.ToString() ?? "Unable to deserialize \"TestValueObject\".");

                                       return obj;
                                    }
                                    finally
                                    {
                                      reader.Depth--;
                                    }
                                 }

                                 /// <inheritdoc />
                                 public void Serialize(ref global::MessagePack.MessagePackWriter writer, global::Thinktecture.Tests.TestValueObject value, global::MessagePack.MessagePackSerializerOptions options)
                                 {
                                    writer.WriteArrayHeader(3);

                                    var resolver = options.Resolver;
                                    writer.Write(value.ReferenceField);
                                    writer.Write(value.StructProperty);
                                    global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<decimal?>(resolver).Serialize(ref writer, value.NullableStructProperty, options);
                                 }
                              }
                           }

                           """);
   }
}
