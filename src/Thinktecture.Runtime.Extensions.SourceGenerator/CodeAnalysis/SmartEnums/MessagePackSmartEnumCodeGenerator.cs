namespace Thinktecture.CodeAnalysis.SmartEnums;

public class MessagePackSmartEnumCodeGenerator : CodeGeneratorBase
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
      public class ValueObjectMessagePackFormatter : global::Thinktecture.Formatters.ValueObjectMessagePackFormatter<{_state.EnumTypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}>
      {{
         public ValueObjectMessagePackFormatter()
            : base({_state.EnumTypeFullyQualified}.Get, static obj => obj.{_state.KeyProperty.Name})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
