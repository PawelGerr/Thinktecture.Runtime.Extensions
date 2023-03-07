using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class MessagePackValueObjectCodeGenerator : CodeGeneratorBase
{
   private readonly ValueObjectSourceGeneratorState _state;
   private readonly StringBuilder _stringBuilder;

   public override string FileNameSuffix => ".MessagePack";

   public MessagePackValueObjectCodeGenerator(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _stringBuilder = stringBuilder;
   }

   public override void Generate()
   {
      if (_state.AttributeInfo.HasMessagePackFormatterAttribute)
         return;

      if (_state.HasKeyMember)
      {
         GenerateFormatter(_state, _state.KeyMember);
      }
      else if (!_state.Settings.SkipFactoryMethods)
      {
         GenerateValueObjectFormatter(_state, _stringBuilder);
      }
   }

   private void GenerateFormatter(ValueObjectSourceGeneratorState state, EqualityInstanceMemberInfo keyMember)
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
[global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.").Append(state.IsReferenceType ? "ValueObjectMessagePackFormatter" : "StructValueObjectMessagePackFormatter").Append("<").Append(state.TypeFullyQualified).Append(", ").Append(keyMember.Member.TypeFullyQualified).Append(@">))]
partial ").Append(state.IsReferenceType ? "class" : "struct").Append(" ").Append(state.Name).Append(@"
{
}
");
   }

   private static void GenerateValueObjectFormatter(ValueObjectSourceGeneratorState state, StringBuilder sb)
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
[global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
partial ").Append(state.IsReferenceType ? "class" : "struct").Append(" ").Append(state.Name).Append(@"
{
   public sealed class ValueObjectMessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<").Append(state.TypeFullyQualifiedNullAnnotated).Append(@">
   {
      /// <inheritdoc />
      public ").Append(state.TypeFullyQualifiedNullAnnotated).Append(@" Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
      {
         if (reader.TryReadNil())
            return default;

         var count = reader.ReadArrayHeader();

         if (count != ").Append(state.AssignableInstanceFieldsAndProperties.Count).Append(@")
            throw new global::MessagePack.MessagePackSerializationException($""Invalid member count. Expected ").Append(state.AssignableInstanceFieldsAndProperties.Count).Append(@" but found {count} field/property values."");

         global::MessagePack.IFormatterResolver resolver = options.Resolver;
         options.Security.DepthStep(ref reader);

         try
         {
");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@"
            var ").Append(memberInfo.ArgumentName).Append(" = ");

         GenerateReadValue(sb, memberInfo);

         sb.Append("!;");
      }

      sb.Append(@"

            var validationResult = ").Append(state.TypeFullyQualified).Append(".Validate(");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@"
                                       ").Append(memberInfo.ArgumentName).Append(",");
      }

      sb.Append(@"
                                       out var obj);

            if (validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
               throw new global::MessagePack.MessagePackSerializationException($""Unable to deserialize \""").Append(state.TypeMinimallyQualified).Append(@"\"". Error: {validationResult!.ErrorMessage}."");

            return obj;
         }
         finally
         {
           reader.Depth--;
         }
      }

      /// <inheritdoc />
      public void Serialize(ref global::MessagePack.MessagePackWriter writer, ").Append(state.TypeFullyQualifiedNullAnnotated).Append(@" value, global::MessagePack.MessagePackSerializerOptions options)
      {");

      if (state.IsReferenceType)
      {
         sb.Append(@"
         if(value is null)
         {
            writer.WriteNil();
            return;
         }
");
      }

      sb.Append(@"
         writer.WriteArrayHeader(").Append(state.AssignableInstanceFieldsAndProperties.Count).Append(@");

         var resolver = options.Resolver;");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@"
         ");
         GenerateWriteValue(sb, memberInfo);

         sb.Append(";");
      }

      sb.Append(@"
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
         return ;
      }

      sb.Append("global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<").Append(memberInfo.TypeFullyQualifiedWithNullability).Append(">(resolver).Serialize(ref writer, value.").Append(memberInfo.Name).Append(", options)");
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

      sb.Append("global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<").Append(memberInfo.TypeFullyQualifiedWithNullability).Append(">(resolver).Deserialize(ref reader, options)");
   }
}
