using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for JsonConverter for Smart Enums.
/// </summary>
[Generator]
public class JsonSmartEnumSourceGenerator : SmartEnumSourceGeneratorBase
{
   /// <inheritdoc />
   public JsonSmartEnumSourceGenerator()
      : base("_Json")
   {
   }

   /// <inheritdoc />
   protected override string GenerateEnum(EnumSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      if (state.EnumType.HasAttribute("System.Text.Json.Serialization.JsonConverterAttribute"))
         return String.Empty;

      var ns = state.Namespace;
      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Type.GetTypeMembers("ValueObjectJsonConverterFactory").Any());

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial {(state.EnumType.IsValueType ? "struct" : "class")} {state.EnumType.Name}
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

            return new global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<{state.EnumTypeFullyQualified}, {state.KeyTypeFullyQualified}>({state.EnumTypeFullyQualified}.Get, static obj => obj.{state.KeyPropertyName}, options);
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
