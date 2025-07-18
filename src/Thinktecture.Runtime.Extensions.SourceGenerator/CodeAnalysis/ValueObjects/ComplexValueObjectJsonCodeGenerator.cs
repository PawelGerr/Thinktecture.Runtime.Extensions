using System.Text;
using Thinktecture.Json;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectJsonCodeGenerator : CodeGeneratorBase
{
   private readonly ITypeInformation _type;
   private readonly IReadOnlyList<InstanceMemberInfo> _assignableInstanceFieldsAndProperties;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Complex-ValueObject-SystemTextJson-CodeGenerator";
   public override string FileNameSuffix => ".Json";

   public ComplexValueObjectJsonCodeGenerator(
      ITypeInformation type,
      IReadOnlyList<InstanceMemberInfo> assignableInstanceFieldsAndProperties,
      StringBuilder stringBuilder)
   {
      _type = type;
      _assignableInstanceFieldsAndProperties = assignableInstanceFieldsAndProperties;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_type.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_type.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_type.ContainingTypes);

      _sb.Append(@"
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(JsonConverterFactory))]
partial ").AppendTypeKind(_type).Append(" ").Append(_type.Name).Append(@"
{
   public sealed class JsonConverter : global::System.Text.Json.Serialization.JsonConverter<").AppendTypeFullyQualified(_type).Append(@">
   {");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (memberInfo.JsonIgnoreCondition != JsonIgnoreCondition.Always
             && memberInfo.SpecialType != SpecialType.System_Object)
         {
            _sb.Append(@"
      private readonly global::System.Text.Json.Serialization.JsonConverter<").AppendTypeFullyQualified(memberInfo).Append("> _").Append(memberInfo.ArgumentName).Append("Converter;");
         }

         _sb.Append(@"
      private readonly string _").Append(memberInfo.ArgumentName).Append("PropertyName;");
      }

      _sb.Append(@"

      public JsonConverter(global::System.Text.Json.JsonSerializerOptions options)
      {
         if(options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         var namingPolicy = options.PropertyNamingPolicy;
");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (memberInfo.JsonIgnoreCondition != JsonIgnoreCondition.Always
             && memberInfo.SpecialType != SpecialType.System_Object)
         {
            _sb.Append(@"
         this._").Append(memberInfo.ArgumentName).Append("Converter = (global::System.Text.Json.Serialization.JsonConverter<").AppendTypeFullyQualified(memberInfo).Append(">)global::Thinktecture.Internal.JsonSerializerOptionsExtensions.GetCustomMemberConverter(options, typeof(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberInfo).Append("));");
         }

         _sb.Append(@"
         this._").Append(memberInfo.ArgumentName).Append("PropertyName = namingPolicy?.ConvertName(\"").Append(memberInfo.Name).Append(@""") ?? """).Append(memberInfo.Name).Append(@""";");
      }

      _sb.Append(@"
      }

      /// <inheritdoc />
      public override ").AppendTypeFullyQualifiedNullAnnotated(_type).Append(@" Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (reader.TokenType == global::System.Text.Json.JsonTokenType.Null)
            ");

      if (_type.DisallowsDefaultValue)
      {
         _sb.Append("throw new global::System.Text.Json.JsonException($\"Cannot convert null to type \\\"").AppendTypeMinimallyQualified(_type).Append("\\\" because it doesn't allow default values.\");");
      }
      else
      {
         _sb.Append("return default;");
      }

      _sb.Append(@"

         if (reader.TokenType != global::System.Text.Json.JsonTokenType.StartObject)
            throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.StartObject)}\""."");
");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         // Prepare variables for properties with "JsonIgnoreCondition.Always" as well

         if (!memberInfo.IsReferenceTypeOrNullableStruct && memberInfo.DisallowsDefaultValue)
         {
            _sb.Append(@"
         global::Thinktecture.Argument<").AppendTypeFullyQualified(memberInfo).Append("> ").AppendEscaped(memberInfo.ArgumentName).Append(" = default;");
         }
         else
         {
            _sb.Append(@"
         ").AppendTypeFullyQualifiedNullAnnotated(memberInfo).Append(" ").AppendEscaped(memberInfo.ArgumentName).Append(" = default;");
         }
      }

      _sb.Append(@"

         var comparer = options.PropertyNameCaseInsensitive ? global::System.StringComparer.OrdinalIgnoreCase : global::System.StringComparer.Ordinal;

         while (reader.Read())
         {
            if (reader.TokenType == global::System.Text.Json.JsonTokenType.EndObject)
               break;

            if (reader.TokenType != global::System.Text.Json.JsonTokenType.PropertyName)
               throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.PropertyName)}\""."");

            var propName = reader.GetString();

            if(!reader.Read())
               throw new global::System.Text.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""").AppendTypeMinimallyQualified(_type).Append(@"\""."");
");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (i == 0)
         {
            _sb.Append(@"
            if ");
         }
         else
         {
            _sb.Append(@"
            else if ");
         }

         _sb.Append("(comparer.Equals(propName, this._").Append(memberInfo.ArgumentName).Append(@"PropertyName))
            {");

         // Although empty, keep the condition; otherwise we end up in "else" and throw an exception
         if (memberInfo.JsonIgnoreCondition != JsonIgnoreCondition.Always)
         {
            _sb.Append(@"
               ").AppendEscaped(memberInfo.ArgumentName).Append(" = ");

            if (memberInfo.SpecialType == SpecialType.System_Object)
            {
               _sb.Append("global::System.Text.Json.JsonSerializer.Deserialize<object>(ref reader, options);");
            }
            else
            {
               _sb.Append("this._").Append(memberInfo.ArgumentName).Append("Converter.Read(ref reader, typeof(").AppendTypeFullyQualifiedWithoutNullAnnotation(memberInfo).Append(@"), options);");
            }
         }
         else
         {
            _sb.Append(@"
               // ").Append(memberInfo.Name).Append(" has JsonIgnoreCondition.Always, so we do not deserialize it.");
         }

         _sb.Append(@"
            }");
      }

      if (_assignableInstanceFieldsAndProperties.Count > 0)
      {
         _sb.Append(@"
            else
            {
               throw new global::System.Text.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\""."");
            }");
      }

      _sb.Append(@"
         }");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (!memberInfo.IsReferenceTypeOrNullableStruct && memberInfo.DisallowsDefaultValue)
         {
            _sb.Append(@"

         if (!").AppendEscaped(memberInfo.ArgumentName).Append(@".IsSet)
            throw new global::System.Text.Json.JsonException($""Cannot deserialize type \""").AppendTypeMinimallyQualified(_type).Append("\\\" because the member \\\"").Append(memberInfo.Name).Append("\\\" of type \\\"").AppendTypeFullyQualified(memberInfo).Append("\\\" is missing and does not allow default values.\");");
         }
      }

      _sb.Append(@"

         var validationError = ").AppendTypeFullyQualified(_type).Append(".Validate(");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
                                    ").AppendEscaped(memberInfo.ArgumentName).Append(memberInfo is { IsReferenceTypeOrNullableStruct: false, DisallowsDefaultValue: true } ? ".Value," : "!,");
      }

      _sb.Append(@"
                                    out var obj);

         if (validationError is not null)
            throw new global::System.Text.Json.JsonException(validationError.ToString() ?? ""Unable to deserialize \""").Append(_type.Name).Append(@"\""."");

         return obj;
      }

      /// <inheritdoc />
      public override void Write(global::System.Text.Json.Utf8JsonWriter writer, ").AppendTypeFullyQualified(_type).Append(@" value, global::System.Text.Json.JsonSerializerOptions options)
      {
         writer.WriteStartObject();

         var ignoreNullValues = options.DefaultIgnoreCondition is global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull or global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
         var ignoreDefaultValues = options.DefaultIgnoreCondition == global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (memberInfo.JsonIgnoreCondition == JsonIgnoreCondition.Always)
            continue;

         _sb.Append(@"
         var ").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue = value.").Append(memberInfo.Name).Append(@";
");

         if (memberInfo.JsonIgnoreCondition != JsonIgnoreCondition.Never)
         {
            if (memberInfo.IsReferenceTypeOrNullableStruct)
            {
               var ignoreNullValuesCondition = memberInfo.JsonIgnoreCondition is JsonIgnoreCondition.WhenWritingNull or JsonIgnoreCondition.WhenWritingDefault
                                                  ? null
                                                  : "!ignoreNullValues || ";

               _sb.Append(@"
         if(").Append(ignoreNullValuesCondition).AppendEscaped(memberInfo.ArgumentName).Append(@"PropertyValue is not null)
         {");
            }
            else
            {
               var ignoreDefaultValuesCondition = memberInfo.JsonIgnoreCondition is JsonIgnoreCondition.WhenWritingDefault
                                                     ? null
                                                     : "!ignoreDefaultValues || ";
               _sb.Append(@"
         if(").Append(ignoreDefaultValuesCondition).Append("!").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue.Equals(default(").AppendTypeFullyQualified(memberInfo).Append(@")))
         {");
            }
         }

         _sb.Append(@"
            writer.WritePropertyName(this._").Append(memberInfo.ArgumentName).Append("PropertyName);");

         if (memberInfo.SpecialType == SpecialType.System_Object)
         {
            _sb.Append(@"
            global::System.Text.Json.JsonSerializer.Serialize(writer, ").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue, options);");
         }
         else
         {
            _sb.Append(@"
            this._").Append(memberInfo.ArgumentName).Append("Converter.Write(writer, ").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue, options);");
         }

         if (memberInfo.JsonIgnoreCondition != JsonIgnoreCondition.Never)
         {
            _sb.Append(@"
         }");
         }
      }

      _sb.Append(@"
         writer.WriteEndObject();
      }
   }

   public class JsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert(global::System.Type typeToConvert)
      {
         return typeof(").AppendTypeFullyQualified(_type).Append(@").IsAssignableFrom(typeToConvert);
      }

      /// <inheritdoc />
      public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (typeToConvert is null)
            throw new global::System.ArgumentNullException(nameof(typeToConvert));
         if (options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         return new JsonConverter(options);
      }
   }
}");

      _sb.RenderContainingTypesEnd(_type.ContainingTypes)
         .Append(@"
");
   }
}
