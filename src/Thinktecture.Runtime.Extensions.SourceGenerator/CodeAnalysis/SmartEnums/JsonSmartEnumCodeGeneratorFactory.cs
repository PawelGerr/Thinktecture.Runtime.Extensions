using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class JsonSmartEnumCodeGeneratorFactory : ICodeGeneratorFactory<EnumSourceGeneratorState>
{
   public CodeGeneratorBase Create(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new JsonSmartEnumCodeGenerator(state);
   }

   public override bool Equals(object? obj)
   {
      return obj is JsonSmartEnumCodeGeneratorFactory;
   }

   public bool Equals(ICodeGeneratorFactory<EnumSourceGeneratorState>? obj)
   {
      return obj is JsonSmartEnumCodeGeneratorFactory;
   }

   public override int GetHashCode()
   {
      return GetType().GetHashCode();
   }
}
