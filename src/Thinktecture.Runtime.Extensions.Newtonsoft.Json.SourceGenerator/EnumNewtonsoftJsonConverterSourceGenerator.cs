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
   public class EnumNewtonsoftJsonConverterSourceGenerator : EnumSourceGeneratorBase
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
using Thinktecture;

{(String.IsNullOrWhiteSpace(state.Namespace) ? null : $"namespace {state.Namespace}")}
{{
   public class {state.EnumType}_EnumNewtonsoftJsonConverter : Thinktecture.Json.EnumJsonConverter<{state.EnumType}, {state.KeyType}>
   {{
      public {state.EnumType}_EnumNewtonsoftJsonConverter()
         : base({state.EnumType}.Get)
      {{
      }}
   }}
");

         if (!HasJsonConverterAttribute(state))
         {
            sb.Append($@"
   [Newtonsoft.Json.JsonConverterAttribute(typeof({state.TypeDeclarationSyntax.Identifier}_EnumNewtonsoftJsonConverter))]
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
         return state.TypeDeclarationSyntax.AttributeLists.SelectMany(a => a.Attributes).Any(a => state.Model.GetTypeInfo(a).Type?.ToString() == "Newtonsoft.Json.JsonConverterAttribute");
      }
   }
}
