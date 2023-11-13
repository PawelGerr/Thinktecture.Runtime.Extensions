using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ParsableCodeGenerator : IInterfaceCodeGenerator<ParsableGeneratorState>
{
   public static readonly IInterfaceCodeGenerator<ParsableGeneratorState> Default = new ParsableCodeGenerator(false);
   public static readonly IInterfaceCodeGenerator<ParsableGeneratorState> ForValidatableEnum = new ParsableCodeGenerator(true);

   private readonly bool _isForValidatableEnum;

   public string CodeGeneratorName => "Parsable-CodeGenerator";
   public string FileNameSuffix => ".Parsable";

   private ParsableCodeGenerator(bool isForValidatableEnum)
   {
      _isForValidatableEnum = isForValidatableEnum;
   }

   public void GenerateBaseTypes(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"
   global::System.IParsable<").Append(state.Type.TypeFullyQualified).Append(">");
   }

   public void GenerateImplementation(StringBuilder sb, ParsableGeneratorState state)
   {
      GenerateValidate(sb, state);
      GenerateParse(sb, state);
      GenerateTryParse(sb, state);
   }

   private static void GenerateValidate(StringBuilder sb, ParsableGeneratorState state)
   {
      var keyType = state.KeyMember?.IsString() == true || state.HasStringBasedValidateMethod ? "string" : state.KeyMember?.TypeFullyQualified;
      sb.Append(@"
   private static ").Append(state.ValidationError.TypeFullyQualified).Append("? Validate<T>(").Append(keyType).Append(" key, global::System.IFormatProvider? provider, out ").Append(state.Type.TypeFullyQualifiedNullAnnotated).Append(@" result)
      where T : global::Thinktecture.IValueObjectFactory<").Append(state.Type.TypeFullyQualified).Append(", ").Append(keyType).Append(", ").Append(state.ValidationError.TypeFullyQualified).Append(@">
   {
      return T.Validate(key, provider, out result);
   }");
   }

   private void GenerateParse(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"

   /// <inheritdoc />
   public static ").Append(state.Type.TypeFullyQualified).Append(@" Parse(string s, global::System.IFormatProvider? provider)
   {");

      if (state.KeyMember?.IsString() == true || state.HasStringBasedValidateMethod)
      {
         sb.Append(@"
      var validationError = Validate<").Append(state.Type.TypeFullyQualified).Append(">(s, provider, out var result);");
      }
      else if (state.KeyMember is not null)
      {
         sb.Append(@"
      var key = ").Append(state.KeyMember.TypeFullyQualified).Append(@".Parse(s, provider);
      var validationError = Validate<").Append(state.Type.TypeFullyQualified).Append(">(key, provider, out var result);");
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

      if(validationError is null)
         return result!;

      throw new global::System.FormatException(validationError.ToString() ?? ""Unable to parse \""").Append(state.Type.Name).Append(@"\""."");
   }");
      }
   }

   private void GenerateTryParse(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"

   /// <inheritdoc />
   public static bool TryParse(
      string? s,
      global::System.IFormatProvider? provider,
      [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ").Append(state.Type.TypeFullyQualified).Append(@" result)
   {
      if(s is null)
      {
         result = default;
         return false;
      }");

      if (state.KeyMember?.IsString() == true || state.HasStringBasedValidateMethod)
      {
         sb.Append(@"

      var validationError = Validate<").Append(state.Type.TypeFullyQualified).Append(">(s, provider, out result!);");
      }
      else if (state.KeyMember is not null)
      {
         sb.Append(@"

      if(!").Append(state.KeyMember.TypeFullyQualified).Append(@".TryParse(s, provider, out var key))
      {
         result = default;
         return false;
      }

      var validationError = Validate<").Append(state.Type.TypeFullyQualified).Append(">(key, provider, out result!);");
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
      return validationError is null;
   }");
      }
   }
}
