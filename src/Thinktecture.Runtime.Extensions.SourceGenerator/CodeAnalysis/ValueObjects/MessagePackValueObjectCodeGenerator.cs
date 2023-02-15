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

   public override string? Generate()
   {
      if (_state.AttributeInfo.HasMessagePackFormatterAttribute)
         return null;

      if (_state.HasKeyMember)
         return GenerateFormatter(_state, _state.KeyMember);

      if (!_state.Settings.SkipFactoryMethods)
         return GenerateValueObjectFormatter(_state, _stringBuilder);

      return null;
   }

   private static string GenerateFormatter(ValueObjectSourceGeneratorState state, EqualityInstanceMemberInfo keyMember)
   {
      var ns = state.Namespace;

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.{(state.IsReferenceType ? "ValueObjectMessagePackFormatter" : "StructValueObjectMessagePackFormatter")}<{state.TypeFullyQualified}, {keyMember.Member.TypeFullyQualifiedWithNullability}>))]
   partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
   {{
   }}
{(ns is null ? null : @"}
")}";
   }

   private static string GenerateValueObjectFormatter(ValueObjectSourceGeneratorState state, StringBuilder sb)
   {
      sb.Append($@"{GENERATED_CODE_PREFIX}
{(state.Namespace is null ? null : $@"
namespace {state.Namespace}
{{")}
   [global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
   partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
   {{
      public sealed class ValueObjectMessagePackFormatter : global::MessagePack.Formatters.IMessagePackFormatter<{state.TypeFullyQualifiedNullAnnotated}>
      {{
         /// <inheritdoc />
         public {state.TypeFullyQualifiedNullAnnotated} Deserialize(ref global::MessagePack.MessagePackReader reader, global::MessagePack.MessagePackSerializerOptions options)
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

               var validationResult = {state.TypeFullyQualified}.Validate(");

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
         public void Serialize(ref global::MessagePack.MessagePackWriter writer, {state.TypeFullyQualifiedNullAnnotated} value, global::MessagePack.MessagePackSerializerOptions options)
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
