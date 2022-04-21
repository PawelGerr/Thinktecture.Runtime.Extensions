using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

[Generator]
public class MessagePackSmartEnumSourceGenerator : SmartEnumSourceGeneratorBase<MessagePackEnumSourceGeneratorState, MessagePackBaseEnumExtension>
{
   /// <inheritdoc />
   public MessagePackSmartEnumSourceGenerator()
      : base("_MessagePack")
   {
   }

   protected override MessagePackEnumSourceGeneratorState CreateState(INamedTypeSymbol type, INamedTypeSymbol enumInterface)
   {
      return new MessagePackEnumSourceGeneratorState(type, enumInterface);
   }

   protected override string GenerateEnum(MessagePackEnumSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      if (state.HasMessagePackFormatterAttribute)
         return String.Empty;

      var ns = state.Namespace;
      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Extension.HasValueObjectMessagePackFormatter);

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
   partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectMessagePackFormatter : global::Thinktecture.Formatters.ValueObjectMessagePackFormatter<{state.EnumTypeFullyQualified}, {state.KeyProperty.TypeFullyQualifiedWithNullability}>
      {{
         public ValueObjectMessagePackFormatter()
            : base({state.EnumTypeFullyQualified}.Get, static obj => obj.{state.KeyProperty.Name})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
