using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ParsableCodeGenerator : IInterfaceCodeGenerator
{
   public static readonly IInterfaceCodeGenerator Instance = new ParsableCodeGenerator(false);
   public static readonly IInterfaceCodeGenerator InstanceForValidatableEnum = new ParsableCodeGenerator(true);

   private readonly bool _isForValidatableEnum;

   private ParsableCodeGenerator(bool isForValidatableEnum)
   {
      _isForValidatableEnum = isForValidatableEnum;
   }

   public void GenerateBaseTypes(StringBuilder sb, ITypeInformation type)
   {
      sb.Append($"global::System.IParsable<{type.TypeFullyQualified}>");
   }

   public void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      GenerateParse(sb, type, member);

      GenerateTryParse(sb, type, member);
   }

   private void GenerateParse(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      sb.Append($@"

      /// <inheritdoc />
      public static {type.TypeFullyQualified} Parse(string s, global::System.IFormatProvider? provider)
      {{");

      if (member.IsString())
      {
         sb.Append($@"
         var validationResult = {type.TypeFullyQualified}.Validate(s, out var result);");
      }
      else
      {
         sb.Append($@"
         var key = {member.TypeFullyQualified}.Parse(s, provider);
         var validationResult = {type.TypeFullyQualified}.Validate(key, out var result);");
      }

      if (_isForValidatableEnum)
      {
         sb.Append(@"
         return result!;
      }");
      }
      else
      {
         sb.Append($@"

         if(validationResult is null)
            return result!;

         throw new global::System.FormatException(validationResult.ErrorMessage);
      }}");
      }
   }

   private void GenerateTryParse(StringBuilder sb, ITypeInformation type, IMemberState member)
   {
      sb.Append($@"

      /// <inheritdoc />
      public static bool TryParse(string? s, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out {type.TypeFullyQualified} result)
      {{
         if(s is null)
         {{
            result = default;
            return false;
         }}");

      if (member.IsString())
      {
         sb.Append($@"

         var validationResult = {type.TypeFullyQualified}.Validate(s, out result!);");
      }
      else
      {
         sb.Append($@"

         if(!{member.TypeFullyQualified}.TryParse(s, provider, out var key))
         {{
            result = default;
            return false;
         }}

         var validationResult = {type.TypeFullyQualified}.Validate(key, out result!);");
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
