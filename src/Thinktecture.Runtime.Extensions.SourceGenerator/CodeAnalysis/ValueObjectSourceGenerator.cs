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
   protected override string GenerateValueObject(ValueObjectSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      return new ValueObjectCodeGenerator(state, stringBuilderProvider.GetStringBuilder(10_000)).Generate();
   }
}
