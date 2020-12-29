using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture
{
   /// <summary>
   /// Source generator for JsonConverter for an enum-like class.
   /// </summary>
   [Generator]
   public class EnumJsonConverterSourceGenerator : EnumSourceGeneratorBase
   {
      /// <inheritdoc />
      protected override string GenerateCode(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         var sb = new StringBuilder($@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json.Serialization;
using Thinktecture;

{(String.IsNullOrWhiteSpace(state.Namespace) ? null : $"namespace {state.Namespace}")}
{{
   public class {state.EnumType}_EnumJsonConverter : Thinktecture.Text.Json.Serialization.EnumJsonConverter<{state.EnumType}, {state.KeyType}>
   {{
      public {state.EnumType}_EnumJsonConverter()
         : this(null)
      {{
      }}

      public {state.EnumType}_EnumJsonConverter(
         JsonConverter<{state.KeyType}>? keyConverter)
         : base({state.EnumType}.Get, keyConverter)
      {{
      }}
   }}
");

         if (!HasJsonConverterAttribute(state))
         {
            sb.Append($@"
   [System.Text.Json.Serialization.JsonConverterAttribute(typeof({state.TypeDeclarationSyntax.Identifier}_EnumJsonConverter))]
   partial class {state.EnumType}
   {{
   }}
");
         }

         sb.Append(@"
}
");

         return sb.ToString();
      }

      private static bool HasJsonConverterAttribute(EnumSourceGeneratorState state)
      {
         return state.TypeDeclarationSyntax.AttributeLists.SelectMany(a => a.Attributes).Any(a => state.Model.GetTypeInfo(a).Type?.ToString() == "System.Text.Json.Serialization.JsonConverterAttribute");
      }
   }
}
