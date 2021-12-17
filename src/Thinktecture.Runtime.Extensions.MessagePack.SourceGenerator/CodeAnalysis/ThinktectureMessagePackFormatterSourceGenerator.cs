using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for JsonConverter for an enum-like class.
/// </summary>
[Generator]
public class ThinktectureMessagePackFormatterSourceGenerator : ThinktectureRuntimeExtensionsSourceGeneratorBase
{
   /// <inheritdoc />
   public ThinktectureMessagePackFormatterSourceGenerator()
      : base("_MessagePack")
   {
   }

   /// <inheritdoc />
   protected override string GenerateEnum(EnumSourceGeneratorState state)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Type.GetTypeMembers("ValueObjectMessagePackFormatter").Any());
      return GenerateFormatter(state.EnumType, state.Namespace, state.EnumType.Name, state.KeyType, "Get", state.KeyPropertyName, requiresNew);
   }

   /// <inheritdoc />
   protected override string? GenerateValueObject(ValueObjectSourceGeneratorState state)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      if (state.HasKeyMember)
         return GenerateFormatter(state.Type, state.Namespace, state.Type.Name, state.KeyMember.Member.Type, "Create", state.KeyMember.Member.Identifier.ToString(), false);

      if (!state.SkipFactoryMethods)
         return GenerateValueObjectFormatter(state);

      return null;
   }

   private static string GenerateFormatter(
      ITypeSymbol type,
      string? @namespace,
      string typeName,
      ITypeSymbol keyType,
      string factoryMethod,
      string keyMember,
      bool requiresNew)
   {
      if (type.HasAttribute("MessagePack.MessagePackFormatterAttribute"))
         return String.Empty;

      return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Thinktecture;

{(String.IsNullOrWhiteSpace(@namespace) ? null : $"namespace {@namespace}")}
{{
   [MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
   partial {(type.IsValueType ? "struct" : "class")} {typeName}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectMessagePackFormatter : Thinktecture.Formatters.ValueObjectMessagePackFormatter<{typeName}, {keyType}>
      {{
         public ValueObjectMessagePackFormatter()
            : base({typeName}.{factoryMethod}, static obj => obj.{keyMember})
         {{
         }}
      }}
   }}
}}
";
   }

   private static string GenerateValueObjectFormatter(ValueObjectSourceGeneratorState state)
   {
      if (state.Type.HasAttribute("MessagePack.MessagePackFormatterAttribute"))
         return String.Empty;

      var sb = new StringBuilder($@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using MessagePack;
using MessagePack.Formatters;
using Thinktecture;

{(String.IsNullOrWhiteSpace(state.Namespace) ? null : $"namespace {state.Namespace}")}
{{
   [MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
   partial {(state.Type.IsValueType ? "struct" : "class")} {state.Type.Name}
   {{
      public class ValueObjectMessagePackFormatter : MessagePack.Formatters.IMessagePackFormatter<{state.Type.Name}{state.NullableQuestionMark}>
      {{
         /// <inheritdoc />
         public {state.Type.Name}{state.NullableQuestionMark} Deserialize(ref MessagePack.MessagePackReader reader, MessagePack.MessagePackSerializerOptions options)
         {{
            if (reader.TryReadNil())
               return default;

            var count = reader.ReadArrayHeader();

            if (count != {state.AssignableInstanceFieldsAndProperties.Count})
               throw new MessagePack.MessagePackSerializationException($""Invalid member count. Expected {state.AssignableInstanceFieldsAndProperties.Count} but found {{count}} field/property values."");

            MessagePack.IFormatterResolver resolver = options.Resolver;
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

               var validationResult = {state.Type.Name}.TryCreate(");

      for (var i = 0; i < state.AssignableInstanceFieldsAndProperties.Count; i++)
      {
         var memberInfo = state.AssignableInstanceFieldsAndProperties[i];

         sb.Append(@$"
                                          {memberInfo.ArgumentName},");
      }

      sb.Append(@$"
                                          out var obj);

               if (validationResult != System.ComponentModel.DataAnnotations.ValidationResult.Success)
                  throw new MessagePack.MessagePackSerializationException($""Unable to deserialize '{state.Type.Name}'. Error: {{validationResult!.ErrorMessage}}."");

               return obj;
            }}
            finally
            {{
              reader.Depth--;
            }}
         }}

         /// <inheritdoc />
         public void Serialize(ref MessagePack.MessagePackWriter writer, {state.Type.Name}{state.NullableQuestionMark} value, MessagePack.MessagePackSerializerOptions options)
         {{");

      if (state.Type.IsReferenceType)
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
}}
");

      return sb.ToString();
   }

   private static string GenerateWriteValue(InstanceMemberInfo memberInfo)
   {
      var command = memberInfo.Type.SpecialType switch
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
         return $"writer.{command}(value.{memberInfo.Identifier})";

      return @$"resolver.GetFormatterWithVerify<{memberInfo.Type}>().Serialize(ref writer, value.{memberInfo.Identifier}, options)";
   }

   private static string GenerateReadValue(InstanceMemberInfo memberInfo)
   {
      var command = memberInfo.Type.SpecialType switch
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

      return @$"resolver.GetFormatterWithVerify<{memberInfo.Type}>().Deserialize(ref reader, options)";
   }
}
