using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class NewtonsoftJsonValueObjectCodeGenerator : CodeGeneratorBase
{
   private readonly ValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _stringBuilder;

   public override string FileNameSuffix => ".NewtonsoftJson";

   public NewtonsoftJsonValueObjectCodeGenerator(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _stringBuilder = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      if (_state.AttributeInfo.HasNewtonsoftJsonConverterAttribute)
         return;

      if (_state.HasKeyMember)
      {
         GenerateJsonConverter(_state, _state.KeyMember);
      }
      else if (!_state.Settings.SkipFactoryMethods)
      {
         GenerateValueObjectJsonConverter(_state, _stringBuilder, cancellationToken);
      }
   }

   private void GenerateJsonConverter(ValueObjectSourceGeneratorState state, EqualityInstanceMemberInfo keyMember)
   {
      _stringBuilder.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (state.Namespace is not null)
      {
         _stringBuilder.Append(@"
namespace ").Append(state.Namespace).Append(@";
");
      }

      _stringBuilder.Append(@"
[global::Newtonsoft.Json.JsonConverterAttribute(typeof(global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<").Append(state.TypeFullyQualified).Append(", ").Append(keyMember.Member.TypeFullyQualified).Append(@">))]
partial ").Append(state.IsReferenceType ? "class" : "struct").Append(" ").Append(state.Name).Append(@"
{
}
");
   }

   private static void GenerateValueObjectJsonConverter(ValueObjectSourceGeneratorState state, StringBuilder sb, CancellationToken cancellationToken)
   {
      sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (state.Namespace is not null)
      {
         sb.Append(@"
namespace ").Append(state.Namespace).Append(@";
");
      }

      sb.Append(@"
[global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
partial ").Append(state.IsReferenceType ? "class" : "struct").Append(" ").Append(state.Name).Append(@"
{
   public sealed class ValueObjectNewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter
   {
      private static readonly global::System.Type _type = typeof(").Append(state.TypeFullyQualified).Append(@");

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

            return default(").Append(state.TypeFullyQualified).Append(@");
         }

         if (reader.TokenType != global::Newtonsoft.Json.JsonToken.StartObject)
            throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").Append(state.TypeMinimallyQualified).Append(@"\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.StartObject)}\""."");
");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@"
         ").Append(memberInfo.TypeFullyQualifiedNullAnnotated).Append(" ").Append(memberInfo.ArgumentName).Append(" = default;");
      }

      sb.Append(@"

         var comparer = global::System.StringComparer.OrdinalIgnoreCase;

         while (reader.Read())
         {
            if (reader.TokenType == global::Newtonsoft.Json.JsonToken.EndObject)
               break;

            if (reader.TokenType != global::Newtonsoft.Json.JsonToken.PropertyName)
               throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").Append(state.TypeMinimallyQualified).Append(@"\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.PropertyName)}\""."");

            var propName = reader.Value!.ToString();

            if(!reader.Read())
               throw new global::Newtonsoft.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""").Append(state.TypeMinimallyQualified).Append(@"\""."");
");

      cancellationToken.ThrowIfCancellationRequested();

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

         sb.Append(@"(comparer.Equals(propName, """).Append(memberInfo.ArgumentName).Append(@"""))
            {
               ").Append(memberInfo.ArgumentName).Append(" = serializer.Deserialize<").Append(memberInfo.TypeFullyQualifiedWithNullability).Append(@">(reader);
            }");
      }

      if (state.AssignableInstanceFieldsAndProperties.Count > 0)
      {
         sb.Append(@"
            else
            {
               throw new global::Newtonsoft.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""").Append(state.TypeMinimallyQualified).Append(@"\""."");
            }");
      }

      sb.Append(@"
         }

         var validationResult = ").Append(state.TypeFullyQualified).Append(".Validate(");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@"
                                    ").Append(memberInfo.ArgumentName).Append("!,");
      }

      sb.Append(@"
                                    out var obj);

         if (validationResult != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::Newtonsoft.Json.JsonException($""Unable to deserialize \""").Append(state.TypeMinimallyQualified).Append(@"\"". Error: {validationResult!.ErrorMessage}."");

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

         var obj = (").Append(state.TypeFullyQualified).Append(@")value;
         var resolver = serializer.ContractResolver as global::Newtonsoft.Json.Serialization.DefaultContractResolver;

         writer.WriteStartObject();");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@"
         var ").Append(memberInfo.ArgumentName).Append(@"PropertyValue = obj.").Append(memberInfo.Name).Append(@";
");

         if (memberInfo.IsReferenceTypeOrNullableStruct)
         {
            sb.Append(@"
         if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || ").Append(memberInfo.ArgumentName).Append(@"PropertyValue is not null)
         {
            ");
         }
         else
         {
            sb.Append(@"
         ");
         }

         sb.Append(@"writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""").Append(memberInfo.Name).Append(@""") : """).Append(memberInfo.Name).Append(@""");
         ");

         if (memberInfo.IsReferenceTypeOrNullableStruct)
            sb.Append("   ");

         GenerateWriteValue(sb, memberInfo);

         if (memberInfo.IsReferenceTypeOrNullableStruct)
            sb.Append(@"
         }");
      }

      sb.Append(@"
         writer.WriteEndObject();
      }
   }
}
");
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
               sb?.Append("serializer.Serialize(writer, ").Append(memberInfo.ArgumentName).Append("PropertyValue);");
               return;
         }
      }

      sb?.Append("writer.").Append(command).Append("(").Append(memberInfo.ArgumentName).Append("PropertyValue);");
   }
}
