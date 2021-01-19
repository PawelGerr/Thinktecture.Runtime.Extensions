using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   /// <summary>
   /// Source generator for JsonConverter for an enum-like class.
   /// </summary>
   [Generator]
   public class ThinktectureJsonConverterSourceGenerator : ThinktectureRuntimeExtensionsSourceGeneratorBase
   {
      /// <inheritdoc />
      protected override string GenerateEnum(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         return GenerateJsonConverter(state.EnumType, state.Namespace, state.EnumIdentifier, state.KeyType, "Get");
      }

      /// <inheritdoc />
      protected override string? GenerateValueType(ValueTypeSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         if (state.HasKeyMember)
            return GenerateJsonConverter(state.Type, state.Namespace, state.TypeIdentifier, state.KeyMember.Member.Type, "Create");

         if (!state.SkipFactoryMethods)
            return GenerateValueTypeJsonConverter(state);

         return null;
      }

      private static string GenerateJsonConverter(
         ITypeSymbol type,
         string? @namespace,
         SyntaxToken typeIdentifier,
         ITypeSymbol keyType,
         string factoryMethod)
      {
         if (type.HasAttribute("System.Text.Json.Serialization.JsonConverterAttribute"))
            return String.Empty;

         return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture;

{(String.IsNullOrWhiteSpace(@namespace) ? null : $"namespace {@namespace}")}
{{
   public class {typeIdentifier}_ValueTypeJsonConverterFactory : JsonConverterFactory
   {{
      /// <inheritdoc />
      public override bool CanConvert(Type typeToConvert)
      {{
         return typeof({typeIdentifier}).IsAssignableFrom(typeToConvert);
      }}

      /// <inheritdoc />
      public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
      {{
         if (typeToConvert is null)
            throw new ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new ArgumentNullException(nameof(options));

         return new Thinktecture.Text.Json.Serialization.ValueTypeJsonConverter<{typeIdentifier}, {keyType}>({typeIdentifier}.{factoryMethod}, obj => ({keyType}) obj, options);
      }}
   }}

   [System.Text.Json.Serialization.JsonConverterAttribute(typeof({typeIdentifier}_ValueTypeJsonConverterFactory))]
   partial class {typeIdentifier}
   {{
   }}
}}
";
      }

      private static string GenerateValueTypeJsonConverter(ValueTypeSourceGeneratorState state)
      {
         if (state.Type.HasAttribute("System.Text.Json.Serialization.JsonConverterAttribute"))
            return String.Empty;

         var sb = new StringBuilder($@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture.Text.Json.Serialization;

{(String.IsNullOrWhiteSpace(state.Namespace) ? null : $"namespace {state.Namespace}")}
{{
   public class {state.TypeIdentifier}_ValueTypeJsonConverter : JsonConverter<{state.TypeIdentifier}>
   {{");

         for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
         {
            var memberInfo = state.AssignableInstanceFieldsAndProperties[i];
            var needsConverter = GenerateWriteValue(null, memberInfo);

            if (needsConverter)
            {
               sb.Append(@$"
      private readonly JsonConverter<{memberInfo.Type}> _{memberInfo.ArgumentName}Converter;");
            }

            sb.Append(@$"
      private readonly string _{memberInfo.ArgumentName}PropertyName;");
         }

         sb.Append(@$"

      public {state.TypeIdentifier}_ValueTypeJsonConverter(JsonSerializerOptions options)
      {{
         if(options is null)
            throw new ArgumentNullException(nameof(options));

         var namingPolicy = options.PropertyNamingPolicy;
");

         for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
         {
            var memberInfo = state.AssignableInstanceFieldsAndProperties[i];
            var needsConverter = GenerateWriteValue(null, memberInfo);

            if (needsConverter)
            {
               sb.Append(@$"
         this._{memberInfo.ArgumentName}Converter = (JsonConverter<{memberInfo.Type}>)options.GetConverter(typeof({memberInfo.Type}));");
            }

            sb.Append(@$"
         this._{memberInfo.ArgumentName}PropertyName = namingPolicy?.ConvertName(""{memberInfo.Identifier}"") ?? ""{memberInfo.Identifier}"";");
         }

         sb.Append(@$"
      }}

      /// <inheritdoc />
      public override {state.TypeIdentifier}{state.NullableQuestionMark} Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
      {{
         if (reader.TokenType == JsonTokenType.Null)
            return default;

         if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException($""Unexpected token '{{reader.TokenType}}' when trying to deserialize '{state.TypeIdentifier}'. Expected token: '{{JsonTokenType.StartArray}}'."");
");

         for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
         {
            var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

            sb.Append(@$"
         {memberInfo.Type}{memberInfo.NullableQuestionMark} {memberInfo.ArgumentName} = default;");
         }

         sb.Append(@$"

         var comparer = options.PropertyNameCaseInsensitive ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;

         while (reader.Read())
         {{
            if (reader.TokenType == JsonTokenType.EndObject)
               break;

            if (reader.TokenType != JsonTokenType.PropertyName)
               throw new JsonException($""Unexpected token '{{reader.TokenType}}' when trying to deserialize '{state.TypeIdentifier}'. Expected token: '{{JsonTokenType.PropertyName}}'."");

            var propName = reader.GetString();

            if(!reader.Read())
               throw new JsonException($""Unexpected end of the JSON message when trying the read the value of '{{propName}}' during deserialization of '{state.TypeIdentifier}'."");
");

         for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
         {
            var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

            if (i == 0)
            {
               sb.Append(@$"
            if ");
            }
            else
            {
               sb.Append(@$"
            else if ");
            }

            sb.Append(@$"(comparer.Equals(propName, this._{memberInfo.ArgumentName}PropertyName))
            {{
               {memberInfo.ArgumentName} = {GenerateReadValue(memberInfo)};
            }}");
         }

         if (state.AssignableInstanceFieldsAndProperties.Count > 0)
         {
            sb.Append(@$"
            else
            {{
               throw new JsonException($""Unknown member '{{propName}}' encountered when trying to deserialize '{state.TypeIdentifier}'."");
            }}");
         }

         sb.Append(@$"
         }}

         var validationResult = {state.TypeIdentifier}.TryCreate(");

         for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
         {
            var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

            sb.Append(@$"
                                    {memberInfo.ArgumentName}!,");
         }

         sb.Append(@$"
                                    out var obj);

         if (validationResult != ValidationResult.Success)
            throw new JsonException($""Unable to deserialize '{state.TypeIdentifier}'. Error: {{validationResult!.ErrorMessage}}."");

         return obj;
      }}

      /// <inheritdoc />
      public override void Write(Utf8JsonWriter writer, {state.TypeIdentifier} value, JsonSerializerOptions options)
      {{
         writer.WriteStartObject();

         var ignoreNullValues = options.IgnoreNullValues;
");

         for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
         {
            var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

            sb.Append(@$"
         var {memberInfo.ArgumentName}PropertyValue = value.{memberInfo.Identifier};
");

            if (memberInfo.IsReferenceTypeOrNullableStruct)
            {
               sb.Append(@$"
         if(!ignoreNullValues || {memberInfo.ArgumentName}PropertyValue is not null)
         {{
            ");
            }
            else
            {
               sb.Append(@$"
         ");
            }

            sb.Append(@$"writer.WritePropertyName(this._{memberInfo.ArgumentName}PropertyName);
         ");

            if (memberInfo.IsReferenceTypeOrNullableStruct)
               sb.Append(@$"   ");

            GenerateWriteValue(sb, memberInfo);

            if (memberInfo.IsReferenceTypeOrNullableStruct)
               sb.Append(@$"
         }}");
         }

         sb.Append($@"
         writer.WriteEndObject();
      }}
   }}

   public class {state.TypeIdentifier}_ValueTypeJsonConverterFactory : JsonConverterFactory
   {{
      /// <inheritdoc />
      public override bool CanConvert(Type typeToConvert)
      {{
         return typeof({state.TypeIdentifier}).IsAssignableFrom(typeToConvert);
      }}

      /// <inheritdoc />
      public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
      {{
         if (typeToConvert is null)
            throw new ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new ArgumentNullException(nameof(options));

         return new {state.TypeIdentifier}_ValueTypeJsonConverter(options);
      }}
   }}

   [System.Text.Json.Serialization.JsonConverterAttribute(typeof({state.TypeIdentifier}_ValueTypeJsonConverterFactory))]
   partial class {state.TypeIdentifier}
   {{
   }}
}}
");

         return sb.ToString();
      }

      private static bool GenerateWriteValue(StringBuilder? sb, InstanceMemberInfo memberInfo)
      {
         var command = memberInfo.Type.SpecialType switch
         {
            SpecialType.System_Boolean => "WriteBooleanValue",

            SpecialType.System_String => "WriteStringValue",
            SpecialType.System_DateTime => "WriteStringValue",

            SpecialType.System_Byte => "WriteNumberValue",
            SpecialType.System_SByte => "WriteNumberValue",
            SpecialType.System_Int16 => "WriteNumberValue",
            SpecialType.System_UInt16 => "WriteNumberValue",
            SpecialType.System_Int32 => "WriteNumberValue",
            SpecialType.System_UInt32 => "WriteNumberValue",
            SpecialType.System_Int64 => "WriteNumberValue",
            SpecialType.System_UInt64 => "WriteNumberValue",
            SpecialType.System_Single => "WriteNumberValue",
            SpecialType.System_Double => "WriteNumberValue",
            SpecialType.System_Decimal => "WriteNumberValue",
            _ => null
         };

         if (command is null)
         {
            if (memberInfo.Type.Name == "System.Guid")
            {
               command = "WriteStringValue";
            }
            else if (memberInfo.Type.Name == "System.DateTimeOffset")
            {
               command = "WriteStringValue";
            }
            else
            {
               sb?.Append(@$"this._{memberInfo.ArgumentName}Converter.Write(writer, {memberInfo.ArgumentName}PropertyValue, options);");

               return true;
            }
         }

         sb?.Append("writer.").Append(command).Append($"({memberInfo.ArgumentName}PropertyValue);");

         return false;
      }

      private static string GenerateReadValue(InstanceMemberInfo memberInfo)
      {
         var command = memberInfo.Type.SpecialType switch
         {
            SpecialType.System_Boolean => "GetBoolean",

            SpecialType.System_String => "GetString",
            SpecialType.System_DateTime => "GetDateTime",

            SpecialType.System_Byte => "GetByte",
            SpecialType.System_SByte => "GetSByte",

            SpecialType.System_Int16 => "GetInt16",
            SpecialType.System_UInt16 => "GetUInt16",

            SpecialType.System_Int32 => "GetInt32",
            SpecialType.System_UInt32 => "GetUInt32",

            SpecialType.System_Int64 => "GetInt64",
            SpecialType.System_UInt64 => "GetUInt64",

            SpecialType.System_Single => "GetSingle",
            SpecialType.System_Double => "GetDouble",
            SpecialType.System_Decimal => "GetDecimal",
            _ => null
         };

         if (command is null)
         {
            if (memberInfo.Type.Name == "System.Guid")
            {
               command = "GetGuid";
            }
            else if (memberInfo.Type.Name == "System.DateTimeOffset")
            {
               command = "GetDateTimeOffset";
            }
            else
            {
               return @$"this._{memberInfo.ArgumentName}Converter.Read(ref reader, typeof({memberInfo.Type}), options)";
            }
         }

         return @$"reader.{command}()";
      }
   }
}
