using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for JsonConverter for Smart Enums.
/// </summary>
[Generator]
public class MessagePackSmartEnumSourceGenerator : SmartEnumSourceGeneratorBase
{
   /// <inheritdoc />
   public MessagePackSmartEnumSourceGenerator()
      : base("_MessagePack")
   {
   }

   /// <inheritdoc />
   protected override string GenerateEnum(EnumSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      var type = state.EnumType;

      if (type.HasAttribute("MessagePack.MessagePackFormatterAttribute"))
         return String.Empty;

      var ns = state.Namespace;
      var typeName = state.EnumType.Name;
      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Type.GetTypeMembers("ValueObjectMessagePackFormatter").Any());

      return $@"{GENERATED_CODE_PREFIX}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Thinktecture;
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
   partial {(type.IsValueType ? "struct" : "class")} {typeName}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectMessagePackFormatter : Thinktecture.Formatters.ValueObjectMessagePackFormatter<{typeName}, {state.KeyType}>
      {{
         public ValueObjectMessagePackFormatter()
            : base({typeName}.Get, static obj => obj.{state.KeyPropertyName})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
