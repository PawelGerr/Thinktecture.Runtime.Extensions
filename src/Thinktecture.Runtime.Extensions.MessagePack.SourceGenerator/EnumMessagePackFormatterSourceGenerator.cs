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
   public class {state.EnumIdentifier}_EnumMessagePackFormatter : Thinktecture.Formatters.EnumMessagePackFormatter<{state.EnumIdentifier}, {state.KeyType}>
   {{
      public {state.EnumIdentifier}_EnumMessagePackFormatter()
         : base({state.EnumIdentifier}.Get)
      {{
      }}
   }}
");

         if (!state.HasAttribute("MessagePack.MessagePackFormatterAttribute"))
         {
            sb.Append($@"
   [MessagePack.MessagePackFormatter(typeof({state.EnumIdentifier}_EnumMessagePackFormatter))]
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
   }
}
