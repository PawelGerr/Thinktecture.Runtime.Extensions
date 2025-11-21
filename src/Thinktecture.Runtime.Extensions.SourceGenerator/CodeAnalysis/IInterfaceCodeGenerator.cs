using System.Text;

namespace Thinktecture.CodeAnalysis;

public interface IInterfaceCodeGenerator : IInterfaceCodeGenerator<InterfaceCodeGeneratorState>;

public interface IInterfaceCodeGenerator<in TState>
{
   string CodeGeneratorName { get; }
   string FileNameSuffix { get; }
   bool CanAppendColon { get; }

   void GenerateBaseTypes(StringBuilder sb, TState state);
   void GenerateImplementation(StringBuilder sb, TState state);
}
