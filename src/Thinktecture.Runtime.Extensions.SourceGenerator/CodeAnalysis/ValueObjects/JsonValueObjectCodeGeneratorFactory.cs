using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class JsonValueObjectCodeGeneratorFactory : ICodeGeneratorFactory<ValueObjectSourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<ValueObjectSourceGeneratorState> Instance = new JsonValueObjectCodeGeneratorFactory();

   private JsonValueObjectCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new JsonValueObjectCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<ValueObjectSourceGeneratorState>? obj)
   {
      return obj is JsonValueObjectCodeGeneratorFactory;
   }
}
