using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ParsableCodeGenerator : IInterfaceCodeGenerator<ParsableGeneratorState>
{
   public static readonly IInterfaceCodeGenerator<ParsableGeneratorState> ForValueObject = new ParsableCodeGenerator(false, false);
   public static readonly IInterfaceCodeGenerator<ParsableGeneratorState> ForEnum = new ParsableCodeGenerator(true, false);
   public static readonly IInterfaceCodeGenerator<ParsableGeneratorState> ForValidatableEnum = new ParsableCodeGenerator(true, true);

   private readonly bool _isForEnum;
   private readonly bool _isForValidatableEnum;

   public string CodeGeneratorName => "Parsable-CodeGenerator";
   public string FileNameSuffix => ".Parsable";

   private ParsableCodeGenerator(
      bool isForEnum,
      bool isForValidatableEnum)
   {
      _isForEnum = isForEnum;
      _isForValidatableEnum = isForValidatableEnum;
   }

   public void GenerateBaseTypes(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"
   global::System.IParsable<").AppendTypeFullyQualified(state.Type).Append(">");

      if (_isForEnum && state.KeyMember?.IsString() == true)
      {
         sb.Append(@"
#if NET9_0_OR_GREATER
   , global::System.ISpanParsable<").AppendTypeFullyQualified(state.Type).Append(@">
#endif");
      }
   }

   public void GenerateImplementation(StringBuilder sb, ParsableGeneratorState state)
   {
      var isKeyTypeString = state.KeyMember?.IsString() == true;

      GenerateValidate(sb, state);

      GenerateParse(sb, state);

      if (_isForEnum && isKeyTypeString)
         GenerateParseForReadOnlySpanOfChar(sb, state);

      GenerateTryParse(sb, state);

      if (_isForEnum && isKeyTypeString)
         GenerateTryParseForReadOnlySpanOfChar(sb, state);
   }

   private void GenerateValidate(StringBuilder sb, ParsableGeneratorState state)
   {
      var isKeyTypeString = state.KeyMember?.IsString() == true;
      var keyType = isKeyTypeString || state.HasStringBasedValidateMethod ? "string" : state.KeyMember?.TypeFullyQualified;

      sb.Append(@"
   private static ").AppendTypeFullyQualified(state.ValidationError).Append("? Validate<T>(").Append(keyType).Append(" key, global::System.IFormatProvider? provider, out ").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(@" result)
      where T : global::Thinktecture.IObjectFactory<").AppendTypeFullyQualified(state.Type).Append(", ").Append(keyType).Append(", ").AppendTypeFullyQualified(state.ValidationError).Append(@">
   {
      return T.Validate(key, provider, out result);
   }");

      if (_isForEnum && isKeyTypeString)
      {
         sb.Append(@"

#if NET9_0_OR_GREATER
   private static ").AppendTypeFullyQualified(state.ValidationError).Append("? Validate<T>(global::System.ReadOnlySpan<char> key, global::System.IFormatProvider? provider, out ").AppendTypeFullyQualifiedNullAnnotated(state.Type).Append(@" result)
      where T : global::Thinktecture.IObjectFactory<").AppendTypeFullyQualified(state.Type).Append(", global::System.ReadOnlySpan<char>, ").AppendTypeFullyQualified(state.ValidationError).Append(@">
   {
      return T.Validate(key, provider, out result);
   }
#endif");
      }
   }

   private void GenerateParse(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"

   /// <inheritdoc />
   public static ").AppendTypeFullyQualified(state.Type).Append(@" Parse(string s, global::System.IFormatProvider? provider)
   {");

      if (state.KeyMember?.IsString() == true || state.HasStringBasedValidateMethod)
      {
         sb.Append(@"
      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(s, provider, out var result);");
      }
      else if (state.KeyMember is not null)
      {
         sb.Append(@"
      var key = ").AppendTypeFullyQualified(state.KeyMember).Append(@".Parse(s, provider);
      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(key, provider, out var result);");
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

   private void GenerateParseForReadOnlySpanOfChar(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"

#if NET9_0_OR_GREATER
   /// <inheritdoc />
   public static ").AppendTypeFullyQualified(state.Type).Append(@" Parse(global::System.ReadOnlySpan<char> s, global::System.IFormatProvider? provider)
   {");

      sb.Append(@"
      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(s, provider, out var result);");

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

      sb.Append(@"
#endif");
   }

   private void GenerateTryParse(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"

   /// <inheritdoc />
   public static bool TryParse(
      string? s,
      global::System.IFormatProvider? provider,
      [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ").AppendTypeFullyQualified(state.Type).Append(@" result)
   {
      if(s is null)
      {
         result = default;
         return false;
      }");

      if (state.KeyMember?.IsString() == true || state.HasStringBasedValidateMethod)
      {
         sb.Append(@"

      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(s, provider, out result!);");
      }
      else if (state.KeyMember is not null)
      {
         sb.Append(@"

      if(!").AppendTypeFullyQualified(state.KeyMember).Append(@".TryParse(s, provider, out var key))
      {
         result = default;
         return false;
      }

      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(key, provider, out result!);");
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

   private void GenerateTryParseForReadOnlySpanOfChar(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"

#if NET9_0_OR_GREATER
   /// <inheritdoc />
   public static bool TryParse(
      global::System.ReadOnlySpan<char> s,
      global::System.IFormatProvider? provider,
      [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ").AppendTypeFullyQualified(state.Type).Append(@" result)
   {
      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(s, provider, out result!);");

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

      sb.Append(@"
#endif");
   }
}
