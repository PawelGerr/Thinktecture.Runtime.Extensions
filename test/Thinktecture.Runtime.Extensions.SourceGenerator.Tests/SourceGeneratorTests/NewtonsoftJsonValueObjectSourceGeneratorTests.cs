using Thinktecture.CodeAnalysis;
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
   public void Should_generate_NewtonsoftJsonConverter_and_Attribute_if_Attribute_is_missing()
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
                                                                  "_NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial class TestValueObject
   {
      public class ValueObjectNewtonsoftJsonConverter : global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<global::Thinktecture.Tests.TestValueObject, string>
      {
         public ValueObjectNewtonsoftJsonConverter()
            : base(global::Thinktecture.Tests.TestValueObject.Create, static obj => obj.ReferenceField)
         {
         }
      }
   }
}
");
   }

   [Fact]
   public void Should_generate_NewtonsoftJsonConverter_for_keyed_value_object_without_namespace()
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
                                                                  "_NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial class TestValueObject
   {
      public class ValueObjectNewtonsoftJsonConverter : global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<global::TestValueObject, string>
      {
         public ValueObjectNewtonsoftJsonConverter()
            : base(global::TestValueObject.Create, static obj => obj.ReferenceField)
         {
         }
      }
   }
");
   }

   [Fact]
   public void Should_generate_NewtonsoftJsonConverter_and_Attribute_for_struct_if_Attribute_is_missing()
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
                                                                  "_NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial struct TestValueObject
   {
      public class ValueObjectNewtonsoftJsonConverter : global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<global::Thinktecture.Tests.TestValueObject, string>
      {
         public ValueObjectNewtonsoftJsonConverter()
            : base(global::Thinktecture.Tests.TestValueObject.Create, static obj => obj.ReferenceField)
         {
         }
      }
   }
}
");
   }

   [Fact]
   public void Should_generate_NewtonsoftJsonConverter_and_Attribute_for_non_key_value_type_if_Attribute_is_missing()
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
                                                                  "_NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial class TestValueObject
   {
      public class ValueObjectNewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter
      {
         private static readonly global::System.Type _type = typeof(global::Thinktecture.Tests.TestValueObject);

         /// <inheritdoc />
         public override bool CanConvert(global::System.Type objectType)
         {
            return _type.IsAssignableFrom(objectType);
         }

         /// <inheritdoc />
         public override object? ReadJson(global::Newtonsoft.Json.JsonReader reader, global::System.Type objectType, object? existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
         {
            if (reader is null)
               throw new global::System.ArgumentNullException(nameof(reader));
            if (serializer is null)
               throw new global::System.ArgumentNullException(nameof(serializer));

            if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
            {
               if (objectType.IsClass || global::System.Nullable.GetUnderlyingType(objectType) == _type)
                  return null;

               return default(global::Thinktecture.Tests.TestValueObject);
            }

            if (reader.TokenType != global::Newtonsoft.Json.JsonToken.StartObject)
               throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.StartObject)}\""."");

            string? referenceField = default;
            int structProperty = default;
            decimal? nullableStructProperty = default;

            var comparer = global::System.StringComparer.OrdinalIgnoreCase;

            while (reader.Read())
            {
               if (reader.TokenType == global::Newtonsoft.Json.JsonToken.EndObject)
                  break;

               if (reader.TokenType != global::Newtonsoft.Json.JsonToken.PropertyName)
                  throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.PropertyName)}\""."");

               var propName = reader.Value!.ToString();

               if(!reader.Read())
                  throw new global::Newtonsoft.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""TestValueObject\""."");

               if (comparer.Equals(propName, ""referenceField""))
               {
                  referenceField = serializer.Deserialize<string>(reader);
               }
               else if (comparer.Equals(propName, ""structProperty""))
               {
                  structProperty = serializer.Deserialize<int>(reader);
               }
               else if (comparer.Equals(propName, ""nullableStructProperty""))
               {
                  nullableStructProperty = serializer.Deserialize<decimal?>(reader);
               }
               else
               {
                  throw new global::Newtonsoft.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""TestValueObject\""."");
               }
            }

            var validationResult = global::Thinktecture.Tests.TestValueObject.TryCreate(
                                       referenceField!,
                                       structProperty!,
                                       nullableStructProperty!,
                                       out var obj);

            if (validationResult != System.ComponentModel.DataAnnotations.ValidationResult.Success)
               throw new global::Newtonsoft.Json.JsonException($""Unable to deserialize \""TestValueObject\"". Error: {validationResult!.ErrorMessage}."");

            return obj;
         }

         /// <inheritdoc />
         public override void WriteJson(global::Newtonsoft.Json.JsonWriter writer, object? value, global::Newtonsoft.Json.JsonSerializer serializer)
         {
            if (value is null)
            {
               writer.WriteNull();
               return;
            }

            var obj = (global::Thinktecture.Tests.TestValueObject)value;
            var resolver = serializer.ContractResolver as global::Newtonsoft.Json.Serialization.DefaultContractResolver;

            writer.WriteStartObject();
            var referenceFieldPropertyValue = obj.ReferenceField;

            if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || referenceFieldPropertyValue is not null)
            {
               writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""ReferenceField"") : ""ReferenceField"");
               writer.WriteValue(referenceFieldPropertyValue);
            }
            var structPropertyPropertyValue = obj.StructProperty;

            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""StructProperty"") : ""StructProperty"");
            writer.WriteValue(structPropertyPropertyValue);
            var nullableStructPropertyPropertyValue = obj.NullableStructProperty;

            if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || nullableStructPropertyPropertyValue is not null)
            {
               writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""NullableStructProperty"") : ""NullableStructProperty"");
               serializer.Serialize(writer, nullableStructPropertyPropertyValue);
            }
            writer.WriteEndObject();
         }
      }
   }
}
");
   }

   [Fact]
   public void Should_generate_NewtonsoftJsonConverter_for_non_key_value_object_without_namespace()
   {
      var source = @"
using System;
using Thinktecture;

[ValueObject]
public partial class TestValueObject
{
   public readonly string ReferenceField;
   public int StructProperty { get; }
   public decimal? NullableStructProperty { get; }
}
";
      var output = GetGeneratedOutput<ValueObjectSourceGenerator>(source,
                                                                  "_NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial class TestValueObject
   {
      public class ValueObjectNewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter
      {
         private static readonly global::System.Type _type = typeof(global::TestValueObject);

         /// <inheritdoc />
         public override bool CanConvert(global::System.Type objectType)
         {
            return _type.IsAssignableFrom(objectType);
         }

         /// <inheritdoc />
         public override object? ReadJson(global::Newtonsoft.Json.JsonReader reader, global::System.Type objectType, object? existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
         {
            if (reader is null)
               throw new global::System.ArgumentNullException(nameof(reader));
            if (serializer is null)
               throw new global::System.ArgumentNullException(nameof(serializer));

            if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
            {
               if (objectType.IsClass || global::System.Nullable.GetUnderlyingType(objectType) == _type)
                  return null;

               return default(global::TestValueObject);
            }

            if (reader.TokenType != global::Newtonsoft.Json.JsonToken.StartObject)
               throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.StartObject)}\""."");

            string? referenceField = default;
            int structProperty = default;
            decimal? nullableStructProperty = default;

            var comparer = global::System.StringComparer.OrdinalIgnoreCase;

            while (reader.Read())
            {
               if (reader.TokenType == global::Newtonsoft.Json.JsonToken.EndObject)
                  break;

               if (reader.TokenType != global::Newtonsoft.Json.JsonToken.PropertyName)
                  throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.PropertyName)}\""."");

               var propName = reader.Value!.ToString();

               if(!reader.Read())
                  throw new global::Newtonsoft.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""TestValueObject\""."");

               if (comparer.Equals(propName, ""referenceField""))
               {
                  referenceField = serializer.Deserialize<string>(reader);
               }
               else if (comparer.Equals(propName, ""structProperty""))
               {
                  structProperty = serializer.Deserialize<int>(reader);
               }
               else if (comparer.Equals(propName, ""nullableStructProperty""))
               {
                  nullableStructProperty = serializer.Deserialize<decimal?>(reader);
               }
               else
               {
                  throw new global::Newtonsoft.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""TestValueObject\""."");
               }
            }

            var validationResult = global::TestValueObject.TryCreate(
                                       referenceField!,
                                       structProperty!,
                                       nullableStructProperty!,
                                       out var obj);

            if (validationResult != System.ComponentModel.DataAnnotations.ValidationResult.Success)
               throw new global::Newtonsoft.Json.JsonException($""Unable to deserialize \""TestValueObject\"". Error: {validationResult!.ErrorMessage}."");

            return obj;
         }

         /// <inheritdoc />
         public override void WriteJson(global::Newtonsoft.Json.JsonWriter writer, object? value, global::Newtonsoft.Json.JsonSerializer serializer)
         {
            if (value is null)
            {
               writer.WriteNull();
               return;
            }

            var obj = (global::TestValueObject)value;
            var resolver = serializer.ContractResolver as global::Newtonsoft.Json.Serialization.DefaultContractResolver;

            writer.WriteStartObject();
            var referenceFieldPropertyValue = obj.ReferenceField;

            if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || referenceFieldPropertyValue is not null)
            {
               writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""ReferenceField"") : ""ReferenceField"");
               writer.WriteValue(referenceFieldPropertyValue);
            }
            var structPropertyPropertyValue = obj.StructProperty;

            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""StructProperty"") : ""StructProperty"");
            writer.WriteValue(structPropertyPropertyValue);
            var nullableStructPropertyPropertyValue = obj.NullableStructProperty;

            if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || nullableStructPropertyPropertyValue is not null)
            {
               writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""NullableStructProperty"") : ""NullableStructProperty"");
               serializer.Serialize(writer, nullableStructPropertyPropertyValue);
            }
            writer.WriteEndObject();
         }
      }
   }
");
   }

   [Fact]
   public void Should_generate_NewtonsoftJsonConverter_and_Attribute_for_struct_for_non_key_value_type_if_Attribute_is_missing()
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
                                                                  "_NewtonsoftJson",
                                                                  typeof(ValueObjectAttribute).Assembly, typeof(Json.ValueObjectNewtonsoftJsonConverter).Assembly, typeof(Newtonsoft.Json.JsonToken).Assembly);

      AssertOutput(output, @"// <auto-generated />
#nullable enable

namespace Thinktecture.Tests
{
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial struct TestValueObject
   {
      public class ValueObjectNewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter
      {
         private static readonly global::System.Type _type = typeof(global::Thinktecture.Tests.TestValueObject);

         /// <inheritdoc />
         public override bool CanConvert(global::System.Type objectType)
         {
            return _type.IsAssignableFrom(objectType);
         }

         /// <inheritdoc />
         public override object? ReadJson(global::Newtonsoft.Json.JsonReader reader, global::System.Type objectType, object? existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
         {
            if (reader is null)
               throw new global::System.ArgumentNullException(nameof(reader));
            if (serializer is null)
               throw new global::System.ArgumentNullException(nameof(serializer));

            if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
            {
               if (objectType.IsClass || global::System.Nullable.GetUnderlyingType(objectType) == _type)
                  return null;

               return default(global::Thinktecture.Tests.TestValueObject);
            }

            if (reader.TokenType != global::Newtonsoft.Json.JsonToken.StartObject)
               throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.StartObject)}\""."");

            string? referenceField = default;
            int structProperty = default;
            decimal? nullableStructProperty = default;

            var comparer = global::System.StringComparer.OrdinalIgnoreCase;

            while (reader.Read())
            {
               if (reader.TokenType == global::Newtonsoft.Json.JsonToken.EndObject)
                  break;

               if (reader.TokenType != global::Newtonsoft.Json.JsonToken.PropertyName)
                  throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""TestValueObject\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.PropertyName)}\""."");

               var propName = reader.Value!.ToString();

               if(!reader.Read())
                  throw new global::Newtonsoft.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""TestValueObject\""."");

               if (comparer.Equals(propName, ""referenceField""))
               {
                  referenceField = serializer.Deserialize<string>(reader);
               }
               else if (comparer.Equals(propName, ""structProperty""))
               {
                  structProperty = serializer.Deserialize<int>(reader);
               }
               else if (comparer.Equals(propName, ""nullableStructProperty""))
               {
                  nullableStructProperty = serializer.Deserialize<decimal?>(reader);
               }
               else
               {
                  throw new global::Newtonsoft.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""TestValueObject\""."");
               }
            }

            var validationResult = global::Thinktecture.Tests.TestValueObject.TryCreate(
                                       referenceField!,
                                       structProperty!,
                                       nullableStructProperty!,
                                       out var obj);

            if (validationResult != System.ComponentModel.DataAnnotations.ValidationResult.Success)
               throw new global::Newtonsoft.Json.JsonException($""Unable to deserialize \""TestValueObject\"". Error: {validationResult!.ErrorMessage}."");

            return obj;
         }

         /// <inheritdoc />
         public override void WriteJson(global::Newtonsoft.Json.JsonWriter writer, object? value, global::Newtonsoft.Json.JsonSerializer serializer)
         {
            if (value is null)
            {
               writer.WriteNull();
               return;
            }

            var obj = (global::Thinktecture.Tests.TestValueObject)value;
            var resolver = serializer.ContractResolver as global::Newtonsoft.Json.Serialization.DefaultContractResolver;

            writer.WriteStartObject();
            var referenceFieldPropertyValue = obj.ReferenceField;

            if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || referenceFieldPropertyValue is not null)
            {
               writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""ReferenceField"") : ""ReferenceField"");
               writer.WriteValue(referenceFieldPropertyValue);
            }
            var structPropertyPropertyValue = obj.StructProperty;

            writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""StructProperty"") : ""StructProperty"");
            writer.WriteValue(structPropertyPropertyValue);
            var nullableStructPropertyPropertyValue = obj.NullableStructProperty;

            if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || nullableStructPropertyPropertyValue is not null)
            {
               writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""NullableStructProperty"") : ""NullableStructProperty"");
               serializer.Serialize(writer, nullableStructPropertyPropertyValue);
            }
            writer.WriteEndObject();
         }
      }
   }
}
");
   }
}
