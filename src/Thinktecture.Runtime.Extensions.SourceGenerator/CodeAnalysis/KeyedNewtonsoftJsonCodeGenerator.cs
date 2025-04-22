using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class KeyedNewtonsoftJsonCodeGenerator : CodeGeneratorBase
{
   private readonly KeyedSerializerGeneratorState _state;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Keyed-NewtonsoftJson-CodeGenerator";
   public override string FileNameSuffix => ".NewtonsoftJson";

   public KeyedNewtonsoftJsonCodeGenerator(KeyedSerializerGeneratorState state, StringBuilder stringBuilder)
   {
      _state = state;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      var customFactory = _state.AttributeInfo
                                .ObjectFactories
                                .FirstOrDefault(f => f.UseForSerialization.Has(SerializationFrameworks.NewtonsoftJson));
      var keyType = customFactory?.TypeFullyQualified ?? _state.KeyMember?.TypeFullyQualified;

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
[global::Newtonsoft.Json.JsonConverterAttribute(typeof(global::Thinktecture.Json.ThinktectureNewtonsoftJsonConverter<").AppendTypeFullyQualified(_state.Type).Append(", ").Append(keyType).Append(", ").AppendTypeFullyQualified(_state.AttributeInfo.ValidationError).Append(@">))]
partial ").AppendTypeKind(_state.Type).Append(" ").Append(_state.Type.Name).Append(@"
{
}");

      _sb.RenderContainingTypesEnd(_state.Type.ContainingTypes)
         .Append(@"
");
   }
}
