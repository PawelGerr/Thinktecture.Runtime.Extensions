using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for JsonConverter for an enum-like class.
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
   protected override string GenerateEnum(EnumSourceGeneratorState state)
   {
      if (state is null)
         throw new ArgumentNullException(nameof(state));

      var type = state.EnumType;

      if (type.HasAttribute("Newtonsoft.Json.JsonConverterAttribute"))
         return String.Empty;

      var ns = state.Namespace;
      var typeName = state.EnumType.Name;
      var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Type.GetTypeMembers("ValueObjectNewtonsoftJsonConverter").Any());

      return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Thinktecture;
{(ns is null ? null : $@"
namespace {ns}
{{")}
   [Newtonsoft.Json.JsonConverterAttribute(typeof(ValueObjectNewtonsoftJsonConverter))]
   partial {(type.IsValueType ? "struct" : "class")} {typeName}
   {{
      public {(requiresNew ? "new " : null)}class ValueObjectNewtonsoftJsonConverter : Thinktecture.Json.ValueObjectNewtonsoftJsonConverter<{typeName}, {state.KeyType}>
      {{
         public ValueObjectNewtonsoftJsonConverter()
            : base({typeName}.Get, static obj => obj.{state.KeyPropertyName})
         {{
         }}
      }}
   }}
{(ns is null ? null : @"}
")}";
   }
}
