using Microsoft.CodeAnalysis;

namespace Thinktecture.CodeAnalysis;

/// <summary>
/// Source generator for enum-like class and value object.
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
