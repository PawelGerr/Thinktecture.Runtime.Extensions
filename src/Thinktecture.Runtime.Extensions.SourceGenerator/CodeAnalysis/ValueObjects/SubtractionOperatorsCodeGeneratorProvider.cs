using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public class SubtractionOperatorsCodeGeneratorProvider : IOperatorsCodeGeneratorProvider
{
   public static readonly IOperatorsCodeGeneratorProvider Instance = new SubtractionOperatorsCodeGeneratorProvider();

   private SubtractionOperatorsCodeGeneratorProvider()
   {
   }

   public bool TryGet(OperatorsGeneration operatorsGeneration, [MaybeNullWhen(false)] out IInterfaceCodeGenerator generator)
   {
      return SubtractionOperatorsCodeGenerator.TryGet(operatorsGeneration, out generator);
   }
}
