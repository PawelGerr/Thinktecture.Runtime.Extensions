using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class FormattableCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Instance = new FormattableCodeGenerator();

   public string CodeGeneratorName => "Formattable-CodeGenerator";
   public string FileNameSuffix => ".Formattable";

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      sb.Append(@"
   global::System.IFormattable");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      sb.Append(@"
   /// <inheritdoc />
   public string ToString(string? format, global::System.IFormatProvider? formatProvider = null)
   {
      return this.").Append(keyMember.Name).Append(@".ToString(format, formatProvider);
   }");
   }
}
