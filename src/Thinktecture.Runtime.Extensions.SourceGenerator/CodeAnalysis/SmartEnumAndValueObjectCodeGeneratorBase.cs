using System.Text;

namespace Thinktecture.CodeAnalysis;

public abstract class SmartEnumAndValueObjectCodeGeneratorBase : CodeGeneratorBase
{
   protected void GenerateKeyMember(StringBuilder sb, KeyMemberState keyMember, bool isEnum)
   {
      sb.Append(@"

      /// <summary>
      /// The identifier of this ").Append(isEnum ? "item" : "object").Append(@".
      /// </summary>
      ").RenderAccessModifier(keyMember.AccessModifier).Append(" ").Append(keyMember.Kind == MemberKind.Field ? "readonly " : null).AppendTypeFullyQualified(keyMember).Append(" ").Append(keyMember.Name).Append(keyMember.Kind == MemberKind.Property ? " { get; }" : ";");
   }

   protected void GenerateKeyMemberEqualityComparison(StringBuilder sb, KeyMemberState keyMember, string? keyMemberEqualityComparerAccessor)
   {
      sb.Append(@"
         return ");

      if (keyMemberEqualityComparerAccessor is not null)
      {
         sb.Append(keyMemberEqualityComparerAccessor).Append(".EqualityComparer.Equals(this.").Append(keyMember.Name).Append(", other.").Append(keyMember.Name).Append(");");
      }
      else if (keyMember.IsString())
      {
         sb.Append("global::System.StringComparer.OrdinalIgnoreCase.Equals(this.").Append(keyMember.Name).Append(", other.").Append(keyMember.Name).Append(");");
      }
      else
      {
         if (keyMember.IsReferenceType)
            sb.Append("this.").Append(keyMember.Name).Append(" is null ? other.").Append(keyMember.Name).Append(" is null : ");

         sb.Append("this.").Append(keyMember.Name).Append(".Equals(other.").Append(keyMember.Name).Append(");");
      }
   }
}
