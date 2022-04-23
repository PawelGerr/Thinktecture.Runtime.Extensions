namespace Thinktecture.CodeAnalysis.SmartEnums;

public class JsonSmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;

   public override string FileNameSuffix => "_Json";

   public JsonSmartEnumCodeGenerator(EnumSourceGeneratorState state)
   {
      _state = state;
   }

   public override string Generate()
   {
      if (_state.AttributeInfo.HasJsonConverterAttribute)
         return String.Empty;

      var ns = _state.Namespace;
      var requiresNew = _state.HasBaseEnum && (_state.BaseEnum.IsSameAssembly || _state.BaseEnum.HasJsonConverterFactory);

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial {(_state.IsReferenceType ? "class" : "struct")} {_state.Name}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
      {{
         /// <inheritdoc />
         public override bool CanConvert(global::System.Type typeToConvert)
         {{
            return typeof({_state.EnumTypeFullyQualified}).IsAssignableFrom(typeToConvert);
         }}

         /// <inheritdoc />
         public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
         {{
            if (typeToConvert is null)
               throw new global::System.ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new global::System.ArgumentNullException(nameof(options));

            return new global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<{_state.EnumTypeFullyQualified}, {_state.KeyProperty.TypeFullyQualifiedWithNullability}>({_state.EnumTypeFullyQualified}.Get, static obj => obj.{_state.KeyProperty.Name}, options);
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
