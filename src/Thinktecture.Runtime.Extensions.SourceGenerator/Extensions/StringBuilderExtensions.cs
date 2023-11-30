using System.Text;
using Thinktecture.CodeAnalysis;

namespace Thinktecture;

public static class StringBuilderExtensions
{
   public static void GenerateStructLayoutAttributeIfRequired(this StringBuilder sb, bool isReferenceType, bool hasStructLayoutAttribute)
   {
      if (!isReferenceType && !hasStructLayoutAttribute)
      {
         sb.Append(@"
   [global::System.Runtime.InteropServices.StructLayout(global::System.Runtime.InteropServices.LayoutKind.Auto)]");
      }
   }

   public static StringBuilder RenderAccessModifier(
      this StringBuilder sb,
      ValueObjectAccessModifier accessModifier)
   {
      switch (accessModifier)
      {
         case ValueObjectAccessModifier.Private:
            sb.Append("private");
            break;
         case ValueObjectAccessModifier.Protected:
            sb.Append("protected");
            break;
         case ValueObjectAccessModifier.Internal:
            sb.Append("internal");
            break;
         case ValueObjectAccessModifier.Public:
            sb.Append("public");
            break;
         case ValueObjectAccessModifier.PrivateProtected:
            sb.Append("private protected");
            break;
         case ValueObjectAccessModifier.ProtectedInternal:
            sb.Append("protected internal");
            break;
      }

      return sb;
   }

   public static StringBuilder RenderArguments(
      this StringBuilder sb,
      IReadOnlyList<InstanceMemberInfo> members,
      string? prefix = null,
      bool leadingComma = false)
   {
      for (var i = 0; i < members.Count; i++)
      {
         if (leadingComma || i > 0)
            sb.Append(", ");

         sb.RenderArgument(members[i], prefix);
      }

      return sb;
   }

   public static StringBuilder RenderArgument(
      this StringBuilder sb,
      IMemberState member,
      string? prefix = null)
   {
      return sb.Append(prefix).Append(member.ArgumentName.Escaped);
   }

   public static StringBuilder RenderArgumentsWithType(
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

         sb.RenderArgumentWithType(members[i], prefix, useNullableTypes, addAllowNullNotNullCombi);
      }

      if (trailingComma && members.Count > 0)
         sb.Append(comma);

      return sb;
   }

   public static StringBuilder RenderArgumentWithType(
      this StringBuilder sb,
      IMemberState member,
      string? prefix = null,
      bool useNullableTypes = false,
      bool addAllowNullNotNullCombi = false)
   {
      if (addAllowNullNotNullCombi && member.IsReferenceType && member.NullableAnnotation != NullableAnnotation.Annotated)
         sb.Append("[global::System.Diagnostics.CodeAnalysis.AllowNullAttribute, global::System.Diagnostics.CodeAnalysis.NotNullAttribute] ");

      sb.Append(prefix).Append(member.TypeFullyQualifiedWithNullability);

      if (useNullableTypes && !member.IsNullableStruct)
         sb.Append("?");

      return sb.Append(' ').Append(member.ArgumentName.Escaped);
   }

   public static StringBuilder AppendCast(
      this StringBuilder sb,
      ITypeFullyQualified type,
      bool condition = true)
   {
      if (condition)
         sb.Append("(").Append(type.TypeFullyQualified).Append(")");

      return sb;
   }
}
