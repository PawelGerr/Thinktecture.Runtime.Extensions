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

      return $$"""
{{GENERATED_CODE_PREFIX}}
{{(_state.Namespace is null ? null : $@"
namespace {_state.Namespace};
")}}
[global::Newtonsoft.Json.JsonConverterAttribute(typeof(global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<{{_state.TypeFullyQualified}}, {{_state.KeyProperty.TypeFullyQualified}}>))]
partial {{(_state.IsReferenceType ? "class" : "struct")}} {{_state.Name}}
{
}

""";
   }
}
