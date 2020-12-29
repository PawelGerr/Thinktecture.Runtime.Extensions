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
   public class EnumMessagePackFormatterSourceGenerator : EnumSourceGeneratorBase
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
   public class {state.EnumType}_EnumMessagePackFormatter : Thinktecture.Formatters.EnumMessagePackFormatter<{state.EnumType}, {state.KeyType}>
   {{
      public {state.EnumType}_EnumMessagePackFormatter()
         : base({state.EnumType}.Get)
      {{
      }}
   }}
");

         if (!HasMessagePackFormatterAttribute(state))
         {
            sb.Append($@"
   [MessagePack.MessagePackFormatter(typeof({state.TypeDeclarationSyntax.Identifier}_EnumMessagePackFormatter))]
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

      private static bool HasMessagePackFormatterAttribute(EnumSourceGeneratorState state)
      {
         return state.TypeDeclarationSyntax.AttributeLists.SelectMany(a => a.Attributes).Any(a => state.Model.GetTypeInfo(a).Type?.ToString() == "MessagePack.MessagePackFormatterAttribute");
      }
   }
}
