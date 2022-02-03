using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for JsonConverter for Smart Enums.
/// </summary>
[Generator]
public class NewtonsoftJsonSmartEnumSourceGenerator : SmartEnumSourceGeneratorBase
{
   /// <inheritdoc />
   public NewtonsoftJsonSmartEnumSourceGenerator()
      : base("_NewtonsoftJson")
   {
   }

   /// <inheritdoc />
   protected override string GenerateEnum(EnumSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      if (state.EnumType.HasAttribute("global::Newtonsoft.Json.JsonConverterAttribute"))
         return String.Empty;

      var ns = state.Namespace;
      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Type.GetTypeMembers("ValueObjectNewtonsoftJsonConverter").Any());

      return $@"{GENERATED_CODE_PREFIX}
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [global::Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial {(state.EnumType.IsValueType ? "struct" : "class")} {state.EnumType.Name}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectNewtonsoftJsonConverter : global::Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<{state.EnumTypeFullyQualified}, {state.KeyTypeFullyQualified}>
      {{
         public ValueObjectNewtonsoftJsonConverter()
            : base({state.EnumTypeFullyQualified}.Get, static obj => obj.{state.KeyPropertyName})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
