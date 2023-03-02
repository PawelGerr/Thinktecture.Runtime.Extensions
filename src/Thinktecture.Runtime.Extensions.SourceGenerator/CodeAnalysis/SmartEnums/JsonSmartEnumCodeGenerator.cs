namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class JsonSmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;

   public override string FileNameSuffix => ".Json";

   public JsonSmartEnumCodeGenerator(EnumSourceGeneratorState state)
   {
      _state = state;
   }

   public override string? Generate()
   {
      if (_state.AttributeInfo.HasJsonConverterAttribute)
         return null;

      return $$"""
{{GENERATED_CODE_PREFIX}}
{{(_state.Namespace is null ? null : $@"
namespace {_state.Namespace};
")}}
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverterFactory<{{_state.TypeFullyQualified}}, {{_state.KeyProperty.TypeFullyQualified}}>))]
partial {{(_state.IsReferenceType ? "class" : "struct")}} {{_state.Name}}
{
}

""";
   }
}
