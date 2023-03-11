using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SmartEnumCodeGeneratorFactory : ICodeGeneratorFactory<EnumSourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<EnumSourceGeneratorState> Instance = new SmartEnumCodeGeneratorFactory();

   private SmartEnumCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new SmartEnumCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<EnumSourceGeneratorState> other)
   {
      return ReferenceEquals(this, other);
   }
}
