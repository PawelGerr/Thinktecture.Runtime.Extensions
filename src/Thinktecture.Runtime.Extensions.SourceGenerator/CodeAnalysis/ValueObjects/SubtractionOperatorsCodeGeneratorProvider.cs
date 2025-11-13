using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class SubtractionOperatorsCodeGeneratorProvider : IOperatorsCodeGeneratorProvider
{
   public static readonly IOperatorsCodeGeneratorProvider Instance = new SubtractionOperatorsCodeGeneratorProvider();

   private SubtractionOperatorsCodeGeneratorProvider()
   {
   }

   public bool TryGet(
      ImplementedOperators keyMemberOperators,
      OperatorsGeneration operatorsGeneration,
      [NotNullWhen(true)] out IInterfaceCodeGenerator? generator)
   {
      return SubtractionOperatorsCodeGenerator.TryGet(keyMemberOperators, operatorsGeneration, out generator);
   }
}
