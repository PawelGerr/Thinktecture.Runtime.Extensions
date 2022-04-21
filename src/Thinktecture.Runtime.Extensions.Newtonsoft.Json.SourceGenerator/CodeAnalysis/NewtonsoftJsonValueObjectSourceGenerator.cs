using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

[Generator]
public class NewtonsoftJsonValueObjectSourceGenerator : ValueObjectSourceGeneratorBase<NewtonsoftJsonValueObjectSourceGeneratorState>
{
   /// <inheritdoc />
   public NewtonsoftJsonValueObjectSourceGenerator()
      : base("_NewtonsoftJson")
   {
   }

   protected override NewtonsoftJsonValueObjectSourceGeneratorState CreateState(INamedTypeSymbol type, AttributeData valueObjectAttribute)
   {
      return new NewtonsoftJsonValueObjectSourceGeneratorState(type, valueObjectAttribute);
   }

   protected override string? GenerateValueObject(NewtonsoftJsonValueObjectSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      if (state.HasKeyMember)
         return GenerateJsonConverter(state, state.KeyMember);

      if (!state.Settings.SkipFactoryMethods)
         return GenerateValueObjectJsonConverter(state, stringBuilderProvider.GetStringBuilder(8_000));

      return null;
   }

   private static string GenerateJsonConverter(NewtonsoftJsonValueObjectSourceGeneratorState state, EqualityInstanceMemberInfo keyMember)
   {
      if (state.HasJsonConverterAttribute)
         return String.Empty;

      var ns = state.Namespace;

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
   {{
      public class ValueObjectNewtonsoftJsonConverter : global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<{state.TypeFullyQualified}, {keyMember.Member.TypeFullyQualifiedWithNullability}>
      {{
         public ValueObjectNewtonsoftJsonConverter()
            : base({state.TypeFullyQualified}.Create, static obj => obj.{keyMember.Member.Name})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }

   private static string GenerateValueObjectJsonConverter(NewtonsoftJsonValueObjectSourceGeneratorState state, StringBuilder sb)
   {
      if (state.HasJsonConverterAttribute)
         return String.Empty;

      sb.Append($@"{GENERATED_CODE_PREFIX}
{(state.Namespace is null ? null : $@"
namespace {state.Namespace}
{{")}
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
   {{
      public class ValueObjectNewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter
      {{
         private static readonly global::System.Type _type = typeof({state.TypeFullyQualified});

         /// <inheritdoc />
         public override bool CanConvert(global::System.Type objectType)
         {{
            return _type.IsAssignableFrom(objectType);
         }}

         /// <inheritdoc />
         public override object? ReadJson(global::Newtonsoft.Json.JsonReader reader, global::System.Type objectType, object? existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
         {{
            if (reader is null)
               throw new global::System.ArgumentNullException(nameof(reader));
            if (serializer is null)
               throw new global::System.ArgumentNullException(nameof(serializer));

            if (reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
            {{
               if (objectType.IsClass || global::System.Nullable.GetUnderlyingType(objectType) == _type)
                  return null;

               return default({state.TypeFullyQualified});
            }}

            if (reader.TokenType != global::Newtonsoft.Json.JsonToken.StartObject)
               throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{{reader.TokenType}}\"" when trying to deserialize \""{state.TypeMinimallyQualified}\"". Expected token: \""{{(global::Newtonsoft.Json.JsonToken.StartObject)}}\""."");
");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
            {memberInfo.TypeFullyQualified}{memberInfo.NullableQuestionMark} {memberInfo.ArgumentName} = default;");
      }

      sb.Append(@$"

            var comparer = global::System.StringComparer.OrdinalIgnoreCase;

            while (reader.Read())
            {{
               if (reader.TokenType == global::Newtonsoft.Json.JsonToken.EndObject)
                  break;

               if (reader.TokenType != global::Newtonsoft.Json.JsonToken.PropertyName)
                  throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{{reader.TokenType}}\"" when trying to deserialize \""{state.TypeMinimallyQualified}\"". Expected token: \""{{(global::Newtonsoft.Json.JsonToken.PropertyName)}}\""."");

               var propName = reader.Value!.ToString();

               if(!reader.Read())
                  throw new global::Newtonsoft.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{{propName}}\"" during deserialization of \""{state.TypeMinimallyQualified}\""."");
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

         sb.Append(@$"(comparer.Equals(propName, ""{memberInfo.ArgumentName}""))
               {{
                  {memberInfo.ArgumentName} = serializer.Deserialize<{memberInfo.TypeFullyQualifiedWithNullability}>(reader);
               }}");
      }

      if (state.AssignableInstanceFieldsAndProperties.Count > 0)
      {
         sb.Append(@$"
               else
               {{
                  throw new global::Newtonsoft.Json.JsonException($""Unknown member \""{{propName}}\"" encountered when trying to deserialize \""{state.TypeMinimallyQualified}\""."");
               }}");
      }

      sb.Append(@$"
            }}

            var validationResult = {state.TypeFullyQualified}.TryCreate(");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
                                       {memberInfo.ArgumentName}!,");
      }

      sb.Append(@$"
                                       out var obj);

            if (validationResult != System.ComponentModel.DataAnnotations.ValidationResult.Success)
               throw new global::Newtonsoft.Json.JsonException($""Unable to deserialize \""{state.TypeMinimallyQualified}\"". Error: {{validationResult!.ErrorMessage}}."");

            return obj;
         }}

         /// <inheritdoc />
         public override void WriteJson(global::Newtonsoft.Json.JsonWriter writer, object? value, global::Newtonsoft.Json.JsonSerializer serializer)
         {{
            if (value is null)
            {{
               writer.WriteNull();
               return;
            }}

            var obj = ({state.TypeFullyQualified})value;
            var resolver = serializer.ContractResolver as global::Newtonsoft.Json.Serialization.DefaultContractResolver;

            writer.WriteStartObject();");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
            var {memberInfo.ArgumentName}PropertyValue = obj.{memberInfo.Name};
");

         if (memberInfo.IsReferenceTypeOrNullableStruct)
         {
            sb.Append(@$"
            if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || {memberInfo.ArgumentName}PropertyValue is not null)
            {{
               ");
         }
         else
         {
            sb.Append(@"
            ");
         }

         sb.Append(@$"writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""{memberInfo.Name}"") : ""{memberInfo.Name}"");
            ");

         if (memberInfo.IsReferenceTypeOrNullableStruct)
            sb.Append(@"   ");

         GenerateWriteValue(sb, memberInfo);

         if (memberInfo.IsReferenceTypeOrNullableStruct)
            sb.Append(@"
            }");
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
      var command = memberInfo.SpecialType switch
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
         switch (memberInfo.TypeFullyQualified)
         {
            case "global::System.Guid":
            case "global::System.TimeSpan":
            case "global::System.DateTimeOffset":
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
