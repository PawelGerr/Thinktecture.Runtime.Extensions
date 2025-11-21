using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class ParsableCodeGenerator : IInterfaceCodeGenerator<ParsableGeneratorState>
{
   public static readonly IInterfaceCodeGenerator<ParsableGeneratorState> Instance = new ParsableCodeGenerator();

   public string CodeGeneratorName => "Parsable-CodeGenerator";
   public string FileNameSuffix => ".Parsable";
   public bool CanAppendColon => true;

   public void GenerateBaseTypes(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"
   global::System.IParsable<").AppendTypeFullyQualified(state.Type).Append(">");
   }

   public void GenerateImplementation(StringBuilder sb, ParsableGeneratorState state)
   {
      GenerateParse(sb, state);
      GenerateTryParse(sb, state);
   }

   private static void GenerateParse(StringBuilder sb, ParsableGeneratorState state)
   {
      sb.Append(@"

   /// <inheritdoc />
   public static ").AppendTypeFullyQualified(state.Type).Append(@" Parse(string s, global::System.IFormatProvider? provider)
   {");

      if (state.KeyMember?.IsString() == true || state.HasStringBasedValidateMethod)
      {
         sb.Append(@"
      var validationError = global::Thinktecture.Internal.StaticAbstractInvoker.Validate<").AppendTypeFullyQualified(state.Type).Append(", string, ").AppendTypeFullyQualified(state.ValidationError).Append(">(s, provider, out var result);");
      }
      else if (state.KeyMember is not null)
      {
         sb.Append(@"
      var key = global::Thinktecture.Internal.StaticAbstractInvoker.ParseValue<").AppendTypeFullyQualified(state.KeyMember).Append(@">(s, provider);
      var validationError = global::Thinktecture.Internal.StaticAbstractInvoker.Validate<").AppendTypeFullyQualified(state.Type).Append(", ").AppendTypeFullyQualified(state.KeyMember).Append(", ").AppendTypeFullyQualified(state.ValidationError).Append(">(key, provider, out var result);");
      }

      sb.Append(@"

      if(validationError is null)
         return result!;

      throw new global::System.FormatException(validationError.ToString() ?? ""Unable to parse \""").Append(state.Type.Name).Append(@"\""."");
   }");
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

      if (state.KeyMember?.IsString() == true || state.HasStringBasedValidateMethod)
      {
         sb.Append(@"

      var validationError = global::Thinktecture.Internal.StaticAbstractInvoker.Validate<").AppendTypeFullyQualified(state.Type).Append(", string, ").AppendTypeFullyQualified(state.ValidationError).Append(">(s, provider, out result!);");
      }
      else if (state.KeyMember is not null)
      {
         sb.Append(@"

      if(!global::Thinktecture.Internal.StaticAbstractInvoker.TryParseValue<").AppendTypeFullyQualified(state.KeyMember).Append(@">(s, provider, out var key))
      {
         result = default;
         return false;
      }

      var validationError = global::Thinktecture.Internal.StaticAbstractInvoker.Validate < ").AppendTypeFullyQualified(state.Type).Append(",  ").AppendTypeFullyQualified(state.KeyMember).Append(", ").AppendTypeFullyQualified(state.ValidationError).Append(@" > (key, provider, out result!);
      ");
      }

      sb.Append(@"
      return validationError is null;
   }");
   }
}
