using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class SmartEnumCodeGeneratorFactory : ICodeGeneratorFactory<SmartEnumSourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<SmartEnumSourceGeneratorState> Instance = new SmartEnumCodeGeneratorFactory();

   public string CodeGeneratorName => "SmartEnum-CodeGenerator";

   private SmartEnumCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(SmartEnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new SmartEnumCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<SmartEnumSourceGeneratorState> other)
   {
      return ReferenceEquals(this, other);
   }
}
