using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class JsonValueObjectCodeGenerator : CodeGeneratorBase
{
   private readonly ValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _stringBuilder;

   public override string FileNameSuffix => ".Json";

   public JsonValueObjectCodeGenerator(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _stringBuilder = stringBuilder;
   }

   public override string? Generate()
   {
      if (_state.AttributeInfo.HasJsonConverterAttribute)
         return null;

      if (_state.HasKeyMember)
         return GenerateJsonConverter(_state, _state.KeyMember);

      if (!_state.Settings.SkipFactoryMethods)
         return GenerateValueObjectJsonConverter(_state, _stringBuilder);

      return null;
   }

   private static string GenerateJsonConverter(
      ValueObjectSourceGeneratorState state,
      EqualityInstanceMemberInfo keyMember)
   {
      return $$"""
{{GENERATED_CODE_PREFIX}}
{{(state.Namespace is null ? null : $@"
namespace {state.Namespace};
")}}
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverterFactory<{{state.TypeFullyQualified}}, {{keyMember.Member.TypeFullyQualified}}>))]
partial {{(state.IsReferenceType ? "class" : "struct")}} {{state.Name}}
{
}

""";
   }

   private static string GenerateValueObjectJsonConverter(ValueObjectSourceGeneratorState state, StringBuilder sb)
   {
      sb.Append($@"{GENERATED_CODE_PREFIX}
{(state.Namespace is null ? null : $@"
namespace {state.Namespace};
")}
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
{{
   public sealed class ValueObjectJsonConverter : global::System.Text.Json.Serialization.JsonConverter<{state.TypeFullyQualified}>
   {{");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];
         var needsConverter = GenerateWriteValue(null, memberInfo);

         if (needsConverter)
         {
            sb.Append(@$"
      private readonly global::System.Text.Json.Serialization.JsonConverter<{memberInfo.TypeFullyQualifiedWithNullability}> _{memberInfo.ArgumentName}Converter;");
         }

         sb.Append(@$"
      private readonly string _{memberInfo.ArgumentName}PropertyName;");
      }

      sb.Append(@"

      public ValueObjectJsonConverter(global::System.Text.Json.JsonSerializerOptions options)
      {
         if(options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         var namingPolicy = options.PropertyNamingPolicy;
");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];
         var needsConverter = GenerateWriteValue(null, memberInfo);

         if (needsConverter)
         {
            sb.Append(@$"
         this._{memberInfo.ArgumentName}Converter = (global::System.Text.Json.Serialization.JsonConverter<{memberInfo.TypeFullyQualifiedWithNullability}>)options.GetConverter(typeof({memberInfo.TypeFullyQualifiedWithNullability}));");
         }

         sb.Append(@$"
         this._{memberInfo.ArgumentName}PropertyName = namingPolicy?.ConvertName(""{memberInfo.Name}"") ?? ""{memberInfo.Name}"";");
      }

      sb.Append(@$"
      }}

      /// <inheritdoc />
      public override {state.TypeFullyQualifiedNullAnnotated} Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {{
         if (reader.TokenType == global::System.Text.Json.JsonTokenType.Null)
            return default;

         if (reader.TokenType != global::System.Text.Json.JsonTokenType.StartObject)
            throw new global::System.Text.Json.JsonException($""Unexpected token \""{{reader.TokenType}}\"" when trying to deserialize \""{state.TypeMinimallyQualified}\"". Expected token: \""{{(global::System.Text.Json.JsonTokenType.StartObject)}}\""."");
");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
         {memberInfo.TypeFullyQualifiedNullAnnotated} {memberInfo.ArgumentName} = default;");
      }

      sb.Append(@$"

         var comparer = options.PropertyNameCaseInsensitive ? global::System.StringComparer.OrdinalIgnoreCase : global::System.StringComparer.Ordinal;

         while (reader.Read())
         {{
            if (reader.TokenType == global::System.Text.Json.JsonTokenType.EndObject)
               break;

            if (reader.TokenType != global::System.Text.Json.JsonTokenType.PropertyName)
               throw new global::System.Text.Json.JsonException($""Unexpected token \""{{reader.TokenType}}\"" when trying to deserialize \""{state.TypeMinimallyQualified}\"". Expected token: \""{{(global::System.Text.Json.JsonTokenType.PropertyName)}}\""."");

            var propName = reader.GetString();

            if(!reader.Read())
               throw new global::System.Text.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{{propName}}\"" during deserialization of \""{state.TypeMinimallyQualified}\""."");
");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         if (i == 0)
         {
            sb.Append(@"
            if ");
         }
         else
         {
            sb.Append(@"
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
               throw new global::System.Text.Json.JsonException($""Unknown member \""{{propName}}\"" encountered when trying to deserialize \""{state.TypeMinimallyQualified}\""."");
            }}");
      }

      sb.Append(@$"
         }}

         var validationResult = {state.TypeFullyQualified}.Validate(");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
                                    {memberInfo.ArgumentName}!,");
      }

      sb.Append(@$"
                                    out var obj);

         if (validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::System.Text.Json.JsonException($""Unable to deserialize \""{state.Name}\"". Error: {{validationResult!.ErrorMessage}}."");

         return obj;
      }}

      /// <inheritdoc />
      public override void Write(global::System.Text.Json.Utf8JsonWriter writer, {state.TypeFullyQualified} value, global::System.Text.Json.JsonSerializerOptions options)
      {{
         writer.WriteStartObject();

         var ignoreNullValues = options.DefaultIgnoreCondition is global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull or global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
         var ignoreDefaultValues = options.DefaultIgnoreCondition == global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
         var {memberInfo.ArgumentName}PropertyValue = value.{memberInfo.Name};
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
         if(!ignoreDefaultValues || !{memberInfo.ArgumentName}PropertyValue.Equals(default({memberInfo.TypeFullyQualifiedWithNullability})))
         {{
            ");
         }

         sb.Append(@$"writer.WritePropertyName(this._{memberInfo.ArgumentName}PropertyName);
         ");

         sb.Append(@"   ");

         GenerateWriteValue(sb, memberInfo);

         sb.Append(@"
         }");
      }

      sb.Append($@"
         writer.WriteEndObject();
      }}
   }}

   public class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
   {{
      /// <inheritdoc />
      public override bool CanConvert(global::System.Type typeToConvert)
      {{
         return typeof({state.TypeFullyQualified}).IsAssignableFrom(typeToConvert);
      }}

      /// <inheritdoc />
      public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {{
         if (typeToConvert is null)
            throw new global::System.ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         return new ValueObjectJsonConverter(options);
      }}
   }}
}}
");

      return sb.ToString();
   }

   private static bool GenerateWriteValue(StringBuilder? sb, InstanceMemberInfo memberInfo)
   {
      var command = memberInfo.SpecialType switch
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
         if (memberInfo.TypeFullyQualified == "global::System.Guid")
         {
            command = "WriteStringValue";
         }
         else if (memberInfo.TypeFullyQualified == "global::System.DateTimeOffset")
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
      var command = memberInfo.SpecialType switch
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
         if (memberInfo.TypeFullyQualified == "global::System.Guid")
         {
            command = "GetGuid";
         }
         else if (memberInfo.TypeFullyQualified == "global::System.DateTimeOffset")
         {
            command = "GetDateTimeOffset";
         }
         else
         {
            return @$"this._{memberInfo.ArgumentName}Converter.Read(ref reader, typeof({memberInfo.TypeFullyQualifiedWithNullability}), options)";
         }
      }

      return @$"reader.{command}()";
   }
}
