using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class KeyedJsonCodeGenerator : CodeGeneratorBase
{
   private readonly KeyedSerializerGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Keyed-SystemTextJson-CodeGenerator";
   public override string FileNameSuffix => ".Json";

   public KeyedJsonCodeGenerator(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      var customFactory = _state.AttributeInfo
                                .DesiredFactories
                                .FirstOrDefault(f => f.UseForSerialization.HasFlag(SerializationFrameworks.SystemTextJson));
      var keyType = customFactory?.TypeFullyQualified ?? _state.KeyMember?.TypeFullyQualified;

      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_state.Type.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_state.Type.Namespace).Append(@";
");
      }

      _sb.Append(@"
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverterFactory<").Append(_state.Type.TypeFullyQualified).Append(", ").Append(keyType).Append(@">))]
partial ").Append(_state.Type.IsReferenceType ? "class" : "struct").Append(" ").Append(_state.Type.Name).Append(@"
{
}
");
   }
}
