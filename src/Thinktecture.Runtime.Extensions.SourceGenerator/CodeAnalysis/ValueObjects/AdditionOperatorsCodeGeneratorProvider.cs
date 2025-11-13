using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class AdditionOperatorsCodeGeneratorProvider : IOperatorsCodeGeneratorProvider
{
   public static readonly IOperatorsCodeGeneratorProvider Instance = new AdditionOperatorsCodeGeneratorProvider();

   private AdditionOperatorsCodeGeneratorProvider()
   {
   }

   public bool TryGet(ImplementedOperators keyMemberOperators, OperatorsGeneration operatorsGeneration, [NotNullWhen(true)] out IInterfaceCodeGenerator? generator)
   {
      return AdditionOperatorsCodeGenerator.TryGet(keyMemberOperators, operatorsGeneration, out generator);
   }
}
