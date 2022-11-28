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

      var ns = _state.Namespace;

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name}
   {{
      public sealed class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
      {{
         /// <inheritdoc />
         public override bool CanConvert(global::System.Type typeToConvert)
         {{
            return typeof({_state.TypeFullyQualified}).IsAssignableFrom(typeToConvert);
         }}

         /// <inheritdoc />
         public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
         {{
            if (typeToConvert is null)
               throw new global::System.ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new global::System.ArgumentNullException(nameof(options));

#if NET7_0
            return new global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<{_state.TypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}>({(_state.IsValidatable ? "true" : "false")}, options);
#else
            return new global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<{_state.TypeFullyQualified}, {_state.KeyProperty.TypeFullyQualified}>({_state.TypeFullyQualified}.Get, options);
#endif
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
