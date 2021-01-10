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
   public class ThinktectureNewtonsoftJsonConverterSourceGenerator : ThinktectureRuntimeExtensionsSourceGeneratorBase
   {
      /// <inheritdoc />
      protected override string GenerateEnum(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         if (state.EnumType.HasAttribute("Newtonsoft.Json.JsonConverterAttribute"))
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
   public class {state.EnumIdentifier}_EnumNewtonsoftJsonConverter : Thinktecture.Json.EnumJsonConverter<{state.EnumIdentifier}, {state.KeyType}>
   {{
      public {state.EnumIdentifier}_EnumNewtonsoftJsonConverter()
         : base({state.EnumIdentifier}.Get)
      {{
      }}
   }}

   [Newtonsoft.Json.JsonConverterAttribute(typeof({state.EnumIdentifier}_EnumNewtonsoftJsonConverter))]
   partial class {state.EnumIdentifier}
   {{
   }}
}}
");

         return sb.ToString();
      }
   }
}
