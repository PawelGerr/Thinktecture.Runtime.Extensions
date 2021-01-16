using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
{
   /// <summary>
   /// Source generator for model binder for an enum-like classes and value types with a key member.
   /// </summary>
   [Generator]
   public class ThinktectureModelBinderSourceGenerator : ThinktectureRuntimeExtensionsSourceGeneratorBase
   {
      /// <inheritdoc />
      protected override string GenerateEnum(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         return GenerateJsonConverter(state.EnumType, state.Namespace, state.EnumIdentifier, state.KeyType, "Validate");
      }

      /// <inheritdoc />
      protected override string? GenerateValueType(ValueTypeSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         if (!state.HasKeyMember)
            return null;

         return GenerateJsonConverter(state.Type, state.Namespace, state.TypeIdentifier, state.KeyMember.Member.Type, "TryCreate");
      }

      private static string GenerateJsonConverter(
         ITypeSymbol type,
         string? @namespace,
         SyntaxToken typeIdentifier,
         ITypeSymbol keyType,
         string factoryMethod)
      {
         if (type.HasAttribute("System.Text.Json.Serialization.JsonConverterAttribute"))
            return String.Empty;

         return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Thinktecture.AspNetCore.ModelBinding;

{(String.IsNullOrWhiteSpace(@namespace) ? null : $"namespace {@namespace}")}
{{
   public class {typeIdentifier}_ValueTypeModelBinder : Thinktecture.AspNetCore.ModelBinding.ValueTypeModelBinder<{typeIdentifier}, {keyType}>
   {{
      public {typeIdentifier}_ValueTypeModelBinder()
         : base({typeIdentifier}.{factoryMethod})
      {{
      }}
   }}

   [Microsoft.AspNetCore.Mvc.ModelBinderAttribute(typeof({typeIdentifier}_ValueTypeModelBinder))]
   partial class {typeIdentifier}
   {{
   }}
}}
";
      }
   }
}
