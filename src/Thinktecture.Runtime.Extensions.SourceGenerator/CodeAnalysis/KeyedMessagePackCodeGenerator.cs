using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class KeyedMessagePackCodeGenerator : CodeGeneratorBase
{
   private readonly KeyedSerializerGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Keyed-MessagePack-CodeGenerator";
   public override string FileNameSuffix => ".MessagePack";

   public KeyedMessagePackCodeGenerator(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      var customFactory = _state.AttributeInfo
                                .ObjectFactories
                                .FirstOrDefault(f => f.UseForSerialization.HasSerializationFramework(SerializationFrameworks.MessagePack));
      var keyType = customFactory?.TypeFullyQualified ?? _state.KeyMember?.TypeFullyQualified;

      if (keyType is null)
         return;

      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_state.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_state.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_state.ContainingTypes);

      var hasGenerics = !_state.GenericParameters.IsDefaultOrEmpty;

      _sb.Append(@"
[global::MessagePack.MessagePackFormatter(typeof(");

      if (hasGenerics)
      {
         _sb.AppendTypeFullyQualifiedWithoutGenerics(_state, _state.ContainingTypes).AppendGenericTypeParameters(_state, constructOpenGeneric: true).Append(".ValueObjectMessagePackFormatter");
      }
      else
      {
         _sb.Append("global::Thinktecture.Formatters.").Append(_state.Type.IsReferenceType ? "ThinktectureMessagePackFormatter" : "ThinktectureStructMessagePackFormatter").Append("<").AppendTypeFullyQualified(_state.Type).Append(", ").Append(keyType).Append(", ").AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append(">");
      }

      _sb.Append(@"))]
");

      _sb.Append("partial ").AppendTypeKind(_state.Type).Append(" ").Append(_state.Name).AppendGenericTypeParameters(_state).Append(@"
{");

      if (hasGenerics)
         GenerateFormatter(keyType);

      _sb.Append(@"
}");

      _sb.RenderContainingTypesEnd(_state.ContainingTypes);

      _sb.Append(@"
");
   }

   private void GenerateFormatter(string keyType)
   {
      _sb.Append(@"

   public class ValueObjectMessagePackFormatter : global::Thinktecture.Formatters.").Append(_state.Type.IsReferenceType ? "ThinktectureMessagePackFormatter" : "ThinktectureStructMessagePackFormatter").Append("<").AppendTypeFullyQualified(_state.Type).Append(", ").Append(keyType).Append(", ").AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append(@">
   {
   }");
   }
}
