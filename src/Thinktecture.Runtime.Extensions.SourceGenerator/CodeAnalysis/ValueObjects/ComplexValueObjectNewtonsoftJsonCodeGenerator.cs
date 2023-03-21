using System.Text;
using Microsoft.CodeAnalysis;

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

      _sb.Append(@"
[global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
partial ").Append(_type.IsReferenceType ? "class" : "struct").Append(" ").Append(_type.Name).Append(@"
{
   public sealed class ValueObjectNewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter
   {
      private static readonly global::System.Type _type = typeof(").Append(_type.TypeFullyQualified).Append(@");

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

            return default(").Append(_type.TypeFullyQualified).Append(@");
         }

         if (reader.TokenType != global::Newtonsoft.Json.JsonToken.StartObject)
            throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").Append(_type.TypeMinimallyQualified).Append(@"\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.StartObject)}\""."");
");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         ").Append(memberInfo.TypeFullyQualifiedNullAnnotated).Append(" ").Append(memberInfo.ArgumentName).Append(" = default;");
      }

      _sb.Append(@"

         var comparer = global::System.StringComparer.OrdinalIgnoreCase;

         while (reader.Read())
         {
            if (reader.TokenType == global::Newtonsoft.Json.JsonToken.EndObject)
               break;

            if (reader.TokenType != global::Newtonsoft.Json.JsonToken.PropertyName)
               throw new global::Newtonsoft.Json.JsonException($""Unexpected token \""{reader.TokenType}\"" when trying to deserialize \""").Append(_type.TypeMinimallyQualified).Append(@"\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.PropertyName)}\""."");

            var propName = reader.Value!.ToString();

            if(!reader.Read())
               throw new global::Newtonsoft.Json.JsonException($""Unexpected end of the JSON message when trying the read the value of \""{propName}\"" during deserialization of \""").Append(_type.TypeMinimallyQualified).Append(@"\""."");
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
               ").Append(memberInfo.ArgumentName).Append(" = serializer.Deserialize<").Append(memberInfo.TypeFullyQualifiedWithNullability).Append(@">(reader);
            }");
      }

      if (_assignableInstanceFieldsAndProperties.Count > 0)
      {
         _sb.Append(@"
            else
            {
               throw new global::Newtonsoft.Json.JsonException($""Unknown member \""{propName}\"" encountered when trying to deserialize \""").Append(_type.TypeMinimallyQualified).Append(@"\""."");
            }");
      }

      _sb.Append(@"
         }

         var validationResult = ").Append(_type.TypeFullyQualified).Append(".Validate(");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
                                    ").Append(memberInfo.ArgumentName).Append("!,");
      }

      _sb.Append(@"
                                    out var obj);

         if (validationResult != System.ComponentModel.DataAnnotations.ValidationResult.Success)
            throw new global::Newtonsoft.Json.JsonException($""Unable to deserialize \""").Append(_type.TypeMinimallyQualified).Append(@"\"". Error: {validationResult!.ErrorMessage}."");

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

         var obj = (").Append(_type.TypeFullyQualified).Append(@")value;
         var resolver = serializer.ContractResolver as global::Newtonsoft.Json.Serialization.DefaultContractResolver;

         writer.WriteStartObject();");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         var ").Append(memberInfo.ArgumentName).Append(@"PropertyValue = obj.").Append(memberInfo.Name).Append(@";
");

         if (memberInfo.IsReferenceTypeOrNullableStruct)
         {
            _sb.Append(@"
         if(serializer.NullValueHandling != global::Newtonsoft.Json.NullValueHandling.Ignore || ").Append(memberInfo.ArgumentName).Append(@"PropertyValue is not null)
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
