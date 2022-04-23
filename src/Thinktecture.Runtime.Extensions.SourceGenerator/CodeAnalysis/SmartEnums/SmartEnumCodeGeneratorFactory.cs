using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public class SmartEnumCodeGeneratorFactory : ICodeGeneratorFactory<EnumSourceGeneratorState>
{
   public CodeGeneratorBase Create(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new SmartEnumCodeGenerator(state, stringBuilder);
   }

   public override bool Equals(object? obj)
   {
      return obj is SmartEnumCodeGeneratorFactory;
   }

   public bool Equals(ICodeGeneratorFactory<EnumSourceGeneratorState>? obj)
   {
      return obj is SmartEnumCodeGeneratorFactory;
   }

   public override int GetHashCode()
   {
      return GetType().GetHashCode();
   }
}
