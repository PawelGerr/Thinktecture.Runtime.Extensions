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
      public sealed class ValueObjectMessagePackFormatter : global::Thinktecture.Formatters.{(_state.IsReferenceType ? "ValueObjectMessagePackFormatterBase" : "StructValueObjectMessagePackFormatterBase")}<{_state.TypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}>
      {{
#if NET7_0
         public ValueObjectMessagePackFormatter()
            : base({(_state.IsValidatable ? "true" : "false")})
         {{
         }}
#else
         public ValueObjectMessagePackFormatter()
            : base({_state.TypeFullyQualified}.Get)
         {{
         }}
#endif
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
