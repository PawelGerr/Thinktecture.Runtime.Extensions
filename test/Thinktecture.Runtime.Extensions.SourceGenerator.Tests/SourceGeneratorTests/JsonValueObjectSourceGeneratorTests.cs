using Thinktecture.CodeAnalysis;
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
      var output = GetGeneratedOutput<JsonValueObjectSourceGenerator>(source, typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture;

namespace Thinktecture.Tests
{
   [System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial class TestValueObject
   {
      public class ValueObjectJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
      {
         /// <inheritdoc />
         public override bool CanConvert(System.Type typeToConvert)
         {
            return typeof(TestValueObject).IsAssignableFrom(typeToConvert);
         }

         /// <inheritdoc />
         public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
         {
            if (typeToConvert is null)
               throw new ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            return new Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<TestValueObject, string>(TestValueObject.Create, static obj => obj.ReferenceField, options);
         }
      }
   }
}
");
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
      var output = GetGeneratedOutput<JsonValueObjectSourceGenerator>(source, typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture;

   [System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial class TestValueObject
   {
      public class ValueObjectJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
      {
         /// <inheritdoc />
         public override bool CanConvert(System.Type typeToConvert)
         {
            return typeof(TestValueObject).IsAssignableFrom(typeToConvert);
         }

         /// <inheritdoc />
         public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
         {
            if (typeToConvert is null)
               throw new ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            return new Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<TestValueObject, string>(TestValueObject.Create, static obj => obj.ReferenceField, options);
         }
      }
   }
");
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
      var output = GetGeneratedOutput<JsonValueObjectSourceGenerator>(source, typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture;

namespace Thinktecture.Tests
{
   [System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial struct TestValueObject
   {
      public class ValueObjectJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
      {
         /// <inheritdoc />
         public override bool CanConvert(System.Type typeToConvert)
         {
            return typeof(TestValueObject).IsAssignableFrom(typeToConvert);
         }

         /// <inheritdoc />
         public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
         {
            if (typeToConvert is null)
               throw new ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            return new Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<TestValueObject, string>(TestValueObject.Create, static obj => obj.ReferenceField, options);
         }
      }
   }
}
");
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
      var output = GetGeneratedOutput<JsonValueObjectSourceGenerator>(source, typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Text.Json.Serialization;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using JsonTokenType = System.Text.Json.JsonTokenType;
using JsonException = System.Text.Json.JsonException;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using JsonIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition;

namespace Thinktecture.Tests
{
   [System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial class TestValueObject
   {
      public class ValueObjectJsonConverter : System.Text.Json.Serialization.JsonConverter<TestValueObject>
      {
         private readonly string _referenceFieldPropertyName;
         private readonly string _structPropertyPropertyName;
         private readonly System.Text.Json.Serialization.JsonConverter<decimal?> _nullableStructPropertyConverter;
         private readonly string _nullableStructPropertyPropertyName;

         public ValueObjectJsonConverter(JsonSerializerOptions options)
         {
            if(options is null)
               throw new ArgumentNullException(nameof(options));

            var namingPolicy = options.PropertyNamingPolicy;

            this._referenceFieldPropertyName = namingPolicy?.ConvertName(""ReferenceField"") ?? ""ReferenceField"";
            this._structPropertyPropertyName = namingPolicy?.ConvertName(""StructProperty"") ?? ""StructProperty"";
            this._nullableStructPropertyConverter = (System.Text.Json.Serialization.JsonConverter<decimal?>)options.GetConverter(typeof(decimal?));
            this._nullableStructPropertyPropertyName = namingPolicy?.ConvertName(""NullableStructProperty"") ?? ""NullableStructProperty"";
         }

         /// <inheritdoc />
         public override TestValueObject? Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
         {
            if (reader.TokenType == JsonTokenType.Null)
               return default;

            if (reader.TokenType != JsonTokenType.StartObject)
               throw new JsonException($""Unexpected token '{reader.TokenType}' when trying to deserialize 'TestValueObject'. Expected token: '{JsonTokenType.StartObject}'."");

            string? referenceField = default;
            int structProperty = default;
            decimal? nullableStructProperty = default;

            var comparer = options.PropertyNameCaseInsensitive ? System.StringComparer.OrdinalIgnoreCase : System.StringComparer.Ordinal;

            while (reader.Read())
            {
               if (reader.TokenType == JsonTokenType.EndObject)
                  break;

               if (reader.TokenType != JsonTokenType.PropertyName)
                  throw new JsonException($""Unexpected token '{reader.TokenType}' when trying to deserialize 'TestValueObject'. Expected token: '{JsonTokenType.PropertyName}'."");

               var propName = reader.GetString();

               if(!reader.Read())
                  throw new JsonException($""Unexpected end of the JSON message when trying the read the value of '{propName}' during deserialization of 'TestValueObject'."");

               if (comparer.Equals(propName, this._referenceFieldPropertyName))
               {
                  referenceField = reader.GetString();
               }
               else if (comparer.Equals(propName, this._structPropertyPropertyName))
               {
                  structProperty = reader.GetInt32();
               }
               else if (comparer.Equals(propName, this._nullableStructPropertyPropertyName))
               {
                  nullableStructProperty = this._nullableStructPropertyConverter.Read(ref reader, typeof(decimal?), options);
               }
               else
               {
                  throw new JsonException($""Unknown member '{propName}' encountered when trying to deserialize 'TestValueObject'."");
               }
            }

            var validationResult = TestValueObject.TryCreate(
                                       referenceField!,
                                       structProperty!,
                                       nullableStructProperty!,
                                       out var obj);

            if (validationResult != ValidationResult.Success)
               throw new JsonException($""Unable to deserialize 'TestValueObject'. Error: {validationResult!.ErrorMessage}."");

            return obj;
         }

         /// <inheritdoc />
         public override void Write(Utf8JsonWriter writer, TestValueObject value, JsonSerializerOptions options)
         {
            writer.WriteStartObject();

            var ignoreNullValues = options.DefaultIgnoreCondition is JsonIgnoreCondition.WhenWritingNull or JsonIgnoreCondition.WhenWritingDefault;
            var ignoreDefaultValues = options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault;

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
               writer.WriteNumberValue(structPropertyPropertyValue);
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

      public class ValueObjectJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
      {
         /// <inheritdoc />
         public override bool CanConvert(System.Type typeToConvert)
         {
            return typeof(TestValueObject).IsAssignableFrom(typeToConvert);
         }

         /// <inheritdoc />
         public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type typeToConvert, JsonSerializerOptions options)
         {
            if (typeToConvert is null)
               throw new ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            return new ValueObjectJsonConverter(options);
         }
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
      var output = GetGeneratedOutput<JsonValueObjectSourceGenerator>(source, typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Text.Json.Serialization;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using JsonTokenType = System.Text.Json.JsonTokenType;
using JsonException = System.Text.Json.JsonException;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using JsonIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition;

namespace Thinktecture.Tests
{
   [System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial struct TestValueObject
   {
      public class ValueObjectJsonConverter : System.Text.Json.Serialization.JsonConverter<TestValueObject>
      {
         private readonly string _referenceFieldPropertyName;
         private readonly string _structPropertyPropertyName;
         private readonly System.Text.Json.Serialization.JsonConverter<decimal?> _nullableStructPropertyConverter;
         private readonly string _nullableStructPropertyPropertyName;

         public ValueObjectJsonConverter(JsonSerializerOptions options)
         {
            if(options is null)
               throw new ArgumentNullException(nameof(options));

            var namingPolicy = options.PropertyNamingPolicy;

            this._referenceFieldPropertyName = namingPolicy?.ConvertName(""ReferenceField"") ?? ""ReferenceField"";
            this._structPropertyPropertyName = namingPolicy?.ConvertName(""StructProperty"") ?? ""StructProperty"";
            this._nullableStructPropertyConverter = (System.Text.Json.Serialization.JsonConverter<decimal?>)options.GetConverter(typeof(decimal?));
            this._nullableStructPropertyPropertyName = namingPolicy?.ConvertName(""NullableStructProperty"") ?? ""NullableStructProperty"";
         }

         /// <inheritdoc />
         public override TestValueObject Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
         {
            if (reader.TokenType == JsonTokenType.Null)
               return default;

            if (reader.TokenType != JsonTokenType.StartObject)
               throw new JsonException($""Unexpected token '{reader.TokenType}' when trying to deserialize 'TestValueObject'. Expected token: '{JsonTokenType.StartObject}'."");

            string? referenceField = default;
            int structProperty = default;
            decimal? nullableStructProperty = default;

            var comparer = options.PropertyNameCaseInsensitive ? System.StringComparer.OrdinalIgnoreCase : System.StringComparer.Ordinal;

            while (reader.Read())
            {
               if (reader.TokenType == JsonTokenType.EndObject)
                  break;

               if (reader.TokenType != JsonTokenType.PropertyName)
                  throw new JsonException($""Unexpected token '{reader.TokenType}' when trying to deserialize 'TestValueObject'. Expected token: '{JsonTokenType.PropertyName}'."");

               var propName = reader.GetString();

               if(!reader.Read())
                  throw new JsonException($""Unexpected end of the JSON message when trying the read the value of '{propName}' during deserialization of 'TestValueObject'."");

               if (comparer.Equals(propName, this._referenceFieldPropertyName))
               {
                  referenceField = reader.GetString();
               }
               else if (comparer.Equals(propName, this._structPropertyPropertyName))
               {
                  structProperty = reader.GetInt32();
               }
               else if (comparer.Equals(propName, this._nullableStructPropertyPropertyName))
               {
                  nullableStructProperty = this._nullableStructPropertyConverter.Read(ref reader, typeof(decimal?), options);
               }
               else
               {
                  throw new JsonException($""Unknown member '{propName}' encountered when trying to deserialize 'TestValueObject'."");
               }
            }

            var validationResult = TestValueObject.TryCreate(
                                       referenceField!,
                                       structProperty!,
                                       nullableStructProperty!,
                                       out var obj);

            if (validationResult != ValidationResult.Success)
               throw new JsonException($""Unable to deserialize 'TestValueObject'. Error: {validationResult!.ErrorMessage}."");

            return obj;
         }

         /// <inheritdoc />
         public override void Write(Utf8JsonWriter writer, TestValueObject value, JsonSerializerOptions options)
         {
            writer.WriteStartObject();

            var ignoreNullValues = options.DefaultIgnoreCondition is JsonIgnoreCondition.WhenWritingNull or JsonIgnoreCondition.WhenWritingDefault;
            var ignoreDefaultValues = options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault;

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
               writer.WriteNumberValue(structPropertyPropertyValue);
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

      public class ValueObjectJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
      {
         /// <inheritdoc />
         public override bool CanConvert(System.Type typeToConvert)
         {
            return typeof(TestValueObject).IsAssignableFrom(typeToConvert);
         }

         /// <inheritdoc />
         public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type typeToConvert, JsonSerializerOptions options)
         {
            if (typeToConvert is null)
               throw new ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            return new ValueObjectJsonConverter(options);
         }
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
      var output = GetGeneratedOutput<JsonValueObjectSourceGenerator>(source, typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Text.Json.Serialization;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using JsonTokenType = System.Text.Json.JsonTokenType;
using JsonException = System.Text.Json.JsonException;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using JsonIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition;

namespace Thinktecture.Tests
{
   [System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial struct TestValueObject
   {
      public class ValueObjectJsonConverter : System.Text.Json.Serialization.JsonConverter<TestValueObject>
      {
         private readonly string _referenceFieldPropertyName;
         private readonly string _structPropertyPropertyName;
         private readonly System.Text.Json.Serialization.JsonConverter<decimal?> _nullableStructPropertyConverter;
         private readonly string _nullableStructPropertyPropertyName;

         public ValueObjectJsonConverter(JsonSerializerOptions options)
         {
            if(options is null)
               throw new ArgumentNullException(nameof(options));

            var namingPolicy = options.PropertyNamingPolicy;

            this._referenceFieldPropertyName = namingPolicy?.ConvertName(""ReferenceField"") ?? ""ReferenceField"";
            this._structPropertyPropertyName = namingPolicy?.ConvertName(""StructProperty"") ?? ""StructProperty"";
            this._nullableStructPropertyConverter = (System.Text.Json.Serialization.JsonConverter<decimal?>)options.GetConverter(typeof(decimal?));
            this._nullableStructPropertyPropertyName = namingPolicy?.ConvertName(""NullableStructProperty"") ?? ""NullableStructProperty"";
         }

         /// <inheritdoc />
         public override TestValueObject Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
         {
            if (reader.TokenType == JsonTokenType.Null)
               return default;

            if (reader.TokenType != JsonTokenType.StartObject)
               throw new JsonException($""Unexpected token '{reader.TokenType}' when trying to deserialize 'TestValueObject'. Expected token: '{JsonTokenType.StartObject}'."");

            string? referenceField = default;
            int structProperty = default;
            decimal? nullableStructProperty = default;

            var comparer = options.PropertyNameCaseInsensitive ? System.StringComparer.OrdinalIgnoreCase : System.StringComparer.Ordinal;

            while (reader.Read())
            {
               if (reader.TokenType == JsonTokenType.EndObject)
                  break;

               if (reader.TokenType != JsonTokenType.PropertyName)
                  throw new JsonException($""Unexpected token '{reader.TokenType}' when trying to deserialize 'TestValueObject'. Expected token: '{JsonTokenType.PropertyName}'."");

               var propName = reader.GetString();

               if(!reader.Read())
                  throw new JsonException($""Unexpected end of the JSON message when trying the read the value of '{propName}' during deserialization of 'TestValueObject'."");

               if (comparer.Equals(propName, this._referenceFieldPropertyName))
               {
                  referenceField = reader.GetString();
               }
               else if (comparer.Equals(propName, this._structPropertyPropertyName))
               {
                  structProperty = reader.GetInt32();
               }
               else if (comparer.Equals(propName, this._nullableStructPropertyPropertyName))
               {
                  nullableStructProperty = this._nullableStructPropertyConverter.Read(ref reader, typeof(decimal?), options);
               }
               else
               {
                  throw new JsonException($""Unknown member '{propName}' encountered when trying to deserialize 'TestValueObject'."");
               }
            }

            var validationResult = TestValueObject.TryCreate(
                                       referenceField!,
                                       structProperty!,
                                       nullableStructProperty!,
                                       out var obj);

            if (validationResult != ValidationResult.Success)
               throw new JsonException($""Unable to deserialize 'TestValueObject'. Error: {validationResult!.ErrorMessage}."");

            return obj;
         }

         /// <inheritdoc />
         public override void Write(Utf8JsonWriter writer, TestValueObject value, JsonSerializerOptions options)
         {
            writer.WriteStartObject();

            var ignoreNullValues = options.DefaultIgnoreCondition is JsonIgnoreCondition.WhenWritingNull or JsonIgnoreCondition.WhenWritingDefault;
            var ignoreDefaultValues = options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault;

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
               writer.WriteNumberValue(structPropertyPropertyValue);
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

      public class ValueObjectJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
      {
         /// <inheritdoc />
         public override bool CanConvert(System.Type typeToConvert)
         {
            return typeof(TestValueObject).IsAssignableFrom(typeToConvert);
         }

         /// <inheritdoc />
         public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type typeToConvert, JsonSerializerOptions options)
         {
            if (typeToConvert is null)
               throw new ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            return new ValueObjectJsonConverter(options);
         }
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
      var output = GetGeneratedOutput<JsonValueObjectSourceGenerator>(source, typeof(ValueObjectAttribute).Assembly, typeof(Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<,>).Assembly, typeof(System.Text.Json.JsonDocument).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Text.Json.Serialization;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;
using JsonTokenType = System.Text.Json.JsonTokenType;
using JsonException = System.Text.Json.JsonException;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;
using JsonIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition;

   [System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial struct TestValueObject
   {
      public class ValueObjectJsonConverter : System.Text.Json.Serialization.JsonConverter<TestValueObject>
      {
         private readonly string _referenceFieldPropertyName;
         private readonly string _structPropertyPropertyName;
         private readonly System.Text.Json.Serialization.JsonConverter<decimal?> _nullableStructPropertyConverter;
         private readonly string _nullableStructPropertyPropertyName;

         public ValueObjectJsonConverter(JsonSerializerOptions options)
         {
            if(options is null)
               throw new ArgumentNullException(nameof(options));

            var namingPolicy = options.PropertyNamingPolicy;

            this._referenceFieldPropertyName = namingPolicy?.ConvertName(""ReferenceField"") ?? ""ReferenceField"";
            this._structPropertyPropertyName = namingPolicy?.ConvertName(""StructProperty"") ?? ""StructProperty"";
            this._nullableStructPropertyConverter = (System.Text.Json.Serialization.JsonConverter<decimal?>)options.GetConverter(typeof(decimal?));
            this._nullableStructPropertyPropertyName = namingPolicy?.ConvertName(""NullableStructProperty"") ?? ""NullableStructProperty"";
         }

         /// <inheritdoc />
         public override TestValueObject Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
         {
            if (reader.TokenType == JsonTokenType.Null)
               return default;

            if (reader.TokenType != JsonTokenType.StartObject)
               throw new JsonException($""Unexpected token '{reader.TokenType}' when trying to deserialize 'TestValueObject'. Expected token: '{JsonTokenType.StartObject}'."");

            string? referenceField = default;
            int structProperty = default;
            decimal? nullableStructProperty = default;

            var comparer = options.PropertyNameCaseInsensitive ? System.StringComparer.OrdinalIgnoreCase : System.StringComparer.Ordinal;

            while (reader.Read())
            {
               if (reader.TokenType == JsonTokenType.EndObject)
                  break;

               if (reader.TokenType != JsonTokenType.PropertyName)
                  throw new JsonException($""Unexpected token '{reader.TokenType}' when trying to deserialize 'TestValueObject'. Expected token: '{JsonTokenType.PropertyName}'."");

               var propName = reader.GetString();

               if(!reader.Read())
                  throw new JsonException($""Unexpected end of the JSON message when trying the read the value of '{propName}' during deserialization of 'TestValueObject'."");

               if (comparer.Equals(propName, this._referenceFieldPropertyName))
               {
                  referenceField = reader.GetString();
               }
               else if (comparer.Equals(propName, this._structPropertyPropertyName))
               {
                  structProperty = reader.GetInt32();
               }
               else if (comparer.Equals(propName, this._nullableStructPropertyPropertyName))
               {
                  nullableStructProperty = this._nullableStructPropertyConverter.Read(ref reader, typeof(decimal?), options);
               }
               else
               {
                  throw new JsonException($""Unknown member '{propName}' encountered when trying to deserialize 'TestValueObject'."");
               }
            }

            var validationResult = TestValueObject.TryCreate(
                                       referenceField!,
                                       structProperty!,
                                       nullableStructProperty!,
                                       out var obj);

            if (validationResult != ValidationResult.Success)
               throw new JsonException($""Unable to deserialize 'TestValueObject'. Error: {validationResult!.ErrorMessage}."");

            return obj;
         }

         /// <inheritdoc />
         public override void Write(Utf8JsonWriter writer, TestValueObject value, JsonSerializerOptions options)
         {
            writer.WriteStartObject();

            var ignoreNullValues = options.DefaultIgnoreCondition is JsonIgnoreCondition.WhenWritingNull or JsonIgnoreCondition.WhenWritingDefault;
            var ignoreDefaultValues = options.DefaultIgnoreCondition == JsonIgnoreCondition.WhenWritingDefault;

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
               writer.WriteNumberValue(structPropertyPropertyValue);
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

      public class ValueObjectJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
      {
         /// <inheritdoc />
         public override bool CanConvert(System.Type typeToConvert)
         {
            return typeof(TestValueObject).IsAssignableFrom(typeToConvert);
         }

         /// <inheritdoc />
         public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type typeToConvert, JsonSerializerOptions options)
         {
            if (typeToConvert is null)
               throw new ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            return new ValueObjectJsonConverter(options);
         }
      }
   }
");
   }
}
