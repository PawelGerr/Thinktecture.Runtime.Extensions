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
   [global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.{(_state.IsReferenceType ? "ValueObjectMessagePackFormatter" : "StructValueObjectMessagePackFormatter")}<{_state.TypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}>))]
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name}
   {{
   }}
{(ns is null ? null : @"}
")}";
   }
}
