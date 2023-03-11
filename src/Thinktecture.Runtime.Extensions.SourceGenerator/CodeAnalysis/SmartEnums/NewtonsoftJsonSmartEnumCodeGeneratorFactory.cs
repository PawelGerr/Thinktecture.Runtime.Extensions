using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class NewtonsoftJsonSmartEnumCodeGeneratorFactory : ICodeGeneratorFactory<EnumSourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<EnumSourceGeneratorState> Instance = new NewtonsoftJsonSmartEnumCodeGeneratorFactory();

   private NewtonsoftJsonSmartEnumCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new NewtonsoftJsonSmartEnumCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<EnumSourceGeneratorState> other)
   {
      return ReferenceEquals(this, other);
   }
}
