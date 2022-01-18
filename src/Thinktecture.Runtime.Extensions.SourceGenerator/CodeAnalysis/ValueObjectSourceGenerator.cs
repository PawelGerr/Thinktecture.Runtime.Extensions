using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for Value Objects.
/// </summary>
[Generator]
public class ValueObjectSourceGenerator : ValueObjectSourceGeneratorBase
{
   /// <inheritdoc />
   public ValueObjectSourceGenerator()
      : base(null)
   {
   }

   /// <inheritdoc />
   protected override string GenerateValueObject(ValueObjectSourceGeneratorState state)
   {
      return new ValueObjectCodeGenerator(state).Generate();
   }
}
