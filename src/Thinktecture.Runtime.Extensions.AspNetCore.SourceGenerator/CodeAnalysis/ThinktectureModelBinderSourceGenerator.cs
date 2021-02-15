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
      public ThinktectureModelBinderSourceGenerator()
         : base("_ModelBinder")
      {
      }

      /// <inheritdoc />
      protected override string GenerateEnum(EnumSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         var requiresNew = state.HasBaseEnum && (state.BaseEnum.IsSameAssembly || state.BaseEnum.Type.GetTypeMembers("ValueTypeModelBinder").Any());
         return GenerateJsonConverter(state.EnumType, state.Namespace, state.EnumIdentifier, state.KeyType, "Validate", requiresNew);
      }

      /// <inheritdoc />
      protected override string? GenerateValueType(ValueTypeSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         if (!state.HasKeyMember)
            return null;

         return GenerateJsonConverter(state.Type, state.Namespace, state.TypeIdentifier, state.KeyMember.Member.Type, "TryCreate", false);
      }

      private static string GenerateJsonConverter(
         ITypeSymbol type,
         string? @namespace,
         SyntaxToken typeIdentifier,
         ITypeSymbol keyType,
         string factoryMethod,
         bool requiresNew)
      {
         if (type.HasAttribute("Microsoft.AspNetCore.Mvc.ModelBinderAttribute"))
            return String.Empty;

         return $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Thinktecture.AspNetCore.ModelBinding;
using Microsoft.Extensions.Logging;

{(String.IsNullOrWhiteSpace(@namespace) ? null : $"namespace {@namespace}")}
{{
   [Microsoft.AspNetCore.Mvc.ModelBinderAttribute(typeof(ValueTypeModelBinder))]
   partial {(type.IsValueType ? "struct" : "class")} {typeIdentifier}
   {{
      public {(requiresNew ? "new " : null)}class ValueTypeModelBinder : Thinktecture.AspNetCore.ModelBinding.ValueTypeModelBinder<{typeIdentifier}, {keyType}>
      {{
         public ValueTypeModelBinder(ILoggerFactory loggerFactory)
            : base(loggerFactory, {typeIdentifier}.{factoryMethod})
         {{
         }}
      }}
   }}
}}
";
      }
   }
}
