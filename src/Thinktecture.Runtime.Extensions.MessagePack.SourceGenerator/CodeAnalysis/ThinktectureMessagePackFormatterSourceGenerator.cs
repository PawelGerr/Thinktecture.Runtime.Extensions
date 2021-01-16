using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis
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

         return GenerateFormatter(state.EnumType, state.Namespace, state.EnumIdentifier, state.KeyType, "Get");
      }

      /// <inheritdoc />
      protected override string? GenerateValueType(ValueTypeSourceGeneratorState state)
      {
         if (state is null)
            throw new ArgumentNullException(nameof(state));

         if (state.KeyMember is null)
            return null;

         return GenerateFormatter(state.Type, state.Namespace, state.TypeIdentifier, state.KeyMember.Member.Type, "Create");
      }

      private static string GenerateFormatter(
         ITypeSymbol type,
         string? @namespace,
         SyntaxToken typeIdentifier,
         ITypeSymbol keyType,
         string factoryMethod)
      {
         if (type.HasAttribute("MessagePack.MessagePackFormatterAttribute"))
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
   public class {typeIdentifier}_ValueTypeMessagePackFormatter : Thinktecture.Formatters.ValueTypeMessagePackFormatter<{typeIdentifier}, {keyType}>
   {{
      public {typeIdentifier}_ValueTypeMessagePackFormatter()
         : base({typeIdentifier}.{factoryMethod}, obj => ({keyType}) obj)
      {{
      }}
   }}

   [MessagePack.MessagePackFormatter(typeof({typeIdentifier}_ValueTypeMessagePackFormatter))]
   partial class {typeIdentifier}
   {{
   }}
}}
";
      }
   }
}
