using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ComplexValueObjectMessagePackCodeGenerator : CodeGeneratorBase
{
   private readonly ITypeInformation _type;
   private readonly IReadOnlyList<InstanceMemberInfo> _assignableInstanceFieldsAndProperties;
   private readonly StringBuilder _sb;

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
[global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
partial ").Append(_type.IsReferenceType ? "class" : "struct").Append(" ").Append(_type.Name).Append(@"
{
   public sealed class ValueObjectMessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<").AppendTypeFullyQualifiedNullAnnotated(_type).Append(@">
   {
      /// <inheritdoc />
      public ").AppendTypeFullyQualifiedNullAnnotated(_type).Append(@" Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
      {
         if (reader.TryReadNil())
            return default;

         var count = reader.ReadArrayHeader();

         if (count != ").Append(_assignableInstanceFieldsAndProperties.Count).Append(@")
            throw new global::MessagePack.MessagePackSerializationException($""Invalid member count. Expected ").Append(_assignableInstanceFieldsAndProperties.Count).Append(@" but found {count} field/property values."");

         global::MessagePack.IFormatterResolver resolver = options.Resolver;
         options.Security.DepthStep(ref reader);

         try
         {
");

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
            var ").AppendEscaped(memberInfo.ArgumentName).Append(" = ");

         GenerateReadValue(_sb, memberInfo);

         _sb.Append("!;");
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
         writer.WriteArrayHeader(").Append(_assignableInstanceFieldsAndProperties.Count).Append(@");

         var resolver = options.Resolver;");

      cancellationToken.ThrowIfCancellationRequested();

      for (var i = 0; i < _assignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = _assignableInstanceFieldsAndProperties[i];

         _sb.Append(@"
         ");
         GenerateWriteValue(_sb, memberInfo);

         _sb.Append(";");
      }

      _sb.Append(@"
      }
   }
}
");
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
