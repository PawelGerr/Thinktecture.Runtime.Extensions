namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class NewtonsoftJsonSmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;

   public override string FileNameSuffix => ".NewtonsoftJson";

   public NewtonsoftJsonSmartEnumCodeGenerator(EnumSourceGeneratorState state)
   {
      _state = state;
   }

   public override string? Generate()
   {
      if (_state.AttributeInfo.HasNewtonsoftJsonConverterAttribute)
         return null;

      var ns = _state.Namespace;

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name}
   {{
      public sealed class ValueObjectNewtonsoftJsonConverter : global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverterBase<{_state.TypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}>
      {{
#if NET7_0
         public ValueObjectNewtonsoftJsonConverter()
            : base({(_state.IsValidatable ? "true" : "false")})
         {{
         }}
#else
         public ValueObjectNewtonsoftJsonConverter()
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
