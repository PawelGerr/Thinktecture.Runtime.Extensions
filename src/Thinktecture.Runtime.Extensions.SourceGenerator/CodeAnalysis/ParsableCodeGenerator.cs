using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ParsableCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Default = new ParsableCodeGenerator(false);
   public static readonly IInterfaceCodeGenerator ForValidatableEnum = new ParsableCodeGenerator(true);

   private readonly bool _isForValidatableEnum;

   public string CodeGeneratorName => "Parsable-CodeGenerator";
   public string FileNameSuffix => ".Parsable";

   private ParsableCodeGenerator(bool isForValidatableEnum)
   {
      _isForValidatableEnum = isForValidatableEnum;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      sb.Append(@"
   global::System.IParsable<").Append(type.TypeFullyQualified).Append(">");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember)
   {
      GenerateParse(sb, type, keyMember);

      GenerateTryParse(sb, type, keyMember);
   }

   private void GenerateParse(StringBuilder sb, ITypeInformation type, IMemberInformation member)
   {
      sb.Append(@"
   /// <inheritdoc />
   public static ").Append(type.TypeFullyQualified).Append(@" Parse(string s, global::System.IFormatProvider? provider)
   {");

      if (member.IsString())
      {
         sb.Append(@"
      var validationResult = ").Append(type.TypeFullyQualified).Append(".Validate(s, out var result);");
      }
      else
      {
         sb.Append(@"
      var key = ").Append(member.TypeFullyQualified).Append(@".Parse(s, provider);
      var validationResult = ").Append(type.TypeFullyQualified).Append(".Validate(key, out var result);");
      }

      if (_isForValidatableEnum)
      {
         sb.Append(@"
      return result!;
   }");
      }
      else
      {
         sb.Append(@"

      if(validationResult is null)
         return result!;

      throw new global::System.FormatException(validationResult.ErrorMessage);
   }");
      }
   }

   private void GenerateTryParse(StringBuilder sb, ITypeInformation type, IMemberInformation member)
   {
      sb.Append(@"

   /// <inheritdoc />
   public static bool TryParse(
      string? s,
      global::System.IFormatProvider? provider,
      [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ").Append(type.TypeFullyQualified).Append(@" result)
   {
      if(s is null)
      {
         result = default;
         return false;
      }");

      if (member.IsString())
      {
         sb.Append(@"

      var validationResult = ").Append(type.TypeFullyQualified).Append(".Validate(s, out result!);");
      }
      else
      {
         sb.Append(@"

      if(!").Append(member.TypeFullyQualified).Append(@".TryParse(s, provider, out var key))
      {
         result = default;
         return false;
      }

      var validationResult = ").Append(type.TypeFullyQualified).Append(".Validate(key, out result!);");
      }

      if (_isForValidatableEnum)
      {
         sb.Append(@"
      return true;
   }");
      }
      else
      {
         sb.Append(@"
      return validationResult is null;
   }");
      }
   }
}
