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
                                .ObjectFactories
                                .FirstOrDefault(f => f.UseForSerialization.Has(SerializationFrameworks.SystemTextJson));
      var keyType = customFactory?.TypeFullyQualified ?? _state.KeyMember?.TypeFullyQualified;
      var isString = customFactory is null
                        ? _state.KeyMember?.SpecialType == SpecialType.System_String
                        : customFactory.SpecialType == SpecialType.System_String;

      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_state.Type.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_state.Type.Namespace).Append(@";
");
      }

      _sb.RenderContainingTypesStart(_state.Type.ContainingTypes);

      _sb.Append(@"
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(global::Thinktecture.Text.Json.Serialization.ThinktectureJsonConverterFactory<").AppendTypeFullyQualified(_state.Type).Append(", ");

      if (!isString)
         _sb.Append(keyType).Append(", ");

      _sb.AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append(@">))]
partial ").Append(_state.Type.IsReferenceType ? "class " : "struct ").Append(_state.Type.Name).Append(@"
{
}");

      _sb.RenderContainingTypesEnd(_state.Type.ContainingTypes)
         .Append(@"
");
   }
}
