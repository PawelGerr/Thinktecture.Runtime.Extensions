using System.Text;

namespace Thinktecture.CodeAnalysis;

public interface IInterfaceCodeGenerator
{
   string CodeGeneratorName { get; }
   string FileNameSuffix { get; }

   void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember);
   void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember);
}
