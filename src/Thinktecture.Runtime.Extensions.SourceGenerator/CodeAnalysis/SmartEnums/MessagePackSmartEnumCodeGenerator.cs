using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class MessagePackSmartEnumCodeGenerator : CodeGeneratorBase
{
   private readonly EnumSourceGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string FileNameSuffix => ".MessagePack";

   public MessagePackSmartEnumCodeGenerator(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      if (_state.AttributeInfo.HasMessagePackFormatterAttribute)
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
[global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.").Append(_state.IsReferenceType ? "ValueObjectMessagePackFormatter" : "StructValueObjectMessagePackFormatter").Append("<").Append(_state.TypeFullyQualified).Append(", ").Append(_state.KeyProperty.TypeFullyQualified).Append(@">))]
partial ").Append(_state.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Name).Append(@"
{
}
");
   }
}
