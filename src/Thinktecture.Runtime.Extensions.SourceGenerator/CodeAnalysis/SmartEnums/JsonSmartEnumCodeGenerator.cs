using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class JsonSmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string FileNameSuffix => ".Json";

   public JsonSmartEnumCodeGenerator(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      if (_state.AttributeInfo.HasJsonConverterAttribute)
         return;

      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_state.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_state.Namespace).Append(@";
");
      }

      _sb.Append(@"
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverterFactory<").Append(_state.TypeFullyQualified).Append(", ").Append(_state.KeyProperty.TypeFullyQualified).Append(@">))]
partial ").Append(_state.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Name).Append(@"
{
}
");
   }
}
