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
   public class ThinktectureNewtonsoftJsonConverterSourceGenerator : ThinktectureRuntimeExtensionsSourceGeneratorBase
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
         if (type.HasAttribute("Newtonsoft.Json.JsonConverterAttribute"))
            return String.Empty;

         return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Thinktecture;

{(String.IsNullOrWhiteSpace(@namespace) ? null : $"namespace {@namespace}")}
{{
   public class {typeIdentifier}_ValueTypeNewtonsoftJsonConverter : Thinktecture.Json.ValueTypeNewtonsoftJsonConverter<{typeIdentifier}, {keyType}>
   {{
      public {typeIdentifier}_ValueTypeNewtonsoftJsonConverter()
         : base({typeIdentifier}.{factoryMethod}, obj => ({keyType}) obj)
      {{
      }}
   }}

   [Newtonsoft.Json.JsonConverterAttribute(typeof({typeIdentifier}_ValueTypeNewtonsoftJsonConverter))]
   partial class {typeIdentifier}
   {{
   }}
}}
";
      }

      private static string GenerateValueTypeJsonConverter(ValueTypeSourceGeneratorState state)
      {
         if (state.Type.HasAttribute("Newtonsoft.Json.JsonConverterAttribute"))
            return String.Empty;

         var sb = new StringBuilder($@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Thinktecture;

{(String.IsNullOrWhiteSpace(state.Namespace) ? null : $"namespace {state.Namespace}")}
{{
   public class {state.TypeIdentifier}_ValueTypeNewtonsoftJsonConverter : JsonConverter<{state.TypeIdentifier}{state.NullableQuestionMark}>
   {{");

         sb.Append(@$"
      /// <inheritdoc />
      public override {state.TypeIdentifier}{state.NullableQuestionMark} ReadJson(JsonReader reader, Type objectType, {state.TypeIdentifier}{state.NullableQuestionMark} existingValue, bool hasExistingValue, JsonSerializer serializer)
      {{
         if (reader is null)
            throw new ArgumentNullException(nameof(reader));
         if (serializer is null)
            throw new ArgumentNullException(nameof(serializer));

         if (reader.TokenType == JsonToken.Null)
            return default;

         if (reader.TokenType != JsonToken.StartObject)
            throw new JsonException($""Unexpected token '{{reader.TokenType}}' when trying to deserialize '{state.TypeIdentifier}'. Expected token: '{{JsonToken.StartObject}}'."");
");

         for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
         {
            var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

            sb.Append(@$"
         {memberInfo.Type}{memberInfo.NullableQuestionMark} {memberInfo.ArgumentName} = default;");
         }

         sb.Append(@$"

         var comparer = StringComparer.OrdinalIgnoreCase;

         while (reader.Read())
         {{
            if (reader.TokenType == JsonToken.EndObject)
               break;

            if (reader.TokenType != JsonToken.PropertyName)
               throw new JsonException($""Unexpected token '{{reader.TokenType}}' when trying to deserialize '{state.TypeIdentifier}'. Expected token: '{{JsonToken.PropertyName}}'."");

            var propName = reader.Value!.ToString();

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

            sb.Append(@$"(comparer.Equals(propName, ""{memberInfo.ArgumentName}""))
            {{
               {memberInfo.ArgumentName} = serializer.Deserialize<{memberInfo.Type}>(reader);
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
      public override void WriteJson(JsonWriter writer, {state.TypeIdentifier}{state.NullableQuestionMark} value, JsonSerializer serializer)
      {{");

         if (state.Type.IsReferenceType)
         {
            sb.Append(@$"
         if (value == null)
         {{
            writer.WriteNull();
            return;
         }}
");
         }

         sb.Append(@$"
         var resolver = serializer.ContractResolver as DefaultContractResolver;

         writer.WriteStartObject();");

         for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
         {
            var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

            sb.Append(@$"
         var {memberInfo.ArgumentName}PropertyValue = value.{memberInfo.Identifier};
");

            if (memberInfo.IsReferenceTypeOrNullableStruct)
            {
               sb.Append(@$"
         if(serializer.NullValueHandling != NullValueHandling.Ignore || {memberInfo.ArgumentName}PropertyValue is not null)
         {{
            ");
            }
            else
            {
               sb.Append(@$"
         ");
            }

            sb.Append(@$"writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""{memberInfo.Identifier}"") : ""{memberInfo.Identifier}"");
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

   [Newtonsoft.Json.JsonConverterAttribute(typeof({state.TypeIdentifier}_ValueTypeNewtonsoftJsonConverter))]
   partial class {state.TypeIdentifier}
   {{
   }}
}}
");

         return sb.ToString();
      }

      private static void GenerateWriteValue(StringBuilder? sb, InstanceMemberInfo memberInfo)
      {
         var command = memberInfo.Type.SpecialType switch
         {
            SpecialType.System_Boolean => "WriteValue",

            SpecialType.System_Char => "WriteValue",
            SpecialType.System_String => "WriteValue",
            SpecialType.System_DateTime => "WriteValue",

            SpecialType.System_Byte => "WriteValue",
            SpecialType.System_SByte => "WriteValue",
            SpecialType.System_Int16 => "WriteValue",
            SpecialType.System_UInt16 => "WriteValue",
            SpecialType.System_Int32 => "WriteValue",
            SpecialType.System_UInt32 => "WriteValue",
            SpecialType.System_Int64 => "WriteValue",
            SpecialType.System_UInt64 => "WriteValue",
            SpecialType.System_Single => "WriteValue",
            SpecialType.System_Double => "WriteValue",
            SpecialType.System_Decimal => "WriteValue",
            _ => null
         };

         if (command is null)
         {
            switch (memberInfo.Type.Name)
            {
               case "System.Guid":
               case "System.TimeSpan":
               case "System.DateTimeOffset":
                  command = "WriteValue";
                  break;

               default:
                  sb?.Append(@$"serializer.Serialize(writer, {memberInfo.ArgumentName}PropertyValue);");
                  return;
            }
         }

         sb?.Append("writer.").Append(command).Append($"({memberInfo.ArgumentName}PropertyValue);");
      }
   }
}
