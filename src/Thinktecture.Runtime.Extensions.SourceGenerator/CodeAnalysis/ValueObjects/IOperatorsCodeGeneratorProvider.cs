using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public interface IOperatorsCodeGeneratorProvider
{
   bool TryGet(ImplementedOperators keyMemberOperators, OperatorsGeneration operatorsGeneration, [MaybeNullWhen(false)] out IInterfaceCodeGenerator generator);
}
