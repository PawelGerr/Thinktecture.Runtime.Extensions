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
   public class {state.EnumIdentifier}_EnumNewtonsoftJsonConverter : Thinktecture.Json.EnumJsonConverter<{state.EnumIdentifier}, {state.KeyType}>
   {{
      public {state.EnumIdentifier}_EnumNewtonsoftJsonConverter()
         : base({state.EnumIdentifier}.Get)
      {{
      }}
   }}
");

         if (!HasJsonConverterAttribute(state))
         {
            sb.Append($@"
   [Newtonsoft.Json.JsonConverterAttribute(typeof({state.EnumSyntax.Identifier}_EnumNewtonsoftJsonConverter))]
   partial class {state.EnumIdentifier}
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
         return state.EnumSyntax.AttributeLists.SelectMany(a => a.Attributes).Any(a => state.Model.GetTypeInfo(a).Type?.ToString() == "Newtonsoft.Json.JsonConverterAttribute");
      }
   }
}
