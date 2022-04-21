using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class StringBuilderExtensions
{
   public static void GenerateStructLayoutAttributeIfRequired<TBaseEnumExtension>(this StringBuilder sb, EnumSourceGeneratorStateBase<TBaseEnumExtension> state)
      where TBaseEnumExtension : IEquatable<TBaseEnumExtension>
   {
      if (!state.IsReferenceType && !state.HasStructLayoutAttribute)
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
         sb.Append(prefix).Append(member.TypeFullyQualifiedWithNullability);

         if (useNullableTypes && !member.IsNullableStruct)
            sb.Append("?");

         sb.Append(' ').Append(member.ArgumentName);
      }

      if (trailingComma && members.Count > 0)
         sb.Append(comma);
   }
}
