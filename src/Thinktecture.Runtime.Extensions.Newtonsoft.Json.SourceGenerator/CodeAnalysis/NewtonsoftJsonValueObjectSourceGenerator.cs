using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for JsonConverter for Value Objects.
/// </summary>
[Generator]
public class NewtonsoftJsonValueObjectSourceGenerator : ValueObjectSourceGeneratorBase
{
   /// <inheritdoc />
   public NewtonsoftJsonValueObjectSourceGenerator()
      : base("_NewtonsoftJson")
   {
   }

   /// <inheritdoc />
   protected override string? GenerateValueObject(ValueObjectSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      if (state.HasKeyMember)
         return GenerateJsonConverter(state, state.KeyMember);

      if (!state.SkipFactoryMethods)
         return GenerateValueObjectJsonConverter(state, stringBuilderProvider.GetStringBuilder(8_000));

      return null;
   }

   private static string GenerateJsonConverter(ValueObjectSourceGeneratorState state, EqualityInstanceMemberInfo keyMember)
   {
      var type = state.Type;

      if (type.HasAttribute("Newtonsoft.Json.JsonConverterAttribute"))
         return String.Empty;

      var ns = state.Namespace;
      var typeName = state.Type.Name;

      return $@"{GENERATED_CODE_PREFIX}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Thinktecture;
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial {(type.IsValueType ? "struct" : "class")} {typeName}
   {{
      public class ValueObjectNewtonsoftJsonConverter : Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<{typeName}, {keyMember.Member.Type}>
      {{
         public ValueObjectNewtonsoftJsonConverter()
            : base({typeName}.Create, static obj => obj.{keyMember.Member.Identifier})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }

   private static string GenerateValueObjectJsonConverter(ValueObjectSourceGeneratorState state, StringBuilder sb)
   {
      if (state.Type.HasAttribute("Newtonsoft.Json.JsonConverterAttribute"))
         return String.Empty;

      sb.Append($@"{GENERATED_CODE_PREFIX}
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
using JsonException = Newtonsoft.Json.JsonException;
using JsonToken = Newtonsoft.Json.JsonToken;
{(state.Namespace is null ? null : $@"
namespace {state.Namespace}
{{")}
   [Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial {(state.Type.IsValueType ? "struct" : "class")} {state.Type.Name}
   {{
      public class ValueObjectNewtonsoftJsonConverter : Newtonsoft.Json.JsonConverter<{state.Type.Name}{state.NullableQuestionMark}>
      {{");

      sb.Append(@$"
         /// <inheritdoc />
         public override {state.Type.Name}{state.NullableQuestionMark} ReadJson(Newtonsoft.Json.JsonReader reader, System.Type objectType, {state.Type.Name}{state.NullableQuestionMark} existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
         {{
            if (reader is null)
               throw new System.ArgumentNullException(nameof(reader));
            if (serializer is null)
               throw new System.ArgumentNullException(nameof(serializer));

            if (reader.TokenType == JsonToken.Null)
               return default;

            if (reader.TokenType != JsonToken.StartObject)
               throw new JsonException($""Unexpected token '{{reader.TokenType}}' when trying to deserialize '{state.Type.Name}'. Expected token: '{{JsonToken.StartObject}}'."");
");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
            {memberInfo.Type}{memberInfo.NullableQuestionMark} {memberInfo.ArgumentName} = default;");
      }

      sb.Append(@$"

            var comparer = System.StringComparer.OrdinalIgnoreCase;

            while (reader.Read())
            {{
               if (reader.TokenType == JsonToken.EndObject)
                  break;

               if (reader.TokenType != JsonToken.PropertyName)
                  throw new JsonException($""Unexpected token '{{reader.TokenType}}' when trying to deserialize '{state.Type.Name}'. Expected token: '{{JsonToken.PropertyName}}'."");

               var propName = reader.Value!.ToString();

               if(!reader.Read())
                  throw new JsonException($""Unexpected end of the JSON message when trying the read the value of '{{propName}}' during deserialization of '{state.Type.Name}'."");
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
                  throw new JsonException($""Unknown member '{{propName}}' encountered when trying to deserialize '{state.Type.Name}'."");
               }}");
      }

      sb.Append(@$"
            }}

            var validationResult = {state.Type.Name}.TryCreate(");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
                                       {memberInfo.ArgumentName}!,");
      }

      sb.Append(@$"
                                       out var obj);

            if (validationResult != System.ComponentModel.DataAnnotations.ValidationResult.Success)
               throw new JsonException($""Unable to deserialize '{state.Type.Name}'. Error: {{validationResult!.ErrorMessage}}."");

            return obj;
         }}

         /// <inheritdoc />
         public override void WriteJson(Newtonsoft.Json.JsonWriter writer, {state.Type.Name}{state.NullableQuestionMark} value, Newtonsoft.Json.JsonSerializer serializer)
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
            var resolver = serializer.ContractResolver as Newtonsoft.Json.Serialization.DefaultContractResolver;

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
            if(serializer.NullValueHandling != Newtonsoft.Json.NullValueHandling.Ignore || {memberInfo.ArgumentName}PropertyValue is not null)
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
   }}
{(state.Namespace is null ? null : @"}
")}");

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
