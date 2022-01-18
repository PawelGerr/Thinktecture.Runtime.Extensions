using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for Smart Enums.
/// </summary>
[Generator]
public class SmartEnumSourceGenerator : SmartEnumSourceGeneratorBase
{
   /// <inheritdoc />
   public SmartEnumSourceGenerator()
      : base(null)
   {
   }

   /// <inheritdoc />
   protected override string GenerateEnum(EnumSourceGeneratorState state)
   {
      return new SmartEnumCodeGenerator(state).Generate();
   }
}
