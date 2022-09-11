using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public interface IInterfaceCodeGenerator
{
   void GenerateBaseTypes(StringBuilder sb, ITypeInformation type);
   void GenerateImplementation(StringBuilder sb, ITypeInformation type, IMemberState member);
}
