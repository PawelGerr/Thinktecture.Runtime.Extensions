using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public interface IOperatorsCodeGeneratorProvider
{
   bool TryGet(ImplementedOperators keyMemberOperators, OperatorsGeneration operatorsGeneration, [NotNullWhen(true)] out IInterfaceCodeGenerator? generator);
}
