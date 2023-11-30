using Thinktecture.CodeAnalysis.ValueObjects;
using Xunit.Abstractions;

namespace Thinktecture.Runtime.Tests.SourceGeneratorTests;

public class JsonValueObjectSourceGeneratorTests : SourceGeneratorTestsBase
{
   public JsonValueObjectSourceGeneratorTests(ITestOutputHelper output)
      : base(output)
   {
   }

   [Fact]
   public void Should_generate_JsonConverter_and_Attribute_if_Attribute_is_missing()
   {
      var source = @"
using System;
using Thinktecture;

namespace Thinktecture.Tests
{
   [ValueObject]
	public partial class TestValueObject
	{
      public readonly string ReferenceField;
   }
}
";
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, """
// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverterFactory<global::Thinktecture.Tests.TestValueObject, string>))]
partial class TestValueObject
{
}

""");
   }

   [Fact]
   public void Should_generate_JsonConverter_for_keyed_value_object_without_namespace()
   {
      var source = @"
using System;
using Thinktecture;

[ValueObject]
public partial class TestValueObject
{
   public readonly string ReferenceField;
}
";
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, """
// <auto-generated />
#nullable enable

[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverterFactory<global::TestValueObject, string>))]
partial class TestValueObject
{
}

""");
   }

   [Fact]
   public void Should_generate_JsonConverter_and_Attribute_for_struct_if_Attribute_is_missing()
   {
      var source = @"
using System;
using Thinktecture;

namespace Thinktecture.Tests
{
   [ValueObject]
	public readonly partial struct TestValueObject
	{
      public readonly string ReferenceField;
   }
}
";
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, """
// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverterFactory<global::Thinktecture.Tests.TestValueObject, string>))]
partial struct TestValueObject
{
}

""");
   }

   [Fact]
   public void Should_generate_JsonConverter_and_Attribute_for_non_key_value_type_if_Attribute_is_missing()
   {
      var source = @"
using System;
using Thinktecture;

namespace Thinktecture.Tests
{
   [ValueObject]
	public partial class TestValueObject
	{
      public readonly string ReferenceField;
      public int StructProperty { get; }
      public decimal? NullableStructProperty { get; }
   }
}
";
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
partial class TestValueObject
{
   public sealed class ValueObjectJsonConverter : global::System.Text.Json.Serialization.JsonConverter<global::Thinktecture.Tests.TestValueObject>
   {
      private readonly string _referenceFieldPropertyName;
      private readonly global::System.Text.Json.Serialization.JsonConverter<int> _structPropertyConverter;
      private readonly string _structPropertyPropertyName;
      private readonly global::System.Text.Json.Serialization.JsonConverter<decimal?> _nullableStructPropertyConverter;
      private readonly string _nullableStructPropertyPropertyName;

      public ValueObjectJsonConverter(global::System.Text.Json.JsonSerializerOptions options)
      {
         if(options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         var namingPolicy = options.PropertyNamingPolicy;

         this._referenceFieldPropertyName = namingPolicy?.ConvertName(""ReferenceField"") ?? ""ReferenceField"";
         this._structPropertyConverter = (global::System.Text.Json.Serialization.JsonConverter<int>)global::Thinktecture.JsonSerializerOptionsExtensions.GetCustomValueObjectMemberConverter(options, typeof(int));
         this._structPropertyPropertyName = namingPolicy?.ConvertName(""StructProperty"") ?? ""StructProperty"";
         this._nullableStructPropertyConverter = (global::System.Text.Json.Serialization.JsonConverter<decimal?>)global::Thinktecture.JsonSerializerOptionsExtensions.GetCustomValueObjectMemberConverter(options, typeof(decimal?));
         this._nullableStructPropertyPropertyName = namingPolicy?.ConvertName(""NullableStructProperty"") ?? ""NullableStructProperty"";
      }

      /// <inheritdoc />
      public override global::Thinktecture.Tests.TestValueObject? Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (reader.TokenType == global::System.Text.Json.JsonTokenType.Null)
            return default;

         if (reader.TokenType != global::System.Text.Json.JsonTokenType.StartObject)
            throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.StartObject)}\""."");

         string? referenceField = default;
         int structProperty = default;
         decimal? nullableStructProperty = default;

         var comparer = options.PropertyNameCaseInsensitive ? global::System.StringComparer.OrdinalIgnoreCase : global::System.StringComparer.Ordinal;

         while (reader.Read())
         {
            if (reader.TokenType == global::System.Text.Json.JsonTokenType.EndObject)
               break;

            if (reader.TokenType != global::System.Text.Json.JsonTokenType.PropertyName)
               throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.PropertyName)}\""."");

            var propName = reader.GetString();

            if(!reader.Read())
               throw new global::System.Text.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""TestValueObject\""."");

            if (comparer.Equals(propName, this._referenceFieldPropertyName))
            {
               referenceField = reader.GetString();
            }
            else if (comparer.Equals(propName, this._structPropertyPropertyName))
            {
               structProperty = this._structPropertyConverter.Read(ref reader, typeof(int), options);
            }
            else if (comparer.Equals(propName, this._nullableStructPropertyPropertyName))
            {
               nullableStructProperty = this._nullableStructPropertyConverter.Read(ref reader, typeof(decimal?), options);
            }
            else
            {
               throw new global::System.Text.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""TestValueObject\""."");
            }
         }

         var validationResult = global::Thinktecture.Tests.TestValueObject.Validate(
                                    referenceField!,
                                    structProperty!,
                                    nullableStructProperty!,
                                    out var obj);

         if (validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::System.Text.Json.JsonException($""Unable to deserialize \""TestValueObject\"". Error: {validationResult!.ErrorMessage}."");

         return obj;
      }

      /// <inheritdoc />
      public override void Write(global::System.Text.Json.Utf8JsonWriter writer, global::Thinktecture.Tests.TestValueObject value, global::System.Text.Json.JsonSerializerOptions options)
      {
         writer.WriteStartObject();

         var ignoreNullValues = options.DefaultIgnoreCondition is global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull or global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
         var ignoreDefaultValues = options.DefaultIgnoreCondition == global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;

         var referenceFieldPropertyValue = value.ReferenceField;

         if(!ignoreNullValues || referenceFieldPropertyValue is not null)
         {
            writer.WritePropertyName(this._referenceFieldPropertyName);
            writer.WriteStringValue(referenceFieldPropertyValue);
         }
         var structPropertyPropertyValue = value.StructProperty;

         if(!ignoreDefaultValues || !structPropertyPropertyValue.Equals(default(int)))
         {
            writer.WritePropertyName(this._structPropertyPropertyName);
            this._structPropertyConverter.Write(writer, structPropertyPropertyValue, options);
         }
         var nullableStructPropertyPropertyValue = value.NullableStructProperty;

         if(!ignoreNullValues || nullableStructPropertyPropertyValue is not null)
         {
            writer.WritePropertyName(this._nullableStructPropertyPropertyName);
            this._nullableStructPropertyConverter.Write(writer, nullableStructPropertyPropertyValue, options);
         }
         writer.WriteEndObject();
      }
   }

   public class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert(global::System.Type typeToConvert)
      {
         return typeof(global::Thinktecture.Tests.TestValueObject).IsAssignableFrom(typeToConvert);
      }

      /// <inheritdoc />
      public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (typeToConvert is null)
            throw new global::System.ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         return new ValueObjectJsonConverter(options);
      }
   }
}
");
   }

   [Fact]
   public void Should_generate_JsonConverter_and_Attribute_for_struct_for_non_key_value_type_if_Attribute_is_missing()
   {
      var source = @"
using System;
using Thinktecture;

namespace Thinktecture.Tests
{
   [ValueObject]
	public readonly partial struct TestValueObject
	{
      public readonly string ReferenceField;
      public int StructProperty { get; }
      public decimal? NullableStructProperty { get; }
   }
}
";
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
partial struct TestValueObject
{
   public sealed class ValueObjectJsonConverter : global::System.Text.Json.Serialization.JsonConverter<global::Thinktecture.Tests.TestValueObject>
   {
      private readonly string _referenceFieldPropertyName;
      private readonly global::System.Text.Json.Serialization.JsonConverter<int> _structPropertyConverter;
      private readonly string _structPropertyPropertyName;
      private readonly global::System.Text.Json.Serialization.JsonConverter<decimal?> _nullableStructPropertyConverter;
      private readonly string _nullableStructPropertyPropertyName;

      public ValueObjectJsonConverter(global::System.Text.Json.JsonSerializerOptions options)
      {
         if(options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         var namingPolicy = options.PropertyNamingPolicy;

         this._referenceFieldPropertyName = namingPolicy?.ConvertName(""ReferenceField"") ?? ""ReferenceField"";
         this._structPropertyConverter = (global::System.Text.Json.Serialization.JsonConverter<int>)global::Thinktecture.JsonSerializerOptionsExtensions.GetCustomValueObjectMemberConverter(options, typeof(int));
         this._structPropertyPropertyName = namingPolicy?.ConvertName(""StructProperty"") ?? ""StructProperty"";
         this._nullableStructPropertyConverter = (global::System.Text.Json.Serialization.JsonConverter<decimal?>)global::Thinktecture.JsonSerializerOptionsExtensions.GetCustomValueObjectMemberConverter(options, typeof(decimal?));
         this._nullableStructPropertyPropertyName = namingPolicy?.ConvertName(""NullableStructProperty"") ?? ""NullableStructProperty"";
      }

      /// <inheritdoc />
      public override global::Thinktecture.Tests.TestValueObject Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (reader.TokenType == global::System.Text.Json.JsonTokenType.Null)
            return default;

         if (reader.TokenType != global::System.Text.Json.JsonTokenType.StartObject)
            throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.StartObject)}\""."");

         string? referenceField = default;
         int structProperty = default;
         decimal? nullableStructProperty = default;

         var comparer = options.PropertyNameCaseInsensitive ? global::System.StringComparer.OrdinalIgnoreCase : global::System.StringComparer.Ordinal;

         while (reader.Read())
         {
            if (reader.TokenType == global::System.Text.Json.JsonTokenType.EndObject)
               break;

            if (reader.TokenType != global::System.Text.Json.JsonTokenType.PropertyName)
               throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.PropertyName)}\""."");

            var propName = reader.GetString();

            if(!reader.Read())
               throw new global::System.Text.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""TestValueObject\""."");

            if (comparer.Equals(propName, this._referenceFieldPropertyName))
            {
               referenceField = reader.GetString();
            }
            else if (comparer.Equals(propName, this._structPropertyPropertyName))
            {
               structProperty = this._structPropertyConverter.Read(ref reader, typeof(int), options);
            }
            else if (comparer.Equals(propName, this._nullableStructPropertyPropertyName))
            {
               nullableStructProperty = this._nullableStructPropertyConverter.Read(ref reader, typeof(decimal?), options);
            }
            else
            {
               throw new global::System.Text.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""TestValueObject\""."");
            }
         }

         var validationResult = global::Thinktecture.Tests.TestValueObject.Validate(
                                    referenceField!,
                                    structProperty!,
                                    nullableStructProperty!,
                                    out var obj);

         if (validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::System.Text.Json.JsonException($""Unable to deserialize \""TestValueObject\"". Error: {validationResult!.ErrorMessage}."");

         return obj;
      }

      /// <inheritdoc />
      public override void Write(global::System.Text.Json.Utf8JsonWriter writer, global::Thinktecture.Tests.TestValueObject value, global::System.Text.Json.JsonSerializerOptions options)
      {
         writer.WriteStartObject();

         var ignoreNullValues = options.DefaultIgnoreCondition is global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull or global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
         var ignoreDefaultValues = options.DefaultIgnoreCondition == global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;

         var referenceFieldPropertyValue = value.ReferenceField;

         if(!ignoreNullValues || referenceFieldPropertyValue is not null)
         {
            writer.WritePropertyName(this._referenceFieldPropertyName);
            writer.WriteStringValue(referenceFieldPropertyValue);
         }
         var structPropertyPropertyValue = value.StructProperty;

         if(!ignoreDefaultValues || !structPropertyPropertyValue.Equals(default(int)))
         {
            writer.WritePropertyName(this._structPropertyPropertyName);
            this._structPropertyConverter.Write(writer, structPropertyPropertyValue, options);
         }
         var nullableStructPropertyPropertyValue = value.NullableStructProperty;

         if(!ignoreNullValues || nullableStructPropertyPropertyValue is not null)
         {
            writer.WritePropertyName(this._nullableStructPropertyPropertyName);
            this._nullableStructPropertyConverter.Write(writer, nullableStructPropertyPropertyValue, options);
         }
         writer.WriteEndObject();
      }
   }

   public class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert(global::System.Type typeToConvert)
      {
         return typeof(global::Thinktecture.Tests.TestValueObject).IsAssignableFrom(typeToConvert);
      }

      /// <inheritdoc />
      public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (typeToConvert is null)
            throw new global::System.ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         return new ValueObjectJsonConverter(options);
      }
   }
}
");
   }

   [Fact]
   public void Should_generate_JsonConverter_non_key_value_object_if_Attribute_is_missing()
   {
      var source = @"
using System;
using Thinktecture;

namespace Thinktecture.Tests
{
   [ValueObject]
	public readonly partial struct TestValueObject
	{
      public readonly string ReferenceField;
      public int StructProperty { get; }
      public decimal? NullableStructProperty { get; }
   }
}
";
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

namespace Thinktecture.Tests;

[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
partial struct TestValueObject
{
   public sealed class ValueObjectJsonConverter : global::System.Text.Json.Serialization.JsonConverter<global::Thinktecture.Tests.TestValueObject>
   {
      private readonly string _referenceFieldPropertyName;
      private readonly global::System.Text.Json.Serialization.JsonConverter<int> _structPropertyConverter;
      private readonly string _structPropertyPropertyName;
      private readonly global::System.Text.Json.Serialization.JsonConverter<decimal?> _nullableStructPropertyConverter;
      private readonly string _nullableStructPropertyPropertyName;

      public ValueObjectJsonConverter(global::System.Text.Json.JsonSerializerOptions options)
      {
         if(options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         var namingPolicy = options.PropertyNamingPolicy;

         this._referenceFieldPropertyName = namingPolicy?.ConvertName(""ReferenceField"") ?? ""ReferenceField"";
         this._structPropertyConverter = (global::System.Text.Json.Serialization.JsonConverter<int>)global::Thinktecture.JsonSerializerOptionsExtensions.GetCustomValueObjectMemberConverter(options, typeof(int));
         this._structPropertyPropertyName = namingPolicy?.ConvertName(""StructProperty"") ?? ""StructProperty"";
         this._nullableStructPropertyConverter = (global::System.Text.Json.Serialization.JsonConverter<decimal?>)global::Thinktecture.JsonSerializerOptionsExtensions.GetCustomValueObjectMemberConverter(options, typeof(decimal?));
         this._nullableStructPropertyPropertyName = namingPolicy?.ConvertName(""NullableStructProperty"") ?? ""NullableStructProperty"";
      }

      /// <inheritdoc />
      public override global::Thinktecture.Tests.TestValueObject Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (reader.TokenType == global::System.Text.Json.JsonTokenType.Null)
            return default;

         if (reader.TokenType != global::System.Text.Json.JsonTokenType.StartObject)
            throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.StartObject)}\""."");

         string? referenceField = default;
         int structProperty = default;
         decimal? nullableStructProperty = default;

         var comparer = options.PropertyNameCaseInsensitive ? global::System.StringComparer.OrdinalIgnoreCase : global::System.StringComparer.Ordinal;

         while (reader.Read())
         {
            if (reader.TokenType == global::System.Text.Json.JsonTokenType.EndObject)
               break;

            if (reader.TokenType != global::System.Text.Json.JsonTokenType.PropertyName)
               throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.PropertyName)}\""."");

            var propName = reader.GetString();

            if(!reader.Read())
               throw new global::System.Text.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""TestValueObject\""."");

            if (comparer.Equals(propName, this._referenceFieldPropertyName))
            {
               referenceField = reader.GetString();
            }
            else if (comparer.Equals(propName, this._structPropertyPropertyName))
            {
               structProperty = this._structPropertyConverter.Read(ref reader, typeof(int), options);
            }
            else if (comparer.Equals(propName, this._nullableStructPropertyPropertyName))
            {
               nullableStructProperty = this._nullableStructPropertyConverter.Read(ref reader, typeof(decimal?), options);
            }
            else
            {
               throw new global::System.Text.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""TestValueObject\""."");
            }
         }

         var validationResult = global::Thinktecture.Tests.TestValueObject.Validate(
                                    referenceField!,
                                    structProperty!,
                                    nullableStructProperty!,
                                    out var obj);

         if (validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::System.Text.Json.JsonException($""Unable to deserialize \""TestValueObject\"". Error: {validationResult!.ErrorMessage}."");

         return obj;
      }

      /// <inheritdoc />
      public override void Write(global::System.Text.Json.Utf8JsonWriter writer, global::Thinktecture.Tests.TestValueObject value, global::System.Text.Json.JsonSerializerOptions options)
      {
         writer.WriteStartObject();

         var ignoreNullValues = options.DefaultIgnoreCondition is global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull or global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
         var ignoreDefaultValues = options.DefaultIgnoreCondition == global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;

         var referenceFieldPropertyValue = value.ReferenceField;

         if(!ignoreNullValues || referenceFieldPropertyValue is not null)
         {
            writer.WritePropertyName(this._referenceFieldPropertyName);
            writer.WriteStringValue(referenceFieldPropertyValue);
         }
         var structPropertyPropertyValue = value.StructProperty;

         if(!ignoreDefaultValues || !structPropertyPropertyValue.Equals(default(int)))
         {
            writer.WritePropertyName(this._structPropertyPropertyName);
            this._structPropertyConverter.Write(writer, structPropertyPropertyValue, options);
         }
         var nullableStructPropertyPropertyValue = value.NullableStructProperty;

         if(!ignoreNullValues || nullableStructPropertyPropertyValue is not null)
         {
            writer.WritePropertyName(this._nullableStructPropertyPropertyName);
            this._nullableStructPropertyConverter.Write(writer, nullableStructPropertyPropertyValue, options);
         }
         writer.WriteEndObject();
      }
   }

   public class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert(global::System.Type typeToConvert)
      {
         return typeof(global::Thinktecture.Tests.TestValueObject).IsAssignableFrom(typeToConvert);
      }

      /// <inheritdoc />
      public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (typeToConvert is null)
            throw new global::System.ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         return new ValueObjectJsonConverter(options);
      }
   }
}
");
   }

   [Fact]
   public void Should_generate_JsonConverter_for_non_key_value_object_without_namespace()
   {
      var source = @"
using System;
using Thinktecture;

[ValueObject]
public readonly partial struct TestValueObject
{
   public readonly string ReferenceField;
   public int StructProperty { get; }
   public decimal? NullableStructProperty { get; }
}
";
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  ".Json",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
partial struct TestValueObject
{
   public sealed class ValueObjectJsonConverter : global::System.Text.Json.Serialization.JsonConverter<global::TestValueObject>
   {
      private readonly string _referenceFieldPropertyName;
      private readonly global::System.Text.Json.Serialization.JsonConverter<int> _structPropertyConverter;
      private readonly string _structPropertyPropertyName;
      private readonly global::System.Text.Json.Serialization.JsonConverter<decimal?> _nullableStructPropertyConverter;
      private readonly string _nullableStructPropertyPropertyName;

      public ValueObjectJsonConverter(global::System.Text.Json.JsonSerializerOptions options)
      {
         if(options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         var namingPolicy = options.PropertyNamingPolicy;

         this._referenceFieldPropertyName = namingPolicy?.ConvertName(""ReferenceField"") ?? ""ReferenceField"";
         this._structPropertyConverter = (global::System.Text.Json.Serialization.JsonConverter<int>)global::Thinktecture.JsonSerializerOptionsExtensions.GetCustomValueObjectMemberConverter(options, typeof(int));
         this._structPropertyPropertyName = namingPolicy?.ConvertName(""StructProperty"") ?? ""StructProperty"";
         this._nullableStructPropertyConverter = (global::System.Text.Json.Serialization.JsonConverter<decimal?>)global::Thinktecture.JsonSerializerOptionsExtensions.GetCustomValueObjectMemberConverter(options, typeof(decimal?));
         this._nullableStructPropertyPropertyName = namingPolicy?.ConvertName(""NullableStructProperty"") ?? ""NullableStructProperty"";
      }

      /// <inheritdoc />
      public override global::TestValueObject Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (reader.TokenType == global::System.Text.Json.JsonTokenType.Null)
            return default;

         if (reader.TokenType != global::System.Text.Json.JsonTokenType.StartObject)
            throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.StartObject)}\""."");

         string? referenceField = default;
         int structProperty = default;
         decimal? nullableStructProperty = default;

         var comparer = options.PropertyNameCaseInsensitive ? global::System.StringComparer.OrdinalIgnoreCase : global::System.StringComparer.Ordinal;

         while (reader.Read())
         {
            if (reader.TokenType == global::System.Text.Json.JsonTokenType.EndObject)
               break;

            if (reader.TokenType != global::System.Text.Json.JsonTokenType.PropertyName)
               throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.PropertyName)}\""."");

            var propName = reader.GetString();

            if(!reader.Read())
               throw new global::System.Text.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""TestValueObject\""."");

            if (comparer.Equals(propName, this._referenceFieldPropertyName))
            {
               referenceField = reader.GetString();
            }
            else if (comparer.Equals(propName, this._structPropertyPropertyName))
            {
               structProperty = this._structPropertyConverter.Read(ref reader, typeof(int), options);
            }
            else if (comparer.Equals(propName, this._nullableStructPropertyPropertyName))
            {
               nullableStructProperty = this._nullableStructPropertyConverter.Read(ref reader, typeof(decimal?), options);
            }
            else
            {
               throw new global::System.Text.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""TestValueObject\""."");
            }
         }

         var validationResult = global::TestValueObject.Validate(
                                    referenceField!,
                                    structProperty!,
                                    nullableStructProperty!,
                                    out var obj);

         if (validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::System.Text.Json.JsonException($""Unable to deserialize \""TestValueObject\"". Error: {validationResult!.ErrorMessage}."");

         return obj;
      }

      /// <inheritdoc />
      public override void Write(global::System.Text.Json.Utf8JsonWriter writer, global::TestValueObject value, global::System.Text.Json.JsonSerializerOptions options)
      {
         writer.WriteStartObject();

         var ignoreNullValues = options.DefaultIgnoreCondition is global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull or global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
         var ignoreDefaultValues = options.DefaultIgnoreCondition == global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;

         var referenceFieldPropertyValue = value.ReferenceField;

         if(!ignoreNullValues || referenceFieldPropertyValue is not null)
         {
            writer.WritePropertyName(this._referenceFieldPropertyName);
            writer.WriteStringValue(referenceFieldPropertyValue);
         }
         var structPropertyPropertyValue = value.StructProperty;

         if(!ignoreDefaultValues || !structPropertyPropertyValue.Equals(default(int)))
         {
            writer.WritePropertyName(this._structPropertyPropertyName);
            this._structPropertyConverter.Write(writer, structPropertyPropertyValue, options);
         }
         var nullableStructPropertyPropertyValue = value.NullableStructProperty;

         if(!ignoreNullValues || nullableStructPropertyPropertyValue is not null)
         {
            writer.WritePropertyName(this._nullableStructPropertyPropertyName);
            this._nullableStructPropertyConverter.Write(writer, nullableStructPropertyPropertyValue, options);
         }
         writer.WriteEndObject();
      }
   }

   public class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert(global::System.Type typeToConvert)
      {
         return typeof(global::TestValueObject).IsAssignableFrom(typeToConvert);
      }

      /// <inheritdoc />
      public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (typeToConvert is null)
            throw new global::System.ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         return new ValueObjectJsonConverter(options);
      }
   }
}
");
   }
}
