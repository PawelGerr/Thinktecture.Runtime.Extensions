using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

[Generator]
public class MessagePackValueObjectSourceGenerator : ValueObjectSourceGeneratorBase<MessagePackValueObjectSourceGeneratorState>
{
   public MessagePackValueObjectSourceGenerator()
      : base("_MessagePack")
   {
   }

   protected override MessagePackValueObjectSourceGeneratorState CreateState(INamedTypeSymbol type, AttributeData valueObjectAttribute)
   {
      return new MessagePackValueObjectSourceGeneratorState(type, valueObjectAttribute);
   }

   protected override string? GenerateValueObject(MessagePackValueObjectSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      if (state.HasKeyMember)
         return GenerateFormatter(state, state.KeyMember);

      if (!state.Settings.SkipFactoryMethods)
         return GenerateValueObjectFormatter(state, stringBuilderProvider.GetStringBuilder(5_000));

      return null;
   }

   private static string GenerateFormatter(MessagePackValueObjectSourceGeneratorState state, EqualityInstanceMemberInfo keyMember)
   {
      if (state.HasMessagePackFormatterAttribute)
         return String.Empty;

      var ns = state.Namespace;

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
   partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
   {{
      public class ValueObjectMessagePackFormatter : global::Thinktecture.Formatters.ValueObjectMessagePackFormatter<{state.TypeFullyQualified}, {keyMember.Member.TypeFullyQualifiedWithNullability}>
      {{
         public ValueObjectMessagePackFormatter()
            : base({state.TypeFullyQualified}.Create, static obj => obj.{keyMember.Member.Name})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }

   private static string GenerateValueObjectFormatter(MessagePackValueObjectSourceGeneratorState state, StringBuilder sb)
   {
      if (state.HasMessagePackFormatterAttribute)
         return String.Empty;

      sb.Append($@"{GENERATED_CODE_PREFIX}
{(state.Namespace is null ? null : $@"
namespace {state.Namespace}
{{")}
   [global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
   partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
   {{
      public class ValueObjectMessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<{state.TypeFullyQualified}{state.NullableQuestionMark}>
      {{
         /// <inheritdoc />
         public {state.TypeFullyQualified}{state.NullableQuestionMark} Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
         {{
            if (reader.TryReadNil())
               return default;

            var count = reader.ReadArrayHeader();

            if (count != {state.AssignableInstanceFieldsAndProperties.Count})
               throw new global::MessagePack.MessagePackSerializationException($""Invalid member count. Expected {state.AssignableInstanceFieldsAndProperties.Count} but found {{count}} field/property values."");

            global::MessagePack.IFormatterResolver resolver = options.Resolver;
            options.Security.DepthStep(ref reader);

            try
            {{
");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
               var {memberInfo.ArgumentName} = {GenerateReadValue(memberInfo)}!;");
      }

      sb.Append(@$"

               var validationResult = {state.TypeFullyQualified}.TryCreate(");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
                                          {memberInfo.ArgumentName},");
      }

      sb.Append(@$"
                                          out var obj);

               if (validationResult != global::System.ComponentModel.DataAnnotations.ValidationResult.Success)
                  throw new global::MessagePack.MessagePackSerializationException($""Unable to deserialize \""{state.TypeMinimallyQualified}\"". Error: {{validationResult!.ErrorMessage}}."");

               return obj;
            }}
            finally
            {{
              reader.Depth--;
            }}
         }}

         /// <inheritdoc />
         public void Serialize(ref global::MessagePack.MessagePackWriter writer, {state.TypeFullyQualified}{state.NullableQuestionMark} value, global::MessagePack.MessagePackSerializerOptions options)
         {{");

      if (state.IsReferenceType)
      {
         sb.Append(@$"
            if(value is null)
            {{
               writer.WriteNil();
               return;
            }}
");
      }

      sb.Append(@$"
            writer.WriteArrayHeader({state.AssignableInstanceFieldsAndProperties.Count});

            var resolver = options.Resolver;");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
            {GenerateWriteValue(memberInfo)};");
      }

      sb.Append($@"
         }}
      }}
   }}
{(state.Namespace is null ? null : @"}
")}");

      return sb.ToString();
   }

   private static string GenerateWriteValue(InstanceMemberInfo memberInfo)
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
         return $"writer.{command}(value.{memberInfo.Name})";

      return @$"global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<{memberInfo.TypeFullyQualifiedWithNullability}>(resolver).Serialize(ref writer, value.{memberInfo.Name}, options)";
   }

   private static string GenerateReadValue(InstanceMemberInfo memberInfo)
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
         return @$"reader.{command}()";

      return @$"global::MessagePack.FormatterResolverExtensions.GetFormatterWithVerify<{memberInfo.TypeFullyQualifiedWithNullability}>(resolver).Deserialize(ref reader, options)";
   }
}
