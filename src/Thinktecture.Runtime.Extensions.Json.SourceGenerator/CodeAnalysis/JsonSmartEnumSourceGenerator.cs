using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for JsonConverter for Smart Enums.
/// </summary>
[Generator]
public class JsonSmartEnumSourceGenerator : SmartEnumSourceGeneratorBase<JsonEnumSourceGeneratorState, JsonBaseEnumExtension>
{
   /// <inheritdoc />
   public JsonSmartEnumSourceGenerator()
      : base("_Json")
   {
   }

   protected override JsonEnumSourceGeneratorState CreateState(INamedTypeSymbol type, INamedTypeSymbol enumInterface)
   {
      return new JsonEnumSourceGeneratorState(type, enumInterface);
   }

   protected override string GenerateEnum(JsonEnumSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      if (state.HasJsonConverterAttribute)
         return String.Empty;

      var ns = state.Namespace;
      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Extension.HasValueObjectJsonConverterFactory);

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial {(state.IsReferenceType ? "class" : "struct")} {state.Name}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectJsonConverterFactory : global::System.Text.Json.Serialization.JsonConverterFactory
      {{
         /// <inheritdoc />
         public override bool CanConvert(global::System.Type typeToConvert)
         {{
            return typeof({state.EnumTypeFullyQualified}).IsAssignableFrom(typeToConvert);
         }}

         /// <inheritdoc />
         public override global::System.Text.Json.Serialization.JsonConverter CreateConverter(global::System.Type typeToConvert, global::System.Text.Json.JsonSerializerOptions options)
         {{
            if (typeToConvert is null)
               throw new global::System.ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new global::System.ArgumentNullException(nameof(options));

            return new global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<{state.EnumTypeFullyQualified}, {state.KeyProperty.TypeFullyQualifiedWithNullability}>({state.EnumTypeFullyQualified}.Get, static obj => obj.{state.KeyProperty.Name}, options);
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
