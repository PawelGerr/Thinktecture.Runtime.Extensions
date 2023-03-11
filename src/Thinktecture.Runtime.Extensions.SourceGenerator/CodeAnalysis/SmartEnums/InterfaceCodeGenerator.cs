using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public class InterfaceCodeGenerator : CodeGeneratorBase
{
   private readonly IInterfaceCodeGenerator _codeGenerator;
   private readonly ITypeInformation _type;
   private readonly IMemberInformation _keyMember;
   private readonly StringBuilder _sb;

   public override string FileNameSuffix => _codeGenerator.FileNameSuffix;

   public InterfaceCodeGenerator(
      IInterfaceCodeGenerator codeGenerator,
      ITypeInformation type,
      IMemberInformation keyMember,
      StringBuilder stringBuilder)
   {
      _codeGenerator = codeGenerator;
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
partial ").Append(_type.IsReferenceType ? "class" : "struct").Append(" ").Append(_type.Name).Append(" :");

      _codeGenerator.GenerateBaseTypes(_sb, _type, _keyMember);

      _sb.Append(@"
{");

      _codeGenerator.GenerateImplementation(_sb, _type, _keyMember);

      _sb.Append(@"
}
");
   }
}
