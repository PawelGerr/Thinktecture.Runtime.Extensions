using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectMessagePackCodeGenerator<T> : CodeGeneratorBase
   where T : ITypeInformation, IHasGenerics
{
   private readonly T _type;
   private readonly ImmutableArray<InstanceMemberInfo> _assignableInstanceFieldsAndProperties;
   private readonly StringBuilder _sb;
   private readonly int _nextFreeKey;
   private readonly int _headerValue;

   public override string CodeGeneratorName => "Complex-ValueObject-MessagePack-CodeGenerator";
   public override string FileNameSuffix => ".MessagePack";

   public ComplexValueObjectMessagePackCodeGenerator(
      T type,
      ImmutableArray<InstanceMemberInfo> assignableInstanceFieldsAndProperties,
      StringBuilder stringBuilder)
   {
      _type = type;
      _assignableInstanceFieldsAndProperties = assignableInstanceFieldsAndProperties;
      _sb = stringBuilder;

      (_headerValue, _nextFreeKey) = GetKeys(assignableInstanceFieldsAndProperties);
   }

   private static (int MaxKey, int NextFreeKey) GetKeys(ImmutableArray<InstanceMemberInfo> assignableInstanceFieldsAndProperties)
   {
      var nextFreeKey = -1;
      var numberOfMembersWithoutKey = 0;

      for (var i = 0; i < assignableInstanceFieldsAndProperties.Length; i++)
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

      return (nextFreeKey == -1 ? assignableInstanceFieldsAndProperties.Length : nextFreeKey + numberOfMembersWithoutKey + 1, nextFreeKey + 1);
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

      _sb.RenderContainingTypesStart(_type.ContainingTypes);

      _sb.Append(@"
[global::MessagePack.MessagePackFormatter(typeof(");

      if (isGeneric)
         _sb.AppendTypeFullyQualifiedWithoutGenerics(_type, _type.ContainingTypes).AppendGenericTypeParameters(_type, constructOpenGeneric: true).Append(".");

      _sb.Append(@"ValueObjectMessagePackFormatter))]
partial ").AppendTypeKind(_type).Append(" ").Append(_type.Name).AppendGenericTypeParameters(_type).Append(@"
{");
      GenerateFormatter(cancellationToken);
      _sb.Append(@"
}");

      _sb.RenderContainingTypesEnd(_type.ContainingTypes);

      _sb.Append(@"
");
   }

   private void GenerateFormatter(CancellationToken cancellationToken)
   {
      _sb.Append(@"
   /// <summary>
   /// MassagePack formatter for ").AppendTypeForXmlComment(_type).Append(@".
   /// </summary>
   ").Append(GENERATED_CODE_ATTRIBUTE).Append(@"
   [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
   public sealed class ValueObjectMessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<").AppendTypeFullyQualifiedNullAnnotated(_type).Append(@">
   {
      /// <inheritdoc />
      public ").AppendTypeFullyQualifiedNullAnnotated(_type).Append(@" Deserialize(ref global::MessagePack.MessagePackReader __reader, global::MessagePack.MessagePackSerializerOptions __options)
      {
         if (__reader.TryReadNil())
            ");

      if (_type.DisallowsDefaultValue)
      {
         _sb.Append("throw new global::MessagePack.MessagePackSerializationException($\"Cannot convert null to type \\\"").AppendTypeMinimallyQualified(_type).Append("\\\" because it doesn't allow default values.\");");
      }
      else
      {
         _sb.Append("return default;");
      }

      // The fixed parameters and locals of this method share their scope with one local per member, named after
      // the member's argument name. They all carry a double-underscore prefix to prevent a collision
      // (CS0128/CS0136, or CS0841 when a member local shadows a parameter that is used before the local's
      // declaration). The prefix is safe by construction: AppendArgumentName strips a leading run of underscores
      // only when the next character is a letter, so a member-derived identifier can never start with "__"
      // followed by a letter. That invariant additionally depends on ArgumentName.RenderAsIs being false, which
      // holds here because InstanceMemberInfo always creates its ArgumentName with the default renderAsIs: false.
      _sb.Append(@"

         __options.Security.DepthStep(ref __reader);

         var __count = __reader.ReadArrayHeader();
         global::MessagePack.IFormatterResolver __resolver = __options.Resolver;
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

      _sb.Append(@"

         try
         {");

      var nextFreeKey = _nextFreeKey;

      _sb.Append(@"
            for (int __i = 0; __i < __count; __i++)
            {
               switch (__i)
               {");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Length; i++)
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
                     __reader.Skip();
                     break;
               }
            }");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Length; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         if (!memberInfo.IsReferenceTypeOrNullableStruct && memberInfo.DisallowsDefaultValue)
         {
            _sb.Append(@"

            if (!").AppendEscaped(memberInfo.ArgumentName).Append(@".IsSet)
               throw new global::MessagePack.MessagePackSerializationException($""Cannot deserialize type \""").AppendTypeMinimallyQualified(_type).Append("\\\" because the member \\\"").Append(memberInfo.Name).Append("\\\" of type \\\"").AppendTypeFullyQualified(memberInfo).Append(@"\"" is missing and does not allow default values."");");
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
               throw new global::MessagePack.MessagePackSerializationException(__validationError.ToString() ?? ""Unable to deserialize \""").AppendTypeMinimallyQualified(_type).Append(@"\""."");

            return __obj;
         }
         finally
         {
           __reader.Depth--;
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
   }");
   }

   private InstanceMemberInfo? GetMemberWithKey(int key, ref int numberOfUsedMembersWithoutKey, ref int nextFreeKey)
   {
      var skippedMembersWithoutKey = 0;

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Length; i++)
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
         sb.Append("writer.").Append(command).Append("(value.").AppendIdentifier(memberInfo.Name).Append(")");
         return;
      }

      sb.Append("global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<").AppendTypeFullyQualified(memberInfo).Append(">(resolver).Serialize(ref writer, value.").AppendIdentifier(memberInfo.Name).Append(", options)");
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
         sb.Append("__reader.").Append(command).Append("()");
         return;
      }

      sb.Append("global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<").AppendTypeFullyQualified(memberInfo).Append(">(__resolver).Deserialize(ref __reader, __options)");
   }
}
