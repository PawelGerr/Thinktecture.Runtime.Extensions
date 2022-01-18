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
   protected override string GenerateEnum(EnumSourceGeneratorState state)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      var type = state.EnumType;

      if (type.HasAttribute("System.Text.Json.Serialization.JsonConverterAttribute"))
         return String.Empty;

      var ns = state.Namespace;
      var typeName = state.EnumType.Name;
      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Type.GetTypeMembers("ValueObjectJsonConverterFactory").Any());

      return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Thinktecture;
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [System.Text.Json.Serialization.JsonConverterAttribute(typeof(ValueObjectJsonConverterFactory))]
   partial {(type.IsValueType ? "struct" : "class")} {typeName}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectJsonConverterFactory : System.Text.Json.Serialization.JsonConverterFactory
      {{
         /// <inheritdoc />
         public override bool CanConvert(System.Type typeToConvert)
         {{
            return typeof({typeName}).IsAssignableFrom(typeToConvert);
         }}

         /// <inheritdoc />
         public override System.Text.Json.Serialization.JsonConverter CreateConverter(System.Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
         {{
            if (typeToConvert is null)
               throw new ArgumentNullException(nameof(typeToConvert));
            if (options is null)
               throw new ArgumentNullException(nameof(options));

            return new Thinktecture.Text.Json.Serialization.ValueObjectJsonConverter<{typeName}, {state.KeyType}>({typeName}.Get, static obj => obj.{state.KeyPropertyName}, options);
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
