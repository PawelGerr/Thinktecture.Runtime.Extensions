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

      if (state.EnumType.HasAttribute("MessagePack.MessagePackFormatterAttribute"))
         return String.Empty;

      var ns = state.Namespace;
      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Type.GetTypeMembers("ValueObjectMessagePackFormatter").Any());

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
   partial {(state.EnumType.IsValueType ? "struct" : "class")} {state.EnumType.Name}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectMessagePackFormatter : global::Thinktecture.Formatters.ValueObjectMessagePackFormatter<{state.EnumTypeFullyQualified}, {state.KeyTypeFullyQualified}>
      {{
         public ValueObjectMessagePackFormatter()
            : base({state.EnumTypeFullyQualified}.Get, static obj => obj.{state.KeyPropertyName})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
