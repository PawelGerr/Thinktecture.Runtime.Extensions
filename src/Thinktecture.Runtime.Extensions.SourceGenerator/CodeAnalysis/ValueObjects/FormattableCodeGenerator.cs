using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class FormattableCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Instance = new FormattableCodeGenerator();

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type)
   {
      sb.Append("global::System.IFormattable");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      sb.Append($@"

      /// <inheritdoc />
      public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)
      {{
         return this.{member.Name}.ToString(format, formatProvider);
      }}");
   }
}
