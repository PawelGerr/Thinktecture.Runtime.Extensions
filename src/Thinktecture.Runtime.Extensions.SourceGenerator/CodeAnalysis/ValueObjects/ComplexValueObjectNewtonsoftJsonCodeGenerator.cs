using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectNewtonsoftJsonCodeGenerator<T> : CodeGeneratorBase
   where T : ITypeInformation, IHasGenerics
{
   private readonly T _type;
   private readonly ImmutableArray<InstanceMemberInfo> _assignableInstanceFieldsAndProperties;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Complex-ValueObject-NewtonsoftJson-CodeGenerator";
   public override string FileNameSuffix => ".NewtonsoftJson";

   public ComplexValueObjectNewtonsoftJsonCodeGenerator(
      T type,
      ImmutableArray<InstanceMemberInfo> assignableInstanceFieldsAndProperties,
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

      var isGeneric = !_type.GenericParameters.IsDefaultOrEmpty || _type.ContainingTypes.Any(ct => !ct.GenericParameters.IsDefaultOrEmpty);

      if (_type.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_type.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_type.ContainingTypes)
         .Append(@"
[global::Newtonsoft.Json.JsonConverterAttribute(typeof(").Append(isGeneric ? "ValueObjectNewtonsoftJsonConverterFactory" : "ValueObjectNewtonsoftJsonConverter").Append(@"))]
partial ").AppendTypeKind(_type).Append(" ").Append(_type.Name).AppendGenericTypeParameters(_type).Append(@"
{");

      GenerateConverter(cancellationToken);
      _sb.Append(@"
}");
      _sb.RenderContainingTypesEnd(_type.ContainingTypes);

      if (isGeneric)
         GenerateFactory();
   }

   private void GenerateConverter(CancellationToken cancellationToken)
   {
      _sb.Append(@"
   /// <summary>
   /// JSON converter for ").AppendTypeForXmlComment(_type).Append(@".
   /// </summary>
   ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
   [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
   public sealed class ValueObjectNewtonsoftJsonConverter : global::Newtonsoft.Json.JsonConverter
   {
      private static readonly global::System.Type _type = typeof(").AppendTypeFullyQualified(_type).Append(@");

      /// <inheritdoc />
      public override bool CanConvert(global::System.Type objectType)
      {
         return _type == objectType;
      }

      /// <inheritdoc />
      public override object? ReadJson(global::Newtonsoft.Json.JsonReader __reader, global::System.Type __objectType, object? __existingValue, global::Newtonsoft.Json.JsonSerializer __serializer)
      {
         if (__reader is null)
            throw new global::System.ArgumentNullException(""reader"");
         if (__serializer is null)
            throw new global::System.ArgumentNullException(""serializer"");

         if (__reader.TokenType == global::Newtonsoft.Json.JsonToken.Null)
         {
            if(global::System.Nullable.GetUnderlyingType(__objectType) == _type)
               return null;
");

      if (_type.DisallowsDefaultValue)
      {
         _sb.Append(@"
            var (__lineNumber, __linePosition) = GetLineInfo(__reader);

            throw new global::Newtonsoft.Json.JsonReaderException(
               $""Cannot convert null to type \""").AppendTypeMinimallyQualified(_type).Append(@"\"" because it doesn't allow default values."",
               __reader.Path,
               __lineNumber,
               __linePosition,
               null);");
      }
      else
      {
         _sb.Append(@"
            var (__lineNumber, __linePosition) = GetLineInfo(__reader);

            return default(").AppendTypeFullyQualified(_type).Append(");");
      }

      _sb.Append(@"
         }

         if (__reader.TokenType != global::Newtonsoft.Json.JsonToken.StartObject)
         {
            var (__lineNumber, __linePosition) = GetLineInfo(__reader);

            throw new global::Newtonsoft.Json.JsonReaderException(
               $""Unexpected token \""{__reader.TokenType}\"" when trying to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.StartObject)}\""."",
               __reader.Path,
               __lineNumber,
               __linePosition,
               null);
         }
");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Length; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (memberInfo is { IsReferenceTypeOrNullableStruct: false, DisallowsDefaultValue: true })
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

      // Resolve the expected property names through the same contract resolver used on the write path, so that
      // a non-default naming strategy (e.g. SnakeCaseNamingStrategy) round-trips instead of failing the read.
      //
      // The fixed parameters and locals of this method share their scope with one local per member, named after
      // the member's argument name. They all carry a double-underscore prefix to prevent a collision
      // (CS0128/CS0136, or CS0841 when a member local shadows a parameter that is used before the local's
      // declaration). The prefix is safe by construction: AppendArgumentName strips a leading run of underscores
      // only when the next character is a letter, so a member-derived identifier can never start with "__"
      // followed by a letter. That invariant additionally depends on ArgumentName.RenderAsIs being false, which
      // holds here because InstanceMemberInfo always creates its ArgumentName with the default renderAsIs: false.
      // The two ArgumentNullException calls pass a string literal instead of nameof, so that the reported
      // parameter name stays the friendly one declared by the overridden base method.
      _sb.Append(@"

         var __resolver = __serializer.ContractResolver as global::Newtonsoft.Json.Serialization.DefaultContractResolver;");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Length; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         var __").AppendArgumentName(memberInfo.ArgumentName).Append("PropertyName = (__resolver != null) ? __resolver.GetResolvedPropertyName(\"").Append(memberInfo.Name).Append("\") : \"").Append(memberInfo.Name).Append("\";");
      }

      _sb.Append(@"

         var __comparer = global::System.StringComparer.OrdinalIgnoreCase;

         while (__reader.Read())
         {
            if (__reader.TokenType == global::Newtonsoft.Json.JsonToken.EndObject)
               break;

            if (__reader.TokenType != global::Newtonsoft.Json.JsonToken.PropertyName)
            {
               var (__lineNumber, __linePosition) = GetLineInfo(__reader);

               throw new global::Newtonsoft.Json.JsonReaderException(
                  $""Unexpected token \""{__reader.TokenType}\"" when trying to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\"". Expected token: \""{(global::Newtonsoft.Json.JsonToken.PropertyName)}\""."",
                  __reader.Path,
                  __lineNumber,
                  __linePosition,
                  null);
            }

            var __propName = __reader.Value!.ToString();

            if(!__reader.Read())
            {
               var (__lineNumber, __linePosition) = GetLineInfo(__reader);

               throw new global::Newtonsoft.Json.JsonReaderException(
                  $""Unexpected end of the JSON message when trying the read the value of \""{__propName}\"" during deserialization of \""").AppendTypeMinimallyQualified(_type).Append(@"\""."",
                  __reader.Path,
                  __lineNumber,
                  __linePosition,
                  null);
            }
");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Length; i++)
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

         _sb.Append(@"(__comparer.Equals(__propName, __").AppendArgumentName(memberInfo.ArgumentName).Append(@"PropertyName))
            {
               ").AppendEscaped(memberInfo.ArgumentName).Append(" = __serializer.Deserialize<").AppendTypeFullyQualified(memberInfo).Append(@">(__reader);
            }");
      }

      if (_assignableInstanceFieldsAndProperties.Length > 0)
      {
         _sb.Append(@"
            else
            {
               var (__lineNumber, __linePosition) = GetLineInfo(__reader);

               throw new global::Newtonsoft.Json.JsonReaderException(
                  $""Unknown member \""{__propName}\"" encountered when trying to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\""."",
                  __reader.Path,
                  __lineNumber,
                  __linePosition,
                  null);
            }");
      }

      _sb.Append(@"
         }");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Length; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (memberInfo is { IsReferenceTypeOrNullableStruct: false, DisallowsDefaultValue: true })
         {
            _sb.Append(@"

         if (!").AppendEscaped(memberInfo.ArgumentName).Append(@".IsSet)
         {
            var (__lineNumber, __linePosition) = GetLineInfo(__reader);

            throw new global::Newtonsoft.Json.JsonReaderException(
               $""Cannot deserialize type \""").AppendTypeMinimallyQualified(_type).Append("\\\" because the member \\\"").Append(memberInfo.Name).Append("\\\" of type \\\"").AppendTypeFullyQualified(memberInfo).Append(@"\"" is missing and does not allow default values."",
               __reader.Path,
               __lineNumber,
               __linePosition,
               null);
         }");
         }
      }

      _sb.Append(@"

         var __validationError = ").AppendTypeFullyQualified(_type).Append(".Validate(");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Length; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
                                    ").AppendEscaped(memberInfo.ArgumentName).Append(memberInfo is { IsReferenceTypeOrNullableStruct: false, DisallowsDefaultValue: true } ? ".Value," : "!,");
      }

      _sb.Append(@"
                                    out var __obj);

         if (__validationError is not null)
         {
            var (__lineNumber, __linePosition) = GetLineInfo(__reader);

            throw new global::Newtonsoft.Json.JsonSerializationException(
               __validationError.ToString() ?? ""Unable to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\""."",
               __reader.Path,
               __lineNumber,
               __linePosition,
               null);
         }

         return __obj;
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

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Length; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         var ").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue = obj.").AppendIdentifier(memberInfo.Name).Append(@";
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
   }");
   }

   private void GenerateFactory()
   {
      _sb.Append(@"

").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
[global::System.Diagnostics.DebuggerNonUserCodeAttribute]
file class ValueObjectNewtonsoftJsonConverterFactory : global::Newtonsoft.Json.JsonConverter
{
   private static readonly global::System.Collections.Concurrent.ConcurrentDictionary<global::System.Type, global::Newtonsoft.Json.JsonConverter> _converterByType = new();

   public override bool CanConvert(global::System.Type objectType)
   {
      objectType = global::System.Nullable.GetUnderlyingType(objectType) ?? objectType;

      if (!objectType.IsGenericType || objectType.IsGenericTypeDefinition)
         return false;

      return typeof(").AppendTypeFullyQualifiedWithoutGenerics(_type, _type.ContainingTypes).AppendGenericTypeParameters(_type, constructOpenGeneric: true).Append(@") == objectType.GetGenericTypeDefinition();
   }

   public override object? ReadJson(global::Newtonsoft.Json.JsonReader reader, global::System.Type objectType, object? existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
   {
      return _converterByType.GetOrAdd(objectType, CreateConverter).ReadJson(reader, objectType, existingValue, serializer);
   }

   public override void WriteJson(global::Newtonsoft.Json.JsonWriter writer, object? value, global::Newtonsoft.Json.JsonSerializer serializer)
   {
      if (value is null)
      {
         writer.WriteNull();
      }
      else
      {
         _converterByType.GetOrAdd(value.GetType(), CreateConverter).WriteJson(writer, value, serializer);
      }
   }

   private static global::Newtonsoft.Json.JsonConverter CreateConverter(global::System.Type objectType)
   {
      if (objectType is null)
         throw new global::System.ArgumentNullException(nameof(objectType));

      objectType = global::System.Nullable.GetUnderlyingType(objectType) ?? objectType;

      var converterType = objectType.GetNestedType(""ValueObjectNewtonsoftJsonConverter"")
         ?? throw new global::System.Exception(""Implementation of the json converter for the complex value object \""").AppendTypeFullyQualified(_type).Append(@"\"" not found."");

      converterType = converterType.MakeGenericType(objectType.GenericTypeArguments);

      return (global::Newtonsoft.Json.JsonConverter?)global::System.Activator.CreateInstance(converterType)
         ?? throw new global::System.Exception($""Could not create an instance of json converter of type \""{converterType.FullName}\""."");
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
               sb?.Append("serializer.Serialize(writer, ").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue);");
               return;
         }
      }

      sb?.Append("writer.").Append(command).Append("(").AppendEscaped(memberInfo.ArgumentName).Append("PropertyValue);");
   }
}
