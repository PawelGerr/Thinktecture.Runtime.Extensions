using System.Text;

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

      _sb.Append(@"
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
partial ").Append(_type.IsReferenceType ? "class" : "struct").Append(" ").Append(_type.Name).Append(@"
{
   public sealed class ValueObjectJsonConverter : global::System.Text.Json.Serialization.JsonConverter<").Append(_type.TypeFullyQualified).Append(@">
   {");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];
         var needsConverter = GenerateWriteValue(null, memberInfo);

         if (needsConverter)
         {
            _sb.Append(@"
      private readonly global::System.Text.Json.Serialization.JsonConverter<").Append(memberInfo.TypeFullyQualifiedWithNullability).Append("> _").Append(memberInfo.ArgumentName.Raw).Append("Converter;");
         }

         _sb.Append(@"
      private readonly string _").Append(memberInfo.ArgumentName.Raw).Append("PropertyName;");
      }

      _sb.Append(@"

      public ValueObjectJsonConverter(global::System.Text.Json.JsonSerializerOptions options)
      {
         if(options is null)
            throw new global::System.ArgumentNullException(nameof(options));

         var namingPolicy = options.PropertyNamingPolicy;
");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];
         var needsConverter = GenerateWriteValue(null, memberInfo);

         if (needsConverter)
         {
            _sb.Append(@"
         this._").Append(memberInfo.ArgumentName.Raw).Append("Converter = (global::System.Text.Json.Serialization.JsonConverter<").Append(memberInfo.TypeFullyQualifiedWithNullability).Append(">)global::Thinktecture.JsonSerializerOptionsExtensions.GetCustomValueObjectMemberConverter(options, typeof(").Append(memberInfo.TypeFullyQualified).Append("));");
         }

         _sb.Append(@"
         this._").Append(memberInfo.ArgumentName.Raw).Append("PropertyName = namingPolicy?.ConvertName(\"").Append(memberInfo.Name).Append(@""") ?? """).Append(memberInfo.Name).Append(@""";");
      }

      _sb.Append(@"
      }

      /// <inheritdoc />
      public override ").Append(_type.TypeFullyQualifiedNullAnnotated).Append(@" Read(ref global::System.Text.Json.Utf8JsonReader reader, global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
      {
         if (reader.TokenType == global::System.Text.Json.JsonTokenType.Null)
            return default;

         if (reader.TokenType != global::System.Text.Json.JsonTokenType.StartObject)
            throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").Append(_type.TypeMinimallyQualified).Append(@"\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.StartObject)}\""."");
");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         ").Append(memberInfo.TypeFullyQualifiedNullAnnotated).Append(" ").Append(memberInfo.ArgumentName.Escaped).Append(" = default;");
      }

      _sb.Append(@"

         var comparer = options.PropertyNameCaseInsensitive ? global::System.StringComparer.OrdinalIgnoreCase : global::System.StringComparer.Ordinal;

         while (reader.Read())
         {
            if (reader.TokenType == global::System.Text.Json.JsonTokenType.EndObject)
               break;

            if (reader.TokenType != global::System.Text.Json.JsonTokenType.PropertyName)
               throw new global::System.Text.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").Append(_type.TypeMinimallyQualified).Append(@"\"". Expected token: \""{(global::System.Text.Json.JsonTokenType.PropertyName)}\""."");

            var propName = reader.GetString();

            if(!reader.Read())
               throw new global::System.Text.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""").Append(_type.TypeMinimallyQualified).Append(@"\""."");
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

         _sb.Append("(comparer.Equals(propName, this._").Append(memberInfo.ArgumentName.Raw).Append(@"PropertyName))
            {
               ").Append(memberInfo.ArgumentName.Escaped).Append(" = ");

         GenerateReadValue(_sb, memberInfo);

         _sb.Append(@";
            }");
      }

      if (_assignableInstanceFieldsAndProperties.Count > 0)
      {
         _sb.Append(@"
            else
            {
               throw new global::System.Text.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""").Append(_type.TypeMinimallyQualified).Append(@"\""."");
            }");
      }

      _sb.Append(@"
         }

         var validationError = ").Append(_type.TypeFullyQualified).Append(".Validate(");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
                                    ").Append(memberInfo.ArgumentName.Escaped).Append("!,");
      }

      _sb.Append(@"
                                    out var obj);

         if (validationError is not null)
            throw new global::System.Text.Json.JsonException(validationError.ToString() ?? ""Unable to deserialize \""").Append(_type.Name).Append(@"\""."");

         return obj;
      }

      /// <inheritdoc />
      public override void Write(global::System.Text.Json.Utf8JsonWriter writer, ").Append(_type.TypeFullyQualified).Append(@" value, global::System.Text.Json.JsonSerializerOptions options)
      {
         writer.WriteStartObject();

         var ignoreNullValues = options.DefaultIgnoreCondition is global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull or global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
         var ignoreDefaultValues = options.DefaultIgnoreCondition == global::System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault;
");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         var ").Append(memberInfo.ArgumentName.Raw).Append("PropertyValue = value.").Append(memberInfo.Name).Append(@";
");

         if (memberInfo.IsReferenceTypeOrNullableStruct)
         {
            _sb.Append(@"
         if(!ignoreNullValues || ").Append(memberInfo.ArgumentName.Raw).Append(@"PropertyValue is not null)
         {
            ");
         }
         else
         {
            _sb.Append(@"
         if(!ignoreDefaultValues || !").Append(memberInfo.ArgumentName.Raw).Append("PropertyValue.Equals(default(").Append(memberInfo.TypeFullyQualifiedWithNullability).Append(@")))
         {
            ");
         }

         _sb.Append("writer.WritePropertyName(this._").Append(memberInfo.ArgumentName.Raw).Append(@"PropertyName);
         ");

         _sb.Append("   ");

         GenerateWriteValue(_sb, memberInfo);

         _sb.Append(@"
         }");
      }

      _sb.Append(@"
         writer.WriteEndObject();
      }
   }

   public class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
   {
      /// <inheritdoc />
      public override bool CanConvert(global::System.Type typeToConvert)
      {
         return typeof(").Append(_type.TypeFullyQualified).Append(@").IsAssignableFrom(typeToConvert);
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

   private static bool GenerateWriteValue(StringBuilder? sb, InstanceMemberInfo memberInfo)
   {
      var command = memberInfo.SpecialType switch
      {
         SpecialType.System_Boolean => "WriteBooleanValue",

         SpecialType.System_String => "WriteStringValue",
         SpecialType.System_DateTime => "WriteStringValue",

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
            sb?.Append("this._").Append(memberInfo.ArgumentName.Raw).Append("Converter.Write(writer, ").Append(memberInfo.ArgumentName.Raw).Append("PropertyValue, options);");

            return true;
         }
      }

      sb?.Append("writer.").Append(command).Append("(").Append(memberInfo.ArgumentName.Raw).Append("PropertyValue);");

      return false;
   }

   private static void GenerateReadValue(StringBuilder sb, InstanceMemberInfo memberInfo)
   {
      var command = memberInfo.SpecialType switch
      {
         SpecialType.System_Boolean => "GetBoolean",

         SpecialType.System_String => "GetString",
         SpecialType.System_DateTime => "GetDateTime",

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
            sb.Append("this._").Append(memberInfo.ArgumentName.Raw).Append("Converter.Read(ref reader, typeof(").Append(memberInfo.TypeFullyQualified).Append("), options)");
            return;
         }
      }

      sb.Append("reader.").Append(command).Append("()");
   }
}
