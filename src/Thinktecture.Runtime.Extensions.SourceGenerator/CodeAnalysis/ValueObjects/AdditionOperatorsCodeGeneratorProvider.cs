using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public class AdditionOperatorsCodeGeneratorProvider : IOperatorsCodeGeneratorProvider
{
   public static readonly IOperatorsCodeGeneratorProvider Instance = new AdditionOperatorsCodeGeneratorProvider();

   private AdditionOperatorsCodeGeneratorProvider()
   {
   }

   public bool TryGet(OperatorsGeneration operatorsGeneration, [MaybeNullWhen(false)] out IInterfaceCodeGenerator generator)
   {
      return AdditionOperatorsCodeGenerator.TryGet(operatorsGeneration, out generator);
   }
}
