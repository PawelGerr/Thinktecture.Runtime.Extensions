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

         return GenerateJsonConverter(state.EnumType, state.Namespace, state.EnumIdentifier, state.KeyType, "Get");
      }

      /// <inheritdoc />
      protected override string? GenerateValueType(ValueTypeSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         if (!state.HasKeyMember)
            return null;

         return GenerateJsonConverter(state.Type, state.Namespace, state.TypeIdentifier, state.KeyMember.Type, "Create");
      }

      private static string GenerateJsonConverter(
         ITypeSymbol type,
         string? @namespace,
         SyntaxToken typeIdentifier,
         ITypeSymbol keyType,
         string factoryMethod)
      {
         if (type.HasAttribute("Newtonsoft.Json.JsonConverterAttribute"))
            return String.Empty;

         return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Thinktecture;

{(String.IsNullOrWhiteSpace(@namespace) ? null : $"namespace {@namespace}")}
{{
   public class {typeIdentifier}_ValueTypeNewtonsoftJsonConverter : Thinktecture.Json.ValueTypeNewtonsoftJsonConverter<{typeIdentifier}, {keyType}>
   {{
      public {typeIdentifier}_ValueTypeNewtonsoftJsonConverter()
         : base({typeIdentifier}.{factoryMethod}, obj => ({keyType}) obj)
      {{
      }}
   }}

   [Newtonsoft.Json.JsonConverterAttribute(typeof({typeIdentifier}_ValueTypeNewtonsoftJsonConverter))]
   partial class {typeIdentifier}
   {{
   }}
}}
";
      }
   }
}
