using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ParsableCodeGenerator : IInterfaceCodeGenerator<ParsableGeneratorState>
{
   public static readonly IInterfaceCodeGenerator<ParsableGeneratorState> ForValueObject = new ParsableCodeGenerator(false);
   public static readonly IInterfaceCodeGenerator<ParsableGeneratorState> ForEnum = new ParsableCodeGenerator(true);

   private readonly bool _isForEnum;

   public string CodeGeneratorName => "Parsable-CodeGenerator";
   public string FileNameSuffix => ".Parsable";

   private ParsableCodeGenerator(bool isForEnum)
   {
      _isForEnum = isForEnum;
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

      var needParseMethod = false;

      if (state.KeyMember?.IsString() == true || state.HasStringBasedValidateMethod)
      {
         sb.Append(@"
      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(s, provider, out var result);");
      }
      else if (state.KeyMember is not null)
      {
         needParseMethod = true;
         sb.Append(@"
      var key = ParseValue<").AppendTypeFullyQualified(state.KeyMember).Append(@">(s, provider);
      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(key, provider, out var result);");
      }

      sb.Append(@"

      if(validationError is null)
         return result!;

      throw new global::System.FormatException(validationError.ToString() ?? ""Unable to parse \""").Append(state.Type.Name).Append(@"\""."");
   }");

      if (needParseMethod)
      {
         sb.Append(@"

   [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
   private static TValue ParseValue<TValue>(string s, global::System.IFormatProvider? provider)
      where TValue : global::System.IParsable<TValue>
   {
      return TValue.Parse(s, provider);
   }");
      }
   }

   private static void GenerateParseForReadOnlySpanOfChar(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"

#if NET9_0_OR_GREATER
   /// <inheritdoc />
   public static ").AppendTypeFullyQualified(state.Type).Append(@" Parse(global::System.ReadOnlySpan<char> s, global::System.IFormatProvider? provider)
   {
      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(@">(s, provider, out var result);

      if(validationError is null)
         return result!;

      throw new global::System.FormatException(validationError.ToString() ?? ""Unable to parse \""").Append(state.Type.Name).Append(@"\""."");
   }");

      sb.Append(@"
#endif");
   }

   private static void GenerateTryParse(StringBuilder sb, ParsableGeneratorState state)
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

      var needParseMethod = false;

      if (state.KeyMember?.IsString() == true || state.HasStringBasedValidateMethod)
      {
         sb.Append(@"

      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(s, provider, out result!);");
      }
      else if (state.KeyMember is not null)
      {
         needParseMethod = true;
         sb.Append(@"

      if (!TryParseValue<").AppendTypeFullyQualified(state.KeyMember).Append(@">(s, provider, out var key))
      {
         result = default;
         return false;
      }

      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(">(key, provider, out result!);");
      }

      sb.Append(@"
      return validationError is null;
   }");

      if (needParseMethod)
      {
         sb.Append(@"

   [global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
   private static bool TryParseValue<TValue>(string? s, global::System.IFormatProvider? provider, [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue result)
      where TValue : global::System.IParsable<TValue>
   {
      return TValue.TryParse(s, provider, out result);
   }");
      }
   }

   private static void GenerateTryParseForReadOnlySpanOfChar(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"

#if NET9_0_OR_GREATER
   /// <inheritdoc />
   public static bool TryParse(
      global::System.ReadOnlySpan<char> s,
      global::System.IFormatProvider? provider,
      [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ").AppendTypeFullyQualified(state.Type).Append(@" result)
   {
      var validationError = Validate<").AppendTypeFullyQualified(state.Type).Append(@">(s, provider, out result!);
      return validationError is null;
   }
#endif");
   }
}
