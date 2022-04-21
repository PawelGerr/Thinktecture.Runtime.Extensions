using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for Value Objects.
/// </summary>
[Generator]
public class ValueObjectSourceGenerator : ValueObjectSourceGeneratorBase<ValueObjectSourceGeneratorState>
{
   /// <inheritdoc />
   public ValueObjectSourceGenerator()
      : base(null)
   {
   }

   protected override ValueObjectSourceGeneratorState CreateState(INamedTypeSymbol type, AttributeData valueObjectAttribute)
   {
      return new ValueObjectSourceGeneratorState(type, valueObjectAttribute);
   }

   protected override string GenerateValueObject(ValueObjectSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      return new ValueObjectCodeGenerator(state, stringBuilderProvider.GetStringBuilder(10_000)).Generate();
   }
}
