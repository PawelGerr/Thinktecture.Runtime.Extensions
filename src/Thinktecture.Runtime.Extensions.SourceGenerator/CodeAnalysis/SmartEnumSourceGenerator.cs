using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

[Generator]
public class SmartEnumSourceGenerator : SmartEnumSourceGeneratorBase<EnumSourceGeneratorState, int>
{
   public SmartEnumSourceGenerator()
      : base(null)
   {
   }

   protected override EnumSourceGeneratorState CreateState(INamedTypeSymbol type, INamedTypeSymbol enumInterface)
   {
      return new EnumSourceGeneratorState(type, enumInterface);
   }

   protected override string GenerateEnum(EnumSourceGeneratorState state, StringBuilderProvider stringBuilderProvider)
   {
      return new SmartEnumCodeGenerator(state, stringBuilderProvider.GetStringBuilder(12_000)).Generate();
   }
}
