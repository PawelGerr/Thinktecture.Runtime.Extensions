using System.Text;

namespace Thinktecture.CodeAnalysis;

public interface IInterfaceCodeGenerator
{
   string FileNameSuffix { get; }

   void GenerateBaseTypes(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember);
   void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberInformation keyMember);
}
