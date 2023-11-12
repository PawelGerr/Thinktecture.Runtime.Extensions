using System.Text;
using Thinktecture.CodeAnalysis;
using Thinktecture.CodeAnalysis.SmartEnums;

namespace Thinktecture;

public static class StringBuilderExtensions
{
   public static void GenerateStructLayoutAttributeIfRequired(this StringBuilder sb, EnumSourceGeneratorState state)
   {
      if (state is { IsReferenceType: false, Settings.HasStructLayoutAttribute: false })
      {
         sb.Append(@"
   [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]");
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
         sb.Append(prefix).Append(member.ArgumentName.Escaped);
      }
   }

   public static void RenderArgumentsWithType(
      this StringBuilder sb,
      IReadOnlyList<InstanceMemberInfo> members,
      string? prefix = null,
      string comma = ", ",
      bool leadingComma = false,
      bool trailingComma = false,
      bool useNullableTypes = false,
      bool addAllowNullNotNullCombi = false)
   {
      for (var i = 0; i < members.Count; i++)
      {
         if (leadingComma || i > 0)
            sb.Append(comma);

         var member = members[i];

         if (addAllowNullNotNullCombi && member.IsReferenceType && member.NullableAnnotation != NullableAnnotation.Annotated)
            sb.Append("[global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ");

         sb.Append(prefix).Append(member.TypeFullyQualifiedWithNullability);

         if (useNullableTypes && !member.IsNullableStruct)
            sb.Append("?");

         sb.Append(' ').Append(member.ArgumentName.Escaped);
      }

      if (trailingComma && members.Count > 0)
         sb.Append(comma);
   }
}
