namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class MessagePackSmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;

   public override string FileNameSuffix => ".MessagePack";

   public MessagePackSmartEnumCodeGenerator(EnumSourceGeneratorState state)
   {
      _state = state;
   }

   public override string? Generate()
   {
      if (_state.AttributeInfo.HasMessagePackFormatterAttribute)
         return null;

      var ns = _state.Namespace;

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::MessagePack.MessagePackFormatter(typeof(ValueObjectMessagePackFormatter))]
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name}
   {{
      public sealed class ValueObjectMessagePackFormatter : global::Thinktecture.Formatters.ValueObjectMessagePackFormatterBase<{_state.TypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}>
      {{
         public ValueObjectMessagePackFormatter()
            : base({_state.TypeFullyQualified}.Get, static obj => obj.{_state.KeyProperty.Name})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
