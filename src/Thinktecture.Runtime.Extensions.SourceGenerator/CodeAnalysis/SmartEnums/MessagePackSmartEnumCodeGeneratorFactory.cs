using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public class MessagePackSmartEnumCodeGeneratorFactory : ICodeGeneratorFactory<EnumSourceGeneratorState>
{
   public CodeGeneratorBase Create(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new MessagePackSmartEnumCodeGenerator(state);
   }

   public override bool Equals(object? obj)
   {
      return obj is MessagePackSmartEnumCodeGeneratorFactory;
   }

   public bool Equals(ICodeGeneratorFactory<EnumSourceGeneratorState>? obj)
   {
      return obj is MessagePackSmartEnumCodeGeneratorFactory;
   }

   public override int GetHashCode()
   {
      return GetType().GetHashCode();
   }
}
