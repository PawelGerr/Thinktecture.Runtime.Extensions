using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class FormattableCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Instance = new FormattableCodeGenerator();

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      sb.Append(@"
      global::System.IFormattable");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState keyMember)
   {
      sb.Append($@"

      /// <inheritdoc />
      public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)
      {{
         return this.{keyMember.Name}.ToString(format, formatProvider);
      }}");
   }
}
