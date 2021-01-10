using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

namespace Thinktecture
{
   /// <summary>
   /// Source generator for JsonConverter for an enum-like class.
   /// </summary>
   [Generator]
   public class ThinktectureMessagePackFormatterSourceGenerator : ThinktectureRuntimeExtensionsSourceGeneratorBase
   {
      /// <inheritdoc />
      protected override string GenerateEnum(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         if (state.EnumType.HasAttribute("MessagePack.MessagePackFormatterAttribute"))
            return String.Empty;

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

   [MessagePack.MessagePackFormatter(typeof({state.EnumIdentifier}_EnumMessagePackFormatter))]
   partial class {state.EnumIdentifier}
   {{
   }}
");

         sb.Append(@"
}
");

         return sb.ToString();
      }
   }
}
