using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class SpanParsableCodeGenerator : IInterfaceCodeGenerator<SpanParsableGeneratorState>
{
   public static readonly IInterfaceCodeGenerator<SpanParsableGeneratorState> ForValueObject = new SpanParsableCodeGenerator(false);
   public static readonly IInterfaceCodeGenerator<SpanParsableGeneratorState> ForEnum = new SpanParsableCodeGenerator(true);

   private readonly bool _isForEnum;

   public string CodeGeneratorName => "SpanParsable-CodeGenerator";
   public string FileNameSuffix => ".SpanParsable";
   public bool CanAppendColon => false;

   private SpanParsableCodeGenerator(bool isForEnum)
   {
      _isForEnum = isForEnum;
   }

   public void GenerateBaseTypes(StringBuilder sb, SpanParsableGeneratorState state)
   {
      sb.Append(@"
#if NET9_0_OR_GREATER
   : global::System.ISpanParsable<").AppendTypeFullyQualified(state.Type).Append(@">
#endif");
   }

   public void GenerateImplementation(StringBuilder sb, SpanParsableGeneratorState state)
   {
      GenerateParseForReadOnlySpanOfChar(sb, state);
      GenerateTryParseForReadOnlySpanOfChar(sb, state);
   }

   private static void GenerateParseForReadOnlySpanOfChar(StringBuilder sb, SpanParsableGeneratorState state)
   {
      sb.Append(@"

#if NET9_0_OR_GREATER
   /// <inheritdoc />
   public static ").AppendTypeFullyQualified(state.Type).Append(@" Parse(global::System.ReadOnlySpan<char> s, global::System.IFormatProvider? provider)
   {");

      if (state.IsEnum && state.KeyMember?.IsString() == true)
      {
         sb.Append(@"
      var validationError = ").AppendTypeFullyQualified(state.Type).Append(".Validate(s, provider, out var result);");
      }
      else if (state.HasReadOnlySpanOfCharBasedValidateMethod)
      {
         sb.Append(@"
      var validationError = global::Thinktecture.Internal.StaticAbstractInvoker.Validate<").AppendTypeFullyQualified(state.Type).Append(", global::System.ReadOnlySpan<char>, ").AppendTypeFullyQualified(state.ValidationError).Append(">(s, provider, out var result);");
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
   }
#endif");
   }

   private static void GenerateTryParseForReadOnlySpanOfChar(StringBuilder sb, SpanParsableGeneratorState state)
   {
      sb.Append(@"

#if NET9_0_OR_GREATER
   /// <inheritdoc />
   public static bool TryParse(
      global::System.ReadOnlySpan<char> s,
      global::System.IFormatProvider? provider,
      [global::System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out ").AppendTypeFullyQualified(state.Type).Append(@" result)
   {");

      if (state.KeyMember?.IsString() == true && state.IsEnum)
      {
         sb.Append(@"
      var validationError = ").AppendTypeFullyQualified(state.Type).Append(".Validate(s, provider, out result!);");
      }
      else if (state.HasReadOnlySpanOfCharBasedValidateMethod)
      {
         sb.Append(@"
      var validationError = global::Thinktecture.Internal.StaticAbstractInvoker.Validate<").AppendTypeFullyQualified(state.Type).Append(", global::System.ReadOnlySpan<char>, ").AppendTypeFullyQualified(state.ValidationError).Append(">(s, provider, out result!);");
      }
      else if (state.KeyMember is not null)
      {
         sb.Append(@"

      if(!global::Thinktecture.Internal.StaticAbstractInvoker.TryParseValue<").AppendTypeFullyQualified(state.KeyMember).Append(@">(s, provider, out var key))
      {
         result = default;
         return false;
      }

      var validationError = global::Thinktecture.Internal.StaticAbstractInvoker.Validate<").AppendTypeFullyQualified(state.Type).Append(", ").AppendTypeFullyQualified(state.KeyMember).Append(", ").AppendTypeFullyQualified(state.ValidationError).Append(">(key, provider, out result!);");
      }

      sb.Append(@"
      return validationError is null;
   }
#endif");
   }

   public override bool Equals(object? obj)
   {
      return obj is SpanParsableCodeGenerator other && _isForEnum == other._isForEnum;
   }

   public override int GetHashCode()
   {
      return _isForEnum.GetHashCode();
   }
}
