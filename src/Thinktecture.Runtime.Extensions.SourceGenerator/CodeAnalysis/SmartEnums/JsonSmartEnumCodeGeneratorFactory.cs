using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class JsonSmartEnumCodeGeneratorFactory : ICodeGeneratorFactory<EnumSourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<EnumSourceGeneratorState> Instance = new JsonSmartEnumCodeGeneratorFactory();

   private JsonSmartEnumCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new JsonSmartEnumCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<EnumSourceGeneratorState> other)
   {
      return ReferenceEquals(this, other);
   }
}
