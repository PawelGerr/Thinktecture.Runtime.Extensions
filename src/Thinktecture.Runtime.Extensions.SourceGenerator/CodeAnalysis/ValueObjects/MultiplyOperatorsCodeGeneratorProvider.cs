using System.Diagnostics.CodeAnalysis;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public class MultiplyOperatorsCodeGeneratorProvider : IOperatorsCodeGeneratorProvider
{
   public static readonly IOperatorsCodeGeneratorProvider Instance = new MultiplyOperatorsCodeGeneratorProvider();

   private MultiplyOperatorsCodeGeneratorProvider()
   {
   }

   public bool TryGet(ImplementedOperators keyMemberOperators, OperatorsGeneration operatorsGeneration, [MaybeNullWhen(false)] out IInterfaceCodeGenerator generator)
   {
      return MultiplyOperatorsCodeGenerator.TryGet(keyMemberOperators, operatorsGeneration, out generator);
   }
}
