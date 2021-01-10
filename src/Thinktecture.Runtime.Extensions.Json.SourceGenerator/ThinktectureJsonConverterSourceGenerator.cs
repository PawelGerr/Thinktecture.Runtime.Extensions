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
   public class ThinktectureJsonConverterSourceGenerator : ThinktectureRuntimeExtensionsSourceGeneratorBase
   {
      /// <inheritdoc />
      protected override string GenerateEnum(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         if (state.EnumType.HasAttribute("System.Text.Json.Serialization.JsonConverterAttribute"))
            return String.Empty;

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
   public class {state.EnumIdentifier}_EnumJsonConverter : Thinktecture.Text.Json.Serialization.EnumJsonConverter<{state.EnumIdentifier}, {state.KeyType}>
   {{
      public {state.EnumIdentifier}_EnumJsonConverter()
         : this(null)
      {{
      }}

      public {state.EnumIdentifier}_EnumJsonConverter(
         JsonConverter<{state.KeyType}>? keyConverter)
         : base({state.EnumIdentifier}.Get, keyConverter)
      {{
      }}
   }}

   [System.Text.Json.Serialization.JsonConverterAttribute(typeof({state.EnumIdentifier}_EnumJsonConverter))]
   partial class {state.EnumIdentifier}
   {{
   }}
}}
");

         return sb.ToString();
      }
   }
}
