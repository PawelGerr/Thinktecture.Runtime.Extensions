using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class KeyedJsonCodeGenerator : CodeGeneratorBase
{
   private readonly ITypeInformation _type;
   private readonly ITypeFullyQualified _keyMember;
   private readonly StringBuilder _sb;

   public override string CodeGeneratorName => "Keyed-SystemTextJson-CodeGenerator";
   public override string FileNameSuffix => ".Json";

   public KeyedJsonCodeGenerator(ITypeInformation type, ITypeFullyQualified keyMember, StringBuilder stringBuilder)
   {
      _type = type;
      _keyMember = keyMember;
      _sb = stringBuilder;
   }

   public override void Generate(CancellationToken cancellationToken)
   {
      _sb.Append(GENERATED_CODE_PREFIX).Append(@"
");

      if (_type.Namespace is not null)
      {
         _sb.Append(@"
namespace ").Append(_type.Namespace).Append(@";
");
      }

      _sb.Append(@"
[global::System.Text.Json.Serialization.JsonConverterAttribute(typeof(global::Thinktecture.Text.Json.Serialization.ValueObjectJsonConverterFactory<").Append(_type.TypeFullyQualified).Append(", ").Append(_keyMember.TypeFullyQualified).Append(@">))]
partial ").Append(_type.IsReferenceType ? "class" : "struct").Append(" ").Append(_type.Name).Append(@"
{
}
");
   }
}
