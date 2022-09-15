using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectCodeGeneratorFactory : ICodeGeneratorFactory<ValueObjectSourceGeneratorState>
{
   public CodeGeneratorBase Create(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new ValueObjectCodeGenerator(state, stringBuilder);
   }

   public override bool Equals(object? obj)
   {
      return obj is ValueObjectCodeGeneratorFactory;
   }

   public bool Equals(ICodeGeneratorFactory<ValueObjectSourceGeneratorState>? obj)
   {
      return obj is ValueObjectCodeGeneratorFactory;
   }

   public override int GetHashCode()
   {
      return GetType().GetHashCode();
   }
}
