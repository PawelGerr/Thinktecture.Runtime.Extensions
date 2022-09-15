using System.Text;

namespace Thinktecture.CodeAnalysis.SmartEnums;

public sealed class NewtonsoftJsonSmartEnumCodeGeneratorFactory : ICodeGeneratorFactory<EnumSourceGeneratorState>
{
   public CodeGeneratorBase Create(EnumSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new NewtonsoftJsonSmartEnumCodeGenerator(state);
   }

   public override bool Equals(object? obj)
   {
      return obj is NewtonsoftJsonSmartEnumCodeGeneratorFactory;
   }

   public bool Equals(ICodeGeneratorFactory<EnumSourceGeneratorState>? obj)
   {
      return obj is NewtonsoftJsonSmartEnumCodeGeneratorFactory;
   }

   public override int GetHashCode()
   {
      return GetType().GetHashCode();
   }
}
