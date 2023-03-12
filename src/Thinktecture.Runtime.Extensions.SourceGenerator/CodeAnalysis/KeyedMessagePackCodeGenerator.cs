using System.Text;

namespace Thinktecture.CodeAnalysis;

public sealed class KeyedMessagePackCodeGenerator : CodeGeneratorBase
{
   private readonly ITypeInformation _type;
   private readonly ITypeFullyQualified _keyMember;
   private readonly StringBuilder _sb;

   public override string FileNameSuffix => ".MessagePack";

   public KeyedMessagePackCodeGenerator(ITypeInformation type, ITypeFullyQualified keyMember, StringBuilder stringBuilder)
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
[global::MessagePack.MessagePackFormatter(typeof(global::Thinktecture.Formatters.").Append(_type.IsReferenceType ? "ValueObjectMessagePackFormatter" : "StructValueObjectMessagePackFormatter").Append("<").Append(_type.TypeFullyQualified).Append(", ").Append(_keyMember.TypeFullyQualified).Append(@">))]
partial ").Append(_type.IsReferenceType ? "class" : "struct").Append(" ").Append(_type.Name).Append(@"
{
}
");
   }
}
