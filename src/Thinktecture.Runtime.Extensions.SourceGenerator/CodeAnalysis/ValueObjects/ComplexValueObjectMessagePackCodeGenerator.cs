using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectMessagePackCodeGenerator : CodeGeneratorBase
{
   private readonly ITypeInformation _type;
   private readonly IReadOnlyList<InstanceMemberInfo> _assignableInstanceFieldsAndProperties;
   private readonly StringBuilder _sb;
   private readonly int _nextFreeKey;
   private readonly int _headerValue;

   public override string CodeGeneratorName => "Complex-ValueObject-MessagePack-CodeGenerator";
   public override string FileNameSuffix => ".MessagePack";

   public ComplexValueObjectMessagePackCodeGenerator(
      ITypeInformation type,
      IReadOnlyList<InstanceMemberInfo> assignableInstanceFieldsAndProperties,
      StringBuilder stringBuilder)
   {
      _type = type;
      _assignableInstanceFieldsAndProperties = assignableInstanceFieldsAndProperties;
      _sb = stringBuilder;

      (_headerValue, _nextFreeKey) = GetKeys(assignableInstanceFieldsAndProperties);
   }

   private static (int MaxKey, int NextFreeKey) GetKeys(IReadOnlyList<InstanceMemberInfo> assignableInstanceFieldsAndProperties)
   {
      var nextFreeKey = -1;
      var numberOfMembersWithoutKey = 0;

      for (var i = 0; i < assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = assignableInstanceFieldsAndProperties[i];

         if (memberInfo.MessagePackKey is null)
         {
            numberOfMembersWithoutKey++;
            continue;
         }

         if (nextFreeKey < memberInfo.MessagePackKey.Value)
            nextFreeKey = memberInfo.MessagePackKey.Value;
      }

      return (nextFreeKey == -1 ? assignableInstanceFieldsAndProperties.Count : nextFreeKey + numberOfMembersWithoutKey + 1, nextFreeKey + 1);
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
[global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
partial ").AppendTypeKind(_type).Append(" ").Append(_type.Name).Append(@"
{
   public sealed class ValueObjectMessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<").AppendTypeFullyQualifiedNullAnnotated(_type).Append(@">
   {
      /// <inheritdoc />
      public ").AppendTypeFullyQualifiedNullAnnotated(_type).Append(@" Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
      {
         if (reader.TryReadNil())
            ");

      if (_type.DisallowsDefaultValue)
      {
         _sb.Append("throw new global::MessagePack.MessagePackSerializationException($\"Cannot convert null to type \\\"").AppendTypeMinimallyQualified(_type).Append("\\\" because it doesn't allow default values.\");");
      }
      else
      {
         _sb.Append("return default;");
      }

      _sb.Append(@"

         options.Security.DepthStep(ref reader);

         var count = reader.ReadArrayHeader();
         global::MessagePack.IFormatterResolver resolver = options.Resolver;
");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         var ").AppendEscaped(memberInfo.ArgumentName).Append(" = default(").AppendTypeFullyQualified(memberInfo).Append(")!;");
      }

      _sb.Append(@"

         try
         {");

      var nextFreeKey = _nextFreeKey;

      _sb.Append(@"
            for (int i = 0; i < count; i++)
            {
               switch (i)
               {");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];
         var key = memberInfo.MessagePackKey ?? nextFreeKey++;

         _sb.Append(@"
                  case ").Append(key).Append(@":
                  {");

         _sb.Append(@"
                     ").AppendEscaped(memberInfo.ArgumentName).Append(" = ");

         GenerateReadValue(_sb, memberInfo);

         _sb.Append(@"!;
                     break;
                  }");
      }

      _sb.Append(@"
                  default:
                     reader.Skip();
                     break;
               }
            }");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (memberInfo.DisallowsDefaultValue)
         {
            _sb.Append(@"

            if (").AppendEscaped(memberInfo.ArgumentName).Append(" == default(").AppendTypeFullyQualified(memberInfo).Append(@"))
               throw new global::MessagePack.MessagePackSerializationException($""Cannot deserialize type \""").AppendTypeMinimallyQualified(_type).Append("\\\" because the member \\\"").Append(memberInfo.Name).Append("\\\" of type \\\"").AppendTypeFullyQualified(memberInfo).Append(@"\"" is missing and does not allow default values."");");
         }
      }

      _sb.Append(@"

            var validationError = ").AppendTypeFullyQualified(_type).Append(".Validate(");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
                                       ").AppendEscaped(memberInfo.ArgumentName).Append(",");
      }

      _sb.Append(@"
                                       out var obj);

            if (validationError is not null)
               throw new global::MessagePack.MessagePackSerializationException(validationError.ToString() ?? ""Unable to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\""."");

            return obj;
         }
         finally
         {
           reader.Depth--;
         }
      }

      /// <inheritdoc />
      public void Serialize(ref global::MessagePack.MessagePackWriter writer, ").AppendTypeFullyQualifiedNullAnnotated(_type).Append(@" value, global::MessagePack.MessagePackSerializerOptions options)
      {");

      if (_type.IsReferenceType)
      {
         _sb.Append(@"
         if(value is null)
         {
            writer.WriteNil();
            return;
         }
");
      }

      _sb.Append(@"
         writer.WriteArrayHeader(").Append(_headerValue).Append(@");

         var resolver = options.Resolver;");

      cancellationToken.ThrowIfCancellationRequested();

      nextFreeKey = _nextFreeKey;
      var numberOfUsedMembersWithoutKey = 0;

      for (var i = 0; i < _headerValue; i++)
      {
         var memberInfo = GetMemberWithKey(i, ref numberOfUsedMembersWithoutKey, ref nextFreeKey);

         if (memberInfo is not null)
         {
            _sb.Append(@"
         ");
            GenerateWriteValue(_sb, memberInfo);

            _sb.Append(";");
         }
         else
         {
            _sb.Append(@"
         writer.WriteNil();");
         }
      }

      _sb.Append(@"
      }
   }
}");

      _sb.RenderContainingTypesEnd(_type.ContainingTypes)
         .Append(@"
");
   }

   private InstanceMemberInfo? GetMemberWithKey(int key, ref int numberOfUsedMembersWithoutKey, ref int nextFreeKey)
   {
      var skippedMembersWithoutKey = 0;

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (memberInfo.MessagePackKey == key)
            return memberInfo;

         if (memberInfo.MessagePackKey is null)
         {
            if (key != nextFreeKey)
               continue;

            if (numberOfUsedMembersWithoutKey > skippedMembersWithoutKey)
            {
               skippedMembersWithoutKey++;
               continue;
            }

            nextFreeKey++;
            numberOfUsedMembersWithoutKey++;
            return memberInfo;
         }
      }

      return null;
   }

   private static void GenerateWriteValue(StringBuilder sb, InstanceMemberInfo memberInfo)
   {
      var command = memberInfo.SpecialType switch
      {
         SpecialType.System_Boolean => "Write",
         SpecialType.System_Char => "Write",
         SpecialType.System_String => "Write",
         SpecialType.System_DateTime => "Write",
         SpecialType.System_Byte => "Write",
         SpecialType.System_SByte => "Write",
         SpecialType.System_Int16 => "Write",
         SpecialType.System_UInt16 => "Write",
         SpecialType.System_Int32 => "Write",
         SpecialType.System_UInt32 => "Write",
         SpecialType.System_Int64 => "Write",
         SpecialType.System_UInt64 => "Write",
         SpecialType.System_Single => "Write",
         SpecialType.System_Double => "Write",
         _ => null
      };

      if (command is not null)
      {
         sb.Append("writer.").Append(command).Append("(value.").Append(memberInfo.Name).Append(")");
         return;
      }

      sb.Append("global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<").AppendTypeFullyQualified(memberInfo).Append(">(resolver).Serialize(ref writer, value.").Append(memberInfo.Name).Append(", options)");
   }

   private static void GenerateReadValue(StringBuilder sb, InstanceMemberInfo memberInfo)
   {
      var command = memberInfo.SpecialType switch
      {
         SpecialType.System_Boolean => "ReadBoolean",

         SpecialType.System_Char => "ReadChar",
         SpecialType.System_String => "ReadString",

         SpecialType.System_DateTime => "ReadDateTime",

         SpecialType.System_Byte => "ReadByte",
         SpecialType.System_SByte => "ReadSByte",

         SpecialType.System_Int16 => "ReadInt16",
         SpecialType.System_UInt16 => "ReadUInt16",

         SpecialType.System_Int32 => "ReadInt32",
         SpecialType.System_UInt32 => "ReadUInt32",

         SpecialType.System_Int64 => "ReadInt64",
         SpecialType.System_UInt64 => "ReadUInt64",

         SpecialType.System_Single => "ReadSingle",
         SpecialType.System_Double => "ReadDouble",
         _ => null
      };

      if (command is not null)
      {
         sb.Append("reader.").Append(command).Append("()");
         return;
      }

      sb.Append("global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<").AppendTypeFullyQualified(memberInfo).Append(">(resolver).Deserialize(ref reader, options)");
   }
}
