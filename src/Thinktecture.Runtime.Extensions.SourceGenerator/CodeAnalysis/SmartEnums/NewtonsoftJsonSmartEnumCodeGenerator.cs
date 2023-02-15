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
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<{_state.TypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}>))]
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name}
   {{
   }}
{(ns is null ? null : @"}
")}";
   }
}
