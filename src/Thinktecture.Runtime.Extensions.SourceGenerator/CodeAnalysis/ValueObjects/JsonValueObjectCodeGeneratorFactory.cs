using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class JsonValueObjectCodeGeneratorFactory : ICodeGeneratorFactory<ValueObjectSourceGeneratorState>
{
   public CodeGeneratorBase Create(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new JsonValueObjectCodeGenerator(state, stringBuilder);
   }

   public override bool Equals(object? obj)
   {
      return obj is JsonValueObjectCodeGeneratorFactory;
   }

   public bool Equals(ICodeGeneratorFactory<ValueObjectSourceGeneratorState>? obj)
   {
      return obj is JsonValueObjectCodeGeneratorFactory;
   }

   public override int GetHashCode()
   {
      return GetType().GetHashCode();
   }
}
