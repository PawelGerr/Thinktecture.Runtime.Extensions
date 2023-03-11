using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class MessagePackSmartEnumCodeGeneratorFactory : ICodeGeneratorFactory<EnumSourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<EnumSourceGeneratorState> Instance = new MessagePackSmartEnumCodeGeneratorFactory();

   private MessagePackSmartEnumCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new MessagePackSmartEnumCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<EnumSourceGeneratorState> other)
   {
      return ReferenceEquals(this, other);
   }
}
