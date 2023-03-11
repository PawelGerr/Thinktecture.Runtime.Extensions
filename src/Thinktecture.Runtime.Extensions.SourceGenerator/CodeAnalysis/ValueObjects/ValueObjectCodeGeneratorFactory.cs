using System.Text;

namespace Thinktecture.CodeAnalysis.ValueObjects;

public sealed class ValueObjectCodeGeneratorFactory : ICodeGeneratorFactory<ValueObjectSourceGeneratorState>
{
   public static readonly ICodeGeneratorFactory<ValueObjectSourceGeneratorState> Instance = new ValueObjectCodeGeneratorFactory();

   private ValueObjectCodeGeneratorFactory()
   {
   }

   public CodeGeneratorBase Create(ValueObjectSourceGeneratorState state, StringBuilder stringBuilder)
   {
      return new ValueObjectCodeGenerator(state, stringBuilder);
   }

   public bool Equals(ICodeGeneratorFactory<ValueObjectSourceGeneratorState>? obj)
   {
      return ReferenceEquals(this, obj);
   }
}
