using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class DivisionOperatorsCodeGeneratorProvider : IOperatorsCodeGeneratorProvider
{
   public static readonly IOperatorsCodeGeneratorProvider Instance = new DivisionOperatorsCodeGeneratorProvider();

   private DivisionOperatorsCodeGeneratorProvider()
   {
   }

   public bool TryGet(ImplementedOperators keyMemberOperators, OperatorsGeneration operatorsGeneration, [NotNullWhen(true)] out IInterfaceCodeGenerator? generator)
   {
      return DivisionOperatorsCodeGenerator.TryGet(keyMemberOperators, operatorsGeneration, out generator);
   }
}
