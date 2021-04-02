using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Thinktecture.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace Thinktecture
{
   public static class StringBuilderExtensions
   {
      public static void GenerateStructLayoutAttributeIfRequired(this StringBuilder sb, ITypeSymbol type)
      {
         if (type.IsValueType && !type.HasStructLayoutAttribute())
         {
            sb.Append($@"
   [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto)]");
         }
      }

      public static void RenderArguments(
         this StringBuilder sb,
         IReadOnlyList<InstanceMemberInfo> members,
         string? prefix = null,
         bool leadingComma = false)
      {
         for (var i = 0; i < members.Count; i++)
         {
            if (leadingComma || i > 0)
               sb.Append(", ");

            var member = members[i];
            sb.Append(prefix).Append(member.ArgumentName);
         }
      }

      public static void RenderArgumentsWithType(
         this StringBuilder sb,
         IReadOnlyList<InstanceMemberInfo> members,
         string? prefix = null,
         string comma = ", ",
         bool leadingComma = false,
         bool trailingComma = false,
         bool useNullableTypes = false)
      {
         for (var i = 0; i < members.Count; i++)
         {
            if (leadingComma || i > 0)
               sb.Append(comma);

            var member = members[i];
            sb.Append(prefix).Append(member.Type);

            if (useNullableTypes && !member.IsNullableStruct)
               sb.Append("?");

            sb.Append(' ').Append(member.ArgumentName);
         }

         if (trailingComma && members.Count > 0)
            sb.Append(comma);
      }
   }
}
