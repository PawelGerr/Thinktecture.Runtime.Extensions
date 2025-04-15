using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectNewtonsoftJsonCodeGenerator : CodeGeneratorBase
{
   private readonly ITypeInformation _type;
   private readonly IReadOnlyList<InstanceMemberInfo> _assignableInstanceFieldsAndProperties;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Complex-ValueObject-NewtonsoftJson-CodeGenerator";
   public override string FileNameSuffix => ".NewtonsoftJson";

   public ComplexValueObjectNewtonsoftJsonCodeGenerator(
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
[global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
partial ").Append(_type.IsReferenceType ? "class" : "struct").Append(" ").Append(_type.Name).Append(@"
{
   public sealed class ValueObjectNewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter
   {
      private static readonly global::System.Type _type = typeof(").AppendTypeFullyQualified(_type).Append(@");

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
            if(global::System.Nullable.GetUnderlyingType(objectType) == _type)
               return null;
");

      if (_type.DisallowsDefaultValue)
      {
         _sb.Append(@"
            var (lineNumber, linePosition) = GetLineInfo(reader);

            throw new global::Newtonsoft.Json.JsonReaderException(
               $""Cannot convert null to type \""").AppendTypeMinimallyQualified(_type).Append(@"\"" because it doesn't allow default values."",
               reader.Path,
               lineNumber,
               linePosition,
               null);");
      }
      else
      {
         _sb.Append(@"
            var (lineNumber, linePosition) = GetLineInfo(reader);

            return default(").AppendTypeFullyQualified(_type).Append(");");
      }

      _sb.Append(@"
         }

         if (reader.TokenType != global::Newtonsoft.Json.JsonToken.StartObject)
         {
            var (lineNumber, linePosition) = GetLineInfo(reader);

            throw new global::Newtonsoft.Json.JsonReaderException(
               $""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.StartObject)}\""."",
               reader.Path,
               lineNumber,
               linePosition,
               null);
         }
");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         ").AppendTypeFullyQualifiedNullAnnotated(memberInfo).Append(" ").AppendEscaped(memberInfo.ArgumentName).Append(" = default;");
      }

      _sb.Append(@"

         var comparer = global::System.StringComparer.OrdinalIgnoreCase;

         while (reader.Read())
         {
            if (reader.TokenType == global::Newtonsoft.Json.JsonToken.EndObject)
               break;

            if (reader.TokenType != global::Newtonsoft.Json.JsonToken.PropertyName)
            {
               var (lineNumber, linePosition) = GetLineInfo(reader);

               throw new global::Newtonsoft.Json.JsonReaderException(
                  $""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.PropertyName)}\""."",
                  reader.Path,
                  lineNumber,
                  linePosition,
                  null);
            }

            var propName = reader.Value!.ToString();

            if(!reader.Read())
            {
               var (lineNumber, linePosition) = GetLineInfo(reader);

               throw new global::Newtonsoft.Json.JsonReaderException(
                  $""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""").AppendTypeMinimallyQualified(_type).Append(@"\""."",
                  reader.Path,
                  lineNumber,
                  linePosition,
                  null);
            }
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

         _sb.Append(@"(comparer.Equals(propName, """).Append(memberInfo.ArgumentName).Append(@"""))
            {
               ").AppendEscaped(memberInfo.ArgumentName).Append(" = serializer.Deserialize<").AppendTypeFullyQualified(memberInfo).Append(@">(reader);
            }");
      }

      if (_assignableInstanceFieldsAndProperties.Count > 0)
      {
         _sb.Append(@"
            else
            {
               var (lineNumber, linePosition) = GetLineInfo(reader);

               throw new global::Newtonsoft.Json.JsonReaderException(
                  $""Unknown member \""{propName}\"" encountered when trying to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\""."",
                  reader.Path,
                  lineNumber,
                  linePosition,
                  null);
            }");
      }

      _sb.Append(@"
         }");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (memberInfo.DisallowsDefaultValue)
         {
            _sb.Append(@"

         if (").AppendEscaped(memberInfo.ArgumentName).Append(" == default(").AppendTypeFullyQualified(memberInfo).Append(@"))
         {
            var (lineNumber, linePosition) = GetLineInfo(reader);

            throw new global::Newtonsoft.Json.JsonReaderException(
               $""Cannot deserialize type \""").AppendTypeMinimallyQualified(_type).Append("\\\" because the member \\\"").Append(memberInfo.Name).Append("\\\" of type \\\"").AppendTypeFullyQualified(memberInfo).Append(@"\"" is missing and does not allow default values."",
               reader.Path,
               lineNumber,
               linePosition,
               null);
         }");
         }
      }

      _sb.Append(@"

         var validationError = ").AppendTypeFullyQualified(_type).Append(".Validate(");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
                                    ").AppendEscaped(memberInfo.ArgumentName).Append("!,");
      }

      _sb.Append(@"
                                    out var obj);

         if (validationError is not null)
         {
            var (lineNumber, linePosition) = GetLineInfo(reader);

            throw new global::Newtonsoft.Json.JsonSerializationException(
               validationError.ToString() ?? ""Unable to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\""."",
               reader.Path,
               lineNumber,
               linePosition,
               null);
         }

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

         var obj = (").AppendTypeFullyQualified(_type).Append(@")value;
         var resolver = serializer.ContractResolver as global::Newtonsoft.Json.Serialization.DefaultContractResolver;

         writer.WriteStartObject();");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         var ").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue = obj.").Append(memberInfo.Name).Append(@";
");

         if (memberInfo.IsReferenceTypeOrNullableStruct)
         {
            _sb.Append(@"
         if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || ").AppendEscaped(memberInfo.ArgumentName).Append(@"PropertyValue is not null)
         {
            ");
         }
         else
         {
            _sb.Append(@"
         ");
         }

         _sb.Append(@"writer.WritePropertyName((resolver != null) ? resolver.GetResolvedPropertyName(""").Append(memberInfo.Name).Append(@""") : """).Append(memberInfo.Name).Append(@""");
         ");

         if (memberInfo.IsReferenceTypeOrNullableStruct)
            _sb.Append("   ");

         GenerateWriteValue(_sb, memberInfo);

         if (memberInfo.IsReferenceTypeOrNullableStruct)
            _sb.Append(@"
         }");
      }

      _sb.Append(@"
         writer.WriteEndObject();
      }

      private static (int Number, int Position) GetLineInfo(global::Newtonsoft.Json.JsonReader reader)
      {
         var lineInfo = (reader as global::Newtonsoft.Json.IJsonLineInfo);

         if (lineInfo?.HasLineInfo() == true)
         {
            return (lineInfo.LineNumber, lineInfo.LinePosition);
         }
         else
         {
            return (0, 0);
         }
      }
   }
}");

      _sb.RenderContainingTypesEnd(_type.ContainingTypes)
         .Append(@"
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
               sb?.Append("serializer.Serialize(writer, ").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue);");
               return;
         }
      }

      sb?.Append("writer.").Append(command).Append("(").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue);");
   }
}
