using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public class DivisionOperatorsCodeGeneratorProvider : IOperatorsCodeGeneratorProvider
{
   public static readonly IOperatorsCodeGeneratorProvider Instance = new DivisionOperatorsCodeGeneratorProvider();

   private DivisionOperatorsCodeGeneratorProvider()
   {
   }

   public bool TryGet(OperatorsGeneration operatorsGeneration, [MaybeNullWhen(false)] out IInterfaceCodeGenerator generator)
   {
      return DivisionOperatorsCodeGenerator.TryGet(operatorsGeneration, out generator);
   }
}
